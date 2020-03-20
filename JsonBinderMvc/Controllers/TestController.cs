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
        
        [HttpPut("one")]
                 [JsonParameters]
                 public async Task<IActionResult> One(string a)
                 {
                     Console.WriteLine(a);
                     return Ok();
                 }

        [HttpPost("three")]
        public async Task<IActionResult> Three([FromBody] ModelA a)
        {
            Console.WriteLine(a.B);
            return Ok();
        }
        
        [HttpPost("four")]
        [JsonParameters]
        public async Task<IActionResult> Four(int a)
        {
            Console.WriteLine(a);
            return Ok();
        }
        
        [HttpPost("five")]
        [JsonParameters]
        public async Task<IActionResult> Five(List<int> a)
        {
            Console.WriteLine(a[0]);
            return Ok();
        }

        [HttpPost("six")]
        [JsonParameters]
        public async Task<IActionResult> Six(string[] a)
        {
            Console.WriteLine(a[0]);
            return Ok();
        }
    }

    public class ModelA
    {
        public string A { get; set; }
        public string B { get; set; }
    }
}
