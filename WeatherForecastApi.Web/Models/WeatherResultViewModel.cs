using WeatherForecastApi.Application.Models;

namespace WeatherForecastApi.Web.Models
{
    public class WeatherResultViewModel
    {
        public WeatherQueryResult? Result { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; } = null!;
    }
}
