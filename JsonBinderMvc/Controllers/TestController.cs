using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonBinder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JsonBinderMvc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        public TestController()
        {
        }

        [HttpPost("two")]
        [JsonParameters]
        public async Task<IActionResult> TwoParameters(string a, dynamic b)
        {
            Console.WriteLine(b.c);
            return Ok();
        }
    }
}