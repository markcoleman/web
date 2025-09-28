using Serilog;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;
using UmbracoWeb.Models;
using UmbracoWeb.Services;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog
    builder.Host.UseSerilog();

    // Configure PhishLabs settings
    builder.Services.Configure<PhishLabsSettings>(
        builder.Configuration.GetSection(PhishLabsSettings.SectionName));

    // Add HTTP client for PhishLabs service
    builder.Services.AddHttpClient<IPhishLabsService, PhishLabsService>();

    // Register PhishLabs service
    builder.Services.AddScoped<IPhishLabsService, PhishLabsService>();

    // Add antiforgery services
    builder.Services.AddAntiforgery(options =>
    {
        options.HeaderName = "X-CSRF-TOKEN";
        options.SuppressXFrameOptionsHeader = false;
    });

    // Add CORS policy
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("PhishLabsPolicy", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .WithMethods("POST");
        });
    });

    // Add Umbraco
    builder.CreateUmbracoBuilder()
        .AddBackOffice()
        .AddWebsite()
        .AddDeliveryApi()
        .AddComposers()
        .Build();

    var app = builder.Build();

    // Configure the HTTP request pipeline
    await app.BootUmbracoAsync();

    // Use CORS
    app.UseCors("PhishLabsPolicy");

    app.UseUmbraco()
        .WithMiddleware(u =>
        {
            u.UseBackOffice();
            u.UseWebsite();
        })
        .WithEndpoints(u =>
        {
            u.UseBackOfficeEndpoints();
            u.UseWebsiteEndpoints();
        });

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
