using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebService2.Controllers;
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [Authorize]
    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>?> GetAsync(CancellationToken cancellationToken = default)
    {
        HttpClient client = new()
        {
            BaseAddress = new("https://localhost:7118/"),
        };

        HttpRequestMessage req = new(HttpMethod.Get, "WeatherForecast");

        // copia i cookie del chiamante
        foreach (string? cookie in Request.Headers["Cookie"])
            req.Headers.Add("Cookie", cookie);

        HttpResponseMessage resp = await client.SendAsync(req, cancellationToken);

        IEnumerable<WeatherForecast>? result = await resp.Content.ReadFromJsonAsync<IEnumerable<WeatherForecast>>(cancellationToken: cancellationToken);

        return result;
    }
}
