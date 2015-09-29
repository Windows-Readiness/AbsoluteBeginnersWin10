using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;






namespace UWPWeatherService.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public async Task<ActionResult> Index(string lat, string lon)
        {
            // Validation / trimming
            var latitude = double.Parse(lat.Substring(0, 5));
            var longitude = double.Parse(lon.Substring(0, 5));

            var weather = await Models.OpenWeatherMapProxy.GetWeather(latitude, longitude);

            ViewBag.Name = weather.name;
            ViewBag.Temp = ((int)weather.main.temp).ToString();
            ViewBag.Description = weather.weather[0].description;

            return View();
        }
    }
}