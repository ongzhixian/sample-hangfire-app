using Hangfire;

namespace Kairos.WebApi.Services;

public class HangfireJobFactory
{
    [JobDisplayName("{0}")]
    public async Task CreateSimulateJob(string jobDisplayName, int jobTimeInSeconds = 0)
    {
        Console.WriteLine("Simulate doing {0}.", jobDisplayName);
        
        await Task.Delay(TimeSpan.FromSeconds(jobTimeInSeconds + Random.Shared.Next(0, 6)));
        
        Console.WriteLine("{0} completed.", jobDisplayName);
    }
}