using Hangfire;

namespace Kairos.WebApi.ApiOperations;

public class HangfireExampleApi
{
    private readonly ILogger<HangfireExampleApi> logger;
    
    public HangfireExampleApi(ILogger<HangfireExampleApi> logger)
    {
        this.logger = logger;
    }

    [JobDisplayName("Fire-and-Forget Job Example")]
    public void ExampleOfFireAndForgetJob()
    {
        logger.LogInformation("Enqueue an example of a Fire-and-Forget Job");
        var jobId = BackgroundJob.Enqueue(
            () => Test()
            );
    }

    [JobDisplayName("Fire-and-Forget Job Example")]
    public void Test()
    {
        Console.WriteLine("Hello, world!");
    }
    
    [JobDisplayName("Delayed Job Example")]
    public void ExampleOfDelayedJob()
    {
        logger.LogInformation("Enqueue an example of a Delayed (5 minutes) Job");
        var jobId = BackgroundJob.Schedule(
            () => Console.WriteLine("Delayed!"),
            TimeSpan.FromMinutes(5));
    }
    
    [JobDisplayName("Continuation Job Example")]
    public void ExampleOfContinuationJob()
    {
        logger.LogInformation("Enqueue an example of a start of Continuation Job");
        var jobId = BackgroundJob.Enqueue(() => Console.WriteLine("Hello, world!"));
        
        logger.LogInformation("Enqueue an example of a Continuation Job");
        BackgroundJob.ContinueJobWith(
            jobId,
            () => Console.WriteLine("Continuation!"));
        
    }
    
    [JobDisplayName("Recurring Job Example")]
    public void ExampleOfRecurringJob()
    {
        logger.LogInformation("Enqueue an example of a start of Continuation Job");
        
        RecurringJob.AddOrUpdate(
            "example-recurring-job",
            () => Console.WriteLine("Recurring!"),
            Cron.Hourly);
    }
}



public static partial class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapHangfireExampleApi(this IEndpointRouteBuilder endpoints)
    {
        string[] hangfireExampleTags = ["Hangfire Example"];
        string prefix = "hangfire-example";
        
        endpoints.MapGet($"{prefix}/fire-and-forget-job", (HangfireExampleApi api) => api.ExampleOfFireAndForgetJob())
            .WithTags(hangfireExampleTags);
        
        endpoints.MapGet($"{prefix}/delayed-job", (HangfireExampleApi api) => api.ExampleOfDelayedJob())
            .WithTags(hangfireExampleTags);
        
        endpoints.MapGet($"{prefix}/continuation-job", (HangfireExampleApi api) => api.ExampleOfContinuationJob())
            .WithTags(hangfireExampleTags);
        
        endpoints.MapGet($"{prefix}/recurring-job", (HangfireExampleApi api) => api.ExampleOfRecurringJob())
            .WithTags(hangfireExampleTags);
        
        return endpoints;
    }
}