using Kairos.WebApi.ApiOperations;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddOpenApi();

    builder.Services.AddSerilog((_, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(builder.Configuration));

    builder.Services.AddSwaggerGen();
    
    builder.Services.AddHealthChecks();
    
    builder.Services.AddScoped<HelloWorldApi>();
        
    var app = builder.Build();
    
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        
        app.MapOpenApi();
    }
    
    app.UseHttpsRedirection();
    
    app.UseDefaultFiles();
    
    app.UseStaticFiles();

    app.MapHealthChecks("health");
    
    app.MapHelloWorldApi();
  
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