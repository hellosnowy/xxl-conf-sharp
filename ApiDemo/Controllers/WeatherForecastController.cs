using Microsoft.AspNetCore.Mvc;
using xxl_conf_core;

namespace ApiDemo.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly XxlConfClient confClient;
        private readonly ConfigHander configHander;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
            XxlConfClient xxlConfClient,
            ConfigHander hander)
        {
            _logger = logger;
            confClient = xxlConfClient;
            configHander = hander;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> GetWeatherForecast()
        {
            //configHander.start();
            //var json = @"{""code"":200,""msg"":null,""data"":{""default.test"":""≤‚ ‘»•»•»•""}}";
            //var jsonObj = JsonConvert.DeserializeObject<ReturnT<Dictionary<string, string>>>(json);
            //var json = @"{'code':200,'msg':null,'data':'aaaa'}";
            //var jsonObj = JsonConvert.DeserializeObject<ReturnT<string>>(json);
            //var jsonStr= JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            //Console.WriteLine(jsonStr.Equals(json));
            //var data = confClient.get("default.test");
            // Console.WriteLine(data);
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet(Name = "GetConfig")]
        public string GetConfig(string key)
        {
            return confClient.get(key);

        }
    }
}