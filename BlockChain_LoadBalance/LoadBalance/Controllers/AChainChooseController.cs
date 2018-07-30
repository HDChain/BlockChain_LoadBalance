using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoadBalance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class AChainChooseController : ControllerBase {

        [HttpPost]
        [HttpGet]
        public abstract IActionResult GetPreferServer(int chainid, string address);

    }
}