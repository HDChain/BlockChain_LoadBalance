using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoadBalance.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DefaultController : ControllerBase
    {
        [HttpGet]
        public string Test() {
            return "ok";
        }

        [HttpGet]
        public string GetPreferServer(int chainid,string address) {



            return string.Empty;
        }

    }
}