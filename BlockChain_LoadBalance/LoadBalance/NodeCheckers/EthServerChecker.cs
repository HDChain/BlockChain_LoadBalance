using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using log4net;
using LoadBalance.Models;
using Newtonsoft.Json.Linq;

namespace LoadBalance.NodeCheckers
{
    /// <summary>
    /// ref https://github.com/ethereum/wiki/wiki/JSON-RPC
    ///
    /// 
    /// </summary>
    public class EthServerChecker : INodeChecker {
        private static readonly ILog Logger = LogManager.GetLogger(Log4NetCore.CoreRepository, typeof(EthServerChecker));
        private ServerDefine _serverDefine;
        private Timer _timer;
        private long _curBlockNumber = 0;
        private long _lastestBlockTime = 0;
        private int _peerCount = 0;
        private int _chainId = 0;

        public EthServerChecker() {
            _timer = new Timer(1000);
            _timer.Elapsed += OnTimerOnElapsed;
        }


        public void SetConfig(int chainid, ChainType chainType, ServerDefine server) {
            _serverDefine = server;
            _chainId = chainid;
        }

        public ServerDefine GetConfig() {
            return _serverDefine;
        }

        public int GetChainId() {
            return _chainId;
        }

        public long GetBlockNumber() {
            return _curBlockNumber;
        }

        public int GetPeersCount() {

            if (!_serverDefine.RpcApi.Contains("admin")) {
                return -1;
            }



            return 0;
        }

        public long GetLastestBlockTimeTicker() {
            return _lastestBlockTime;
        }

        public int GetUserPendingTransactionCount(string address) {
            try {
                using (var client = new HttpClient()) {
                    var resp = client.PostAsync(
                        new Uri(_serverDefine.Rpc), 
                        new StringContent(
                            Newtonsoft.Json.JsonConvert.SerializeObject(
                                new JsonRpcReq() {
                                    Method = "eth_getTransactionCount",
                                    Params = new object[] {address,"pending"}
                                }), Encoding.UTF8, "application/json")).Result;

                    if (!resp.IsSuccessStatusCode) {
                        return -1;
                    }

                    var jobj = JObject.Parse(resp.Content.ReadAsStringAsync().Result);

                    return (int) BigInteger.Parse(jobj.Value<string>("result").Replace("0x",""),NumberStyles.HexNumber);

                }
            } catch (Exception ex) {
                Logger.Error(ex);
            }

            return -1;
        }

        public Timer GetTimer() {
            return _timer;
        }

        public void Start() {
            _timer.Start();
        }

        public void Destory() {
            _timer?.Stop();
            _timer?.Dispose();
        }

        /// <summary>
        ///
        /// in time tick check follow list 
        /// 1. block chain number height 
        /// 2. syning status
        /// 3. 
        ///
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTimerOnElapsed(object sender, ElapsedEventArgs args) {
            _timer.Stop();
           
            try {

                var number = GetEthBlockNumber();

                if (number == -1) {
                    CheckEnd();
                    return;
                }

                _curBlockNumber = number;

                var blocktime = GetLastBlockTime();
                if (blocktime == -1) {
                    CheckEnd();
                    return;
                }

                _lastestBlockTime = blocktime;


            } catch (Exception ex) {
                Logger.Error(ex);
            }

            CheckEnd();
        }

        private void CheckEnd() {
            _timer.Start();
        }

        private long GetEthBlockNumber() {
            try {
                using (var client = new HttpClient()) {
                    var resp = client.PostAsync(
                        new Uri(_serverDefine.Rpc), 
                        new StringContent(
                            Newtonsoft.Json.JsonConvert.SerializeObject(
                                new JsonRpcReq() {
                                    Method = "eth_blockNumber"
                    }), Encoding.UTF8, "application/json")).Result;

                    if (!resp.IsSuccessStatusCode) {
                        return -1;
                    }

                    var jobj = JObject.Parse(resp.Content.ReadAsStringAsync().Result);

                    return (long) BigInteger.Parse(jobj.Value<string>("result").Replace("0x",""),NumberStyles.HexNumber);

                }
            } catch (Exception ex) {
                Logger.Error(ex);
            }

            return -1;
        }

        private long GetLastBlockTime() {
            try {
                using (var client = new HttpClient()) {
                    var resp = client.PostAsync(
                        new Uri(_serverDefine.Rpc), 
                        new StringContent(
                            Newtonsoft.Json.JsonConvert.SerializeObject(
                                new JsonRpcReq() {
                                    Method = "eth_getBlockByNumber",
                                    Params = new object[] {"latest",false}
                                }), Encoding.UTF8, "application/json")).Result;

                    if (!resp.IsSuccessStatusCode) {
                        return -1;
                    }

                    var jobj = JObject.Parse(resp.Content.ReadAsStringAsync().Result);

                    return (long) BigInteger.Parse(jobj["result"].Value<string>("timestamp").Replace("0x",""),NumberStyles.HexNumber);

                }
            } catch (Exception ex) {
                Logger.Error(ex);
            }

            return -1;
        }

    }
}
