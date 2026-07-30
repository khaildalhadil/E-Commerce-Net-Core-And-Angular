using API.Middleware;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Database;
using Infrastructure.services;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Serilog;
using StackExchange.Redis;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    Log.Information(
        "Starting StoreListing API in {Environment} environment on {MachineName}",
        builder.Environment.EnvironmentName,
        Environment.MachineName);

    builder.Services.AddCors();
    builder.Host.UseSerilog(
        (context, services, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services);
        });

    // Add services to the container.

    builder.Services.AddControllers();

    // [ApiController] short-circuits invalid models before the action runs, so validation
    // failures can only be observed here. The built-in factory still builds the response,
    // which keeps the 400 payload byte-for-byte identical.
    builder.Services.PostConfigure<ApiBehaviorOptions>(options =>
    {
        var builtInFactory = options.InvalidModelStateResponseFactory;

        options.InvalidModelStateResponseFactory = actionContext =>
        {
            var validationLogger = actionContext.HttpContext.RequestServices
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("API.ModelValidation");

            // Field names only - attempted values may contain user data and are never logged.
            var invalidFields = actionContext.ModelState
                .Where(entry => entry.Value?.Errors.Count > 0)
                .Select(entry => entry.Key)
                .ToArray();

            validationLogger.LogWarning(
                "Validation failed for {RequestMethod} {RequestPath} on fields {InvalidFields}",
                actionContext.HttpContext.Request.Method,
                actionContext.HttpContext.Request.Path,
                invalidFields);

            return builtInFactory(actionContext);
        };
    });

    // StoreContext added to DI Con
    var connectionString = builder.Configuration.GetConnectionString("Default");

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        // Without a connection string every request that touches the database will fail,
        // so surface it at startup instead of as a wall of runtime errors.
        Log.Fatal("Connection string {ConnectionName} is missing; the API cannot serve data", "Default");
    }
    else
    {
        // Log only the non-sensitive parts of the connection string - never username/password.
        var dbInfo = new NpgsqlConnectionStringBuilder(connectionString);
        Log.Information(
            "Database configured: {DbHost}:{DbPort}/{DbName}",
            dbInfo.Host,
            dbInfo.Port,
            dbInfo.Database);
    }

    builder.Services.AddDbContext<StoreContext>(options =>
    {
        options.UseNpgsql(connectionString);
    });

    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
    builder.Services.AddScoped<ITokenService, TokenService>();

    builder.Services.AddIdentityCore<AppUser>(options =>
    {
        options.Password.RequireNonAlphanumeric = false;
    })
        .AddEntityFrameworkStores<StoreContext>();

    //var tokenKey = builder.Configuration["Token:Key"]
    //    ?? throw new InvalidOperationException("Token:Key is missing from configuration");

    //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //    .AddJwtBearer(options =>
    //    {
    //        options.TokenValidationParameters = new TokenValidationParameters
    //        {
    //            ValidateIssuerSigningKey = true,
    //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
    //            ValidateIssuer = false,
    //            ValidateAudience = false,
    //        };
    //    });


    //// loggen config
    //builder.Services.AddHttpLogging(options =>
    //{
    //    options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPropertiesAndHeaders;
    //    options.RequestHeaders.Add("Authorization");
    //});

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // connet with Redis
    builder.Services.AddSingleton<IConnectionMultiplexer>(config =>
    {
        var connString = builder.Configuration.GetConnectionString("Redis");
        if(connString is null)
        {
            Log.Error("Cannot get redis connection string");
            throw new Exception("Cannot get redis connection string");
        }

        var configuration = ConfigurationOptions.Parse(connString, true);
        return ConnectionMultiplexer.Connect(configuration);
    });

    builder.Services.AddSingleton<ICartService, CartService>();

    // "Prefer: wait" can hold the connection open for up to ~60s on Replicate's side
    // before falling back to polling, so the client timeout must clear that with margin.
    builder.Services.AddHttpClient("Replicate", client =>
    {
        client.Timeout = TimeSpan.FromMinutes(3);
    });

    builder.Services.AddHttpClient("OpenAI", client =>
    {
        client.Timeout = TimeSpan.FromSeconds(60);
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";

        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("UserName", httpContext.User?.Identity?.Name ?? "anonymous");

            diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");

            // Correlates every request-completion event with the errors logged by ExceptionMiddlware.
            diagnosticContext.Set("TraceId", httpContext.TraceIdentifier);

            if(httpContext.User?.Identity?.IsAuthenticated == true) {
                diagnosticContext.Set("UserId", httpContext.User.FindFirst("sub")?.Value ?? "unknown");
            };
        };
    });

    //app.UseHttpLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        Log.Information("Development environment detected, enabling Swagger UI");
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<ExceptionMiddlware>();
    app.UseCors(x=> x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    try
    {
        // it will be destore when it will finish and to add only scope
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<StoreContext>();

        Log.Information("Applying database migrations");
        await context.Database.MigrateAsync();

        await StoreContextSeed.SeedAsync(context);

        Log.Information("Database migration and seeding completed");
    }
    catch (Exception ex)
    {
        // The app deliberately keeps starting after a seeding failure, so this is an
        // error - not fatal - but it needs the full exception to be diagnosable.
        Log.Error(ex, "Database migration or seeding failed; API is starting with a possibly incomplete database");
    }

    Log.Information("StoreListing API started successfully");

    app.Run();

} catch(Exception err)
{
    Log.Fatal(err, "Application terminated unexpectedly during startup");
}
finally
{
    Log.Information("Shutting down StoreListing API");
    Log.CloseAndFlush();
}
