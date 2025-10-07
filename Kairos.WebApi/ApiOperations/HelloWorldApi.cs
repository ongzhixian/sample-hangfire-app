namespace Kairos.WebApi.ApiOperations;

public class HelloWorldApi
{
    public string Greet() => "Hello World from DI Service!";
}

public static partial class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapHelloWorldApi(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("hello", (HelloWorldApi api) => api.Greet());
        
        return endpoints;
    }
}