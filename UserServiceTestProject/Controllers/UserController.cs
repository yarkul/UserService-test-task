using Microsoft.AspNetCore.Mvc;

namespace UserServiceTestProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "User")]
        public IEnumerable<WeatherForecast> Get()
        {
            return default;
        }
    }
}
