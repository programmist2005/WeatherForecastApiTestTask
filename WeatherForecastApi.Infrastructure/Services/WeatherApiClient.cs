using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using WeatherForecast.Api.Implementation.Mapping;
using WeatherForecastApi.Application.Exceptions;
using WeatherForecastApi.Application.Interfaces;
using WeatherForecastApi.Domain.Models;
using WeatherForecastApi.Infrastructure.Mapping;
using WeatherForecastApi.Infrastructure.Models;
using WeatherForecastApi.Infrastructure.Options;

namespace WeatherForecast.Api.Implementation.Services;

public class WeatherApiClient(
    HttpClient httpClient,
    IOptionsSnapshot<ConfigurationOptions> options,
    UrlFromConfigurationOptionsMapper configurationOptionsToUrlMapper,
    WeatherApiFromResponseMapper weatherApiFromResponseMapper,
    ILogger<WeatherApiClient> logger
) : IWeatherApiClient
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ConfigurationOptions _options = options.Value;
    private readonly UrlFromConfigurationOptionsMapper _configurationOptionsToUrlMapper = configurationOptionsToUrlMapper;
    private readonly ILogger<WeatherApiClient> _logger = logger;

    public async Task<WeatherApiModel> GetAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(_configurationOptionsToUrlMapper.Map(_options));
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var weatherApiResponse = JsonSerializer.Deserialize<WeatherApiResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new Exception("Не удалось десериализовать ответ API");

            var weatherApiModel = weatherApiFromResponseMapper.Map(weatherApiResponse);
            return weatherApiModel;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new ApplicationConfigurationException();
        }
        catch (ArgumentOutOfRangeException ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new ApplicationConfigurationException();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Ошибка при выполнении HTTP-запроса: {Message}", ex.Message);
            throw new ApplicationWeatherAPIException();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Ошибка при десериализации ответа API: {Message}", ex.Message);
            throw new ApplicationWeatherAPIException();
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "HTTP-запрос был отменен: {Message}", ex.Message);
            throw new ApplicationWeatherAPIException();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new ApplicationWeatherAPIException();
        }
    }
}
