using Application.Interfaces;
using Infrastructure.Database;
using Infrastructure.services;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// StoreContext added to DI Con
builder.Services.AddDbContext<StoreContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped(typeof (IGenericService<>), typeof (GenericService<>));

builder.Services.AddHttpLogging();

builder.Host.ConfigureLogging(log =>
{
    log.ClearProviders();
    log.AddConsole();
    log.AddDebug();
    log.AddEventLog();
});

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields =
        Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestHeaders |
        Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestQuery;
});

var app = builder.Build();

app.UseHttpLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.MapControllers();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

try
{
    // it will be destore when it will finish and to add only scope
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StoreContext>();
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context);
} catch(Exception ex)
{
    Console.WriteLine(ex);
}

app.Run();