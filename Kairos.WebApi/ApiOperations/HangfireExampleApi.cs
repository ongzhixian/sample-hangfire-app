using Hangfire;
using Kairos.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kairos.WebApi.ApiOperations;

public class HangfireExampleApi
{
    private readonly ILogger<HangfireExampleApi> logger;
    
    public HangfireExampleApi(ILogger<HangfireExampleApi> logger)
    {
        this.logger = logger;
    }

    public void ExampleOfFireAndForgetJob(string jobTitle = "Example of a Fire-and-Forget Job", int jobTimeInSeconds = 0)
    {
        logger.LogInformation("Enqueue an example of a Fire-and-Forget Job");
        BackgroundJob.Enqueue<HangfireJobFactory>( 
            (svc) => svc.CreateSimulateJob(jobTitle, jobTimeInSeconds)
            );
    }
    
    public void ExampleOfDelayedJob()
    {
        logger.LogInformation("Enqueue an example of a Delayed (5 minutes) Job"); 
        BackgroundJob.Schedule<HangfireJobFactory>(
            (svc) => svc.CreateSimulateJob("Delayed Job Example (delayed for 5 minutes)", 0),
            TimeSpan.FromMinutes(5));
    }
    
    public void ExampleOfContinuationJob()
    {
        logger.LogInformation("Enqueue an example of a start of Continuation Job");
        var jobId = BackgroundJob.Enqueue<HangfireJobFactory>(
            (svc) => svc.CreateSimulateJob("Example of job with continuation", 0)
            );
        
        logger.LogInformation("Enqueue an example of a Continuation Job");
        BackgroundJob.ContinueJobWith<HangfireJobFactory>(
            jobId,
            (svc) => svc.CreateSimulateJob("Example of a Continuation Job", 0)
            );
    }
    
    public void ExampleOfRecurringJob()
    {
        logger.LogInformation("Enqueue an example of a start of Continuation Job");
        
        RecurringJob.AddOrUpdate<HangfireJobFactory>(
            "example-recurring-job",
            (svc) => svc.CreateSimulateJob("Example of recurring job", 0),
            Cron.Hourly
            );
    }
}



public static partial class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapHangfireExampleApi(this IEndpointRouteBuilder endpoints)
    {
        string[] hangfireExampleTags = ["Hangfire Example"];
        string prefix = "hangfire-example";
        
        endpoints.MapPost($"{prefix}/fire-and-forget-job", ([FromServices]HangfireExampleApi api
                , int numberOfJobs = 1, int jobTimeInSeconds = 0) =>
                {
                    for (var index = 0; index < numberOfJobs; index++)
                        api.ExampleOfFireAndForgetJob($"Example of a Fire-and-Forget Job ({index+1} of {numberOfJobs})", jobTimeInSeconds);
                })
            .WithTags(hangfireExampleTags);
        
        endpoints.MapPost($"{prefix}/delayed-job", (HangfireExampleApi api) => api.ExampleOfDelayedJob())
            .WithTags(hangfireExampleTags);
        
        endpoints.MapPost($"{prefix}/continuation-job", (HangfireExampleApi api) => api.ExampleOfContinuationJob())
            .WithTags(hangfireExampleTags);
        
        endpoints.MapPost($"{prefix}/recurring-job", (HangfireExampleApi api) => api.ExampleOfRecurringJob())
            .WithTags(hangfireExampleTags);
        
        
        
        return endpoints;
    }
}