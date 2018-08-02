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
using Microsoft.Extensions.Configuration;
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
        private static INodeChoose NodeChoose = null;

        [HttpGet]
        public IActionResult Get() {
            return new OkResult();
        }

        [HttpPost]
        public async Task<IActionResult> Post(JsonRpcClientReq req,int chainid = 0) {

            if (chainid == 0) {
                return BadRequest("chainid error");
            }

            switch (req.Method) {
                case "eth_sendRawTransaction": {
                    var signer = new TransactionSigner();

                    var addr = signer.GetSenderAddress(req.Params[0].ToString());
                    var rpc = NodeChoose.ChooseServer(chainid, addr);

                    if (string.IsNullOrEmpty(rpc)) {
                        return BadRequest("not find suitable rpc server");
                    }

                    var sendResult = await SendRequest(req, rpc);
                    return sendResult.response;
                }
                default: {
                    var rpc = NodeChoose.ChooseServer(chainid);
                    if (string.IsNullOrEmpty(rpc)) {
                        return BadRequest("not find suitable rpc server");
                    }

                    var sendResult = await SendRequest(req, rpc);
                    return sendResult.response;
                }
            }
        }

        private async Task<(bool result,IActionResult response)> SendRequest(JsonRpcClientReq req, string rpc) {
            try {
                using (var client = new HttpClient()) {
                    var resp = await client.PostAsync(rpc,
                        new StringContent(
                            Newtonsoft.Json.JsonConvert.SerializeObject(req),
                            Encoding.UTF8,
                            "application/json"));

                    if (!resp.IsSuccessStatusCode) {
                        return (false,BadRequest(await resp.Content.ReadAsStringAsync()));
                    }

                    return (true,new ContentResult() {
                        Content = await resp.Content.ReadAsStringAsync(),
                        ContentType = resp.Content.Headers?.ContentType?.MediaType,
                        StatusCode = 200
                    });
                }
            } catch (Exception ex) {
                Logger.Error(ex);
                return (false,BadRequest(ex.Message));
            }

            
        }

        static EthRpcController() {
            var chooseType = Startup.Configuration.GetValue<string>("EthNodeChoose:Choose");

            Logger.Info($"eth choose : {chooseType} ");

            switch (chooseType) {
                case "NodeChooseByBlockHeight":
                    NodeChoose = new NodeChooseByBlockHeight(); 
                    break;
                case "NodeChooseByRandom":
                    NodeChoose = new NodeChooseByRandom();
                    break;
            }
        }
    }
}