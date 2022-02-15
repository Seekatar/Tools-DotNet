using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebSettings5Api.Controllers
{
    [ApiController]
    [Route("config")]
    public class ConfigController : ControllerBase
    {
        private readonly ILogger<ConfigController> _logger;
        private readonly IConfiguration _config;

        public ConfigController(ILogger<ConfigController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet("{value}")]
        public string Get(string value)
        {
            return _config[value] ?? "";
        }
    }
}
