using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddOpenApi();

    builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(builder.Configuration));

    builder.Services.AddSwaggerGen();
        
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