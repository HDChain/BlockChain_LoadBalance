using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using log4net;
using LoadBalance.Auth;
using LoadBalance.Models;
using LoadBalance.Models.Cache;
using LoadBalance.Models.Config;
using LoadBalance.NodeChoose;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nethereum.Signer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        [RpcAuth]
        [HttpGet]
        public IActionResult Get() {
            return Ok("ok");
        }

        [RpcAuth]
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
                    if (sendResult.result) {
                        return new ContentResult() {
                            Content = sendResult.response,
                            ContentType = "application/json",
                            StatusCode = 200
                        };
                    }
                    return BadRequest(sendResult.response);
                }
                default: {
                    var cache = EthRpcCache.Instance.CheckCache(chainid, req);
                    if (cache.hasCache) {

                        Logger.Debug($"{req} in cache");
                        
                        //update resp id
                        var respObj = JObject.Parse(cache.result);
                        respObj["id"] = req.Id;
                        respObj["cache"] = cache.strategy.ToString();
                            
                        return new ContentResult() {
                            Content = respObj.ToString(Formatting.None),
                            ContentType = "application/json",
                            StatusCode = 200
                        };
                    }

                    var rpc = NodeChoose.ChooseServer(chainid);
                    if (string.IsNullOrEmpty(rpc)) {
                        return BadRequest("not find suitable rpc server");
                    }

                    var sendResult = await SendRequest(req, rpc);
                    if (sendResult.result) {
                        var respObj = JObject.Parse(sendResult.response);

                        switch (cache.strategy) {
                            case RpcCacheStrategy.NotCache:
                                break;
                            case RpcCacheStrategy.CacheInMemory:
                            case RpcCacheStrategy.CacheInDb:
                                var cacheResult = EthRpcCache.Instance.AddCache(chainid,req,sendResult.response);
                                respObj["cacheOption"] = cache.strategy.ToString();
                                respObj["cacheResult"] = cacheResult;
                                break;
                        }
                        return new ContentResult() {
                            Content = respObj.ToString(Formatting.None),
                            ContentType = "application/json",
                            StatusCode = 200
                        };
                    }

                    return BadRequest(sendResult.response);


                }
            }
        }

        private async Task<(bool result,string response)> SendRequest(JsonRpcClientReq req, string rpc) {
            try {
                using (var client = new HttpClient()) {
                    var resp = await client.PostAsync(rpc,
                        new StringContent(
                            Newtonsoft.Json.JsonConvert.SerializeObject(req),
                            Encoding.UTF8,
                            "application/json"));

                    if (!resp.IsSuccessStatusCode) {
                        return (false,await resp.Content.ReadAsStringAsync());
                    }

                    return (true,await resp.Content.ReadAsStringAsync());
                }
            } catch (Exception ex) {
                Logger.Error(ex);
                return (false,ex.Message);
            }

            
        }

        static EthRpcController() {
            var chooseType = Startup.GetConfig<string>("EthNodeChoose.Choose");

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