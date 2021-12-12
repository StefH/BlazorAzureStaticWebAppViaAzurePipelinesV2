using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BlazorApp.Shared;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace ApiIsolated;

public class HttpTrigger
{
    private readonly ILogger _logger;

    public HttpTrigger(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<HttpTrigger>();
    }

    [Function("WeatherForecast")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        _logger.LogInformation("WeatherForecast - Run");

        var randomNumber = new Random();
        var temp = 0;

        var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = temp = randomNumber.Next(-20, 55),
            Summary = GetSummary(temp)
        }).ToArray();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(result);

        return response;
    }

    private static string GetSummary(int temp)
    {
        return temp switch
        {
            >= 32 => "Hot",
            <= 16 and > 0 => "Cold",
            <= 0 => "Freezing",
            _ => "Mild",
        };
    }
}