using MediatR;
using Microsoft.AspNetCore.Mvc;
using WeatherForecastApi.Application.Exceptions;
using WeatherForecastApi.Application.Queries;
using WeatherForecastApi.Web.Models;

namespace WeatherForecastApi.Web.Controllers
{
    public class WeatherController : Controller
    {
        private readonly IMediator _mediator;

        public WeatherController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var result = new WeatherResultViewModel();
            try
            {
                result.Result = await _mediator.Send(new GetWeatherQuery());
            }
            catch (ApplicationConfigurationException)
            {
                result.IsError = true;
                result.ErrorMessage = "Ошибка конфигурации запроса";
            }
            catch (ApplicationWeatherAPIException)
            {
                result.IsError = true;
                result.ErrorMessage = "Ошибка получения данных о погоде";
            }
            catch (ApplicationUnknownException)
            {
                result.IsError = true;
                result.ErrorMessage = "Произошла неизвестная ошибка";
            }
            return View(result);
        }
    }
}
