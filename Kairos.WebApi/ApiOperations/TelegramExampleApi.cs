using ReadyPerfectly.InstantMessaging;

namespace Kairos.WebApi.ApiOperations;

public class TelegramExampleApi
{
    private readonly ILogger<TelegramExampleApi> logger;
    private readonly ReadyPerfectly.InstantMessaging.TelegramClient telegramClient;
    
    public TelegramExampleApi(ILogger<TelegramExampleApi> logger
        , TelegramClientFactory telegramClientFactory
        )
    {
        this.logger = logger;
        this.telegramClient = telegramClientFactory.Create("zxDevOpsBot");
    }

    public async Task<IResult> GetMe()
    {
        logger.LogInformation("GetMe");

        if (await telegramClient.GetMeAsync() is { } response)
            return TypedResults.Ok(response);    

        return TypedResults.Empty;
    }
}


public static partial class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapTelegramExampleApi(this IEndpointRouteBuilder endpoints)
    {
        string[] openApiTags = ["Telegram Example"];
        string prefix = "telegram-example";
        
        endpoints.MapGet($"{prefix}/get-me", (TelegramExampleApi api) => api.GetMe())
            .WithTags(openApiTags);

        return endpoints;
    }
}