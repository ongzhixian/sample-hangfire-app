using Hangfire;

using Kairos.WebApi.ApiOperations;
using Kairos.WebApi.Services;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.OpenApi.Models;
using ReadyPerfectly.InstantMessaging;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    
    builder.Services.AddOpenApi();

    builder.Services.AddSerilog((_, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(builder.Configuration));
    
    const string swaggerDocId = "kairos-api";
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc(swaggerDocId, new OpenApiInfo
        {
            Title = "Kairos Web API",
            Version = "v1",
            Description = "Kairos Web API"
        });
    });

    builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"))
        .UseSerilogLogProvider()
    );
    
    builder.Services.AddHangfireServer();
    
    builder.Services.AddHttpClient();
    builder.Services.AddHttpLogging(httpLoggingOptions =>
    {
        httpLoggingOptions.LoggingFields = HttpLoggingFields.All; 
    });
    
    builder.Services.AddScoped<TelegramClientFactory>();

    builder.Services.AddHealthChecks();
    
    builder.Services.AddScoped<HelloWorldApi>();

    builder.Services.AddScoped<HangfireExampleApi>();
    builder.Services.AddScoped<TelegramExampleApi>();
    builder.Services.AddScoped<HangfireJobFactory>();
        
    var app = builder.Build();
    
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options => 
            options .SwaggerEndpoint($"/{options.RoutePrefix}/{swaggerDocId}/swagger.json", swaggerDocId)
        );
        
        app.MapOpenApi();
    }
    
    app.UseHttpLogging();
    
    app.UseHttpsRedirection();
    
    app.UseDefaultFiles();
    
    app.UseStaticFiles();
    
    app.UseHangfireDashboard();
    
    app.MapHealthChecks("health");
    
    app.MapHelloWorldApi();

    app.MapHangfireExampleApi();
    app.MapTelegramExampleApi();
  
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}