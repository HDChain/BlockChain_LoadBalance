using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using log4net;
using LoadBalance.Models;
using LoadBalance.NodeChoose;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nethereum.Signer;

namespace LoadBalance.Controllers
{
    /// <summary>
    ///
    /// web api controllor for process eth rpc reverse proxy 
    /// 
    /// eth can config rpc url to this for load balance 
    /// 
    /// </summary>
    
    [Route("ethrpc/")]
    [Route("ethrpc/{chainid}")]
    [ApiController]
    public class EthRpcController : ControllerBase
    {
        private static readonly ILog Logger = LogManager.GetLogger(Log4NetCore.CoreRepository, typeof(EthRpcController));
        private static INodeChoose NodeChoose = new NodeChooseByBlockHeight(); 

        [HttpGet]
        public IActionResult Get() {
            
            return new OkResult();
        }

        [HttpPost]
        public async Task<IActionResult> Post(JsonRpcClientReq req,int chainid = 0) {

            switch (req.Method) {
                case "eth_sendRawTransaction":
                    var signer = new TransactionSigner();

                    var addr = signer.GetSenderAddress(req.Params[0].ToString());
                    NodeChoose.ChooseServer(chainid, addr);
                    break;
                default:

                    var rpc = NodeChoose.ChooseServer(chainid);
                    if (string.IsNullOrEmpty(rpc)) {
                        return BadRequest("chainid error");
                    }

                    try {
                        using (var client = new HttpClient()) {
                            var resp = await client.PostAsync(rpc, 
                                new StringContent(
                                    Newtonsoft.Json.JsonConvert.SerializeObject(req), 
                                    Encoding.UTF8, 
                                    "application/json"));

                            if (!resp.IsSuccessStatusCode) {
                                return BadRequest(await resp.Content.ReadAsStringAsync());
                            }

                            return new ContentResult() {
                                Content = await resp.Content.ReadAsStringAsync(),
                                ContentType = resp.Content.Headers?.ContentType?.MediaType,
                                StatusCode = 200
                            };
                        }

                    } catch (Exception ex) {
                        Logger.Error(ex);
                    }

                    break;
            }

            return new OkResult();
        }

    }
}