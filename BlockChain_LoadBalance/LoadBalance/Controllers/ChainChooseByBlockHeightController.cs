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
    public class ChainChooseByBlockHeightController : AChainChooseController
    {
        [HttpPost]
        [HttpGet]
        public override IActionResult GetPreferServer(int chainid, string address) {
            
            return new OkResult();
        }
    }
}