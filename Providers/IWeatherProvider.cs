using System.Collections.Generic;
using Tac.Models;

namespace Tac.Providers
{
    public interface IWeatherProvider
    {
        List<WeatherForecast> GetForecasts();
    }
}