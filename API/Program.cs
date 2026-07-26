using Application.Interfaces;
using Infrastructure.Database;
using Infrastructure.services;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
    Log.Information("Starting SotreListing API");
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog(
        (context, services, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services);
        });

    // Add services to the container.

    builder.Services.AddControllers();

    // StoreContext added to DI Con
    builder.Services.AddDbContext<StoreContext>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
    });

    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));


    //// loggen config
    //builder.Services.AddHttpLogging(options =>
    //{
    //    options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPropertiesAndHeaders;
    //    options.RequestHeaders.Add("Authorization");
    //});

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";

        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("UserName", httpContext.User?.Identity?.Name ?? "anonymous");

            diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknoown");

            if(httpContext.User?.Identity?.IsAuthenticated == true) {
                diagnosticContext.Set("UserId", httpContext.User.FindFirst("sub")?.Value ?? "unknown");
            };
        };
    });

    //app.UseHttpLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        Log.Information("We Are In Development Env");
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapControllers();

    try
    {
        // it will be destore when it will finish and to add only scope
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<StoreContext>();
        await context.Database.MigrateAsync();
        await StoreContextSeed.SeedAsync(context);
        Log.Information("Seed Product Done");
    }
    catch (Exception ex)
    {
        Log.Fatal(ex.Message);
        Console.WriteLine(ex);
    }

    Log.Information("HotelLinsting API stared Successfully");

    app.Run();

} catch(Exception err)
{
    Log.Fatal(err.Message, "Application terminated ");
}
finally
{
    Log.CloseAndFlush();
}
