﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using log4net;
using LoadBalance.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoadBalance.NodeCheckers {
    /// <summary>
    ///     ref https://github.com/ethereum/wiki/wiki/JSON-RPC
    /// </summary>
    public class EthServerChecker : INodeChecker {
        private static readonly ILog Logger = LogManager.GetLogger(Log4NetCore.CoreRepository, typeof(EthServerChecker));

        private readonly MemoryStream _memoryStream = new MemoryStream();
        private BinaryWriter _binaryWriter;
        private int _chainId;
        private ChainType _chainType = ChainType.None;
        private long _curBlockNumber;
        private bool _isClosed;
        private bool _isOnline = true;

        /// <summary>
        ///     count error ,each success reset
        /// </summary>
        private int _lastErrorCount;

        private long _lastestBlockTime;
        private int _peerCount;
        private ServerDefine _serverDefine;
        private readonly System.Timers.Timer _timer;

        private ClientWebSocket _ws;


        private readonly Dictionary<int, string> _wsSubScriptionId = new Dictionary<int, string>();

        public EthServerChecker() {
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += OnTimerOnElapsed;
        }


        public void SetConfig(int chainid, ChainType chainType, ServerDefine server) {
            _serverDefine = server;
            _chainId = chainid;
            _chainType = chainType;

            if (!string.IsNullOrEmpty(server.Ws)) {
                _binaryWriter = new BinaryWriter(_memoryStream);
                
                Task.Run(WsRun);
            }
        }

        public ServerDefine GetConfig() {
            return _serverDefine;
        }

        public int GetChainId() {
            return _chainId;
        }

        public ChainType GetChainType() {
            return _chainType;
        }

        public long GetBlockNumber() {
            return _curBlockNumber;
        }

        public int GetPeersCount() {
            return _peerCount;
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
                            JsonConvert.SerializeObject(
                                new JsonRpcReq {
                                    Method = "eth_getTransactionCount",
                                    Params = new object[] {address, "pending"}
                                }),
                            Encoding.UTF8,
                            "application/json")).Result;

                    if (!resp.IsSuccessStatusCode)
                        return -1;

                    var jobj = JObject.Parse(resp.Content.ReadAsStringAsync().Result);

                    return (int) BigInteger.Parse(jobj.Value<string>("result").Replace("0x", ""), NumberStyles.HexNumber);
                }
            } catch (Exception ex) {
                Logger.Error(ex.Message);
            }

            return -1;
        }

        public System.Timers.Timer GetTimer() {
            return _timer;
        }

        public void Start() {
            _timer.Start();
        }

        public void Destory() {
            _timer?.Stop();
            _timer?.Dispose();

            _isClosed = true;
            if (_ws != null) {
                if (_ws.State != WebSocketState.Closed && _ws.State != WebSocketState.None)
                    _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);

                _ws.Dispose();
                _binaryWriter.Dispose();
            }
        }

        public bool HasTransaction(string txid) {
            try {
                using (var client = new HttpClient()) {
                    var resp = client.PostAsync(
                        new Uri(_serverDefine.Rpc),
                        new StringContent(
                            JsonConvert.SerializeObject(
                                new JsonRpcReq {
                                    Method = "eth_getTransactionByHash",
                                    Params = new object[] {txid}
                                }),
                            Encoding.UTF8,
                            "application/json")).Result;

                    if (!resp.IsSuccessStatusCode)
                        return false;

                    var jobj = JObject.Parse(resp.Content.ReadAsStringAsync().Result);

                    if (jobj["result"] == null)
                        return false;

                    return true;
                }
            } catch (Exception ex) {
                Logger.Error(ex);
            }

            return false;
        }

        public bool IsOnline() {
            return _isOnline;
        }

        public void Stop() {
            _timer.Stop();
            _timer.Dispose();
        }

        public void PrintSelfInfo() {
            
            Logger.Info($"cid:[{_chainId}] rpc: {_serverDefine.Rpc} online :{_isOnline} bn: {_curBlockNumber} pc: {_peerCount} bt: {_lastestBlockTime}");


        }

        /// <summary>
        ///     in time tick check follow list
        ///     1. block chain number height
        ///     2. syning status
        ///     3.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTimerOnElapsed(object sender, ElapsedEventArgs args) {
            _timer.Stop();

            try {
                var number = UpdateEthBlockNumber();

                if (number == -1) {
                    CheckEnd();
                    return;
                }

                _curBlockNumber = number;

                var blocktime = UpdateLastBlockTime();
                if (blocktime == -1) {
                    CheckEnd();
                    return;
                }

                _lastestBlockTime = blocktime;

                _peerCount = UpdatePeersCount();
            } catch (Exception ex) {
                Logger.Error(ex);
            }

            CheckEnd();
        }

        private int UpdatePeersCount() {
            if (_serverDefine.RpcApi.Contains("net")) {
                try {
                    using (var client = new HttpClient()) {
                        var resp = client.PostAsync(
                            new Uri(_serverDefine.Rpc),
                            new StringContent(
                                JsonConvert.SerializeObject(
                                    new JsonRpcReq {
                                        Method = "net_peerCount"
                                    }),
                                Encoding.UTF8,
                                "application/json")).Result;

                        if (!resp.IsSuccessStatusCode)
                            return -1;

                        var jobj = JObject.Parse(resp.Content.ReadAsStringAsync().Result);
                        _peerCount = (int) BigInteger.Parse(jobj.Value<string>("result").Replace("0x", ""), NumberStyles.HexNumber);

                        ResetErrorCount();

                        return _peerCount;
                    }
                } catch (Exception ex) {
                    Logger.Error(ex.Message);
                    AddErrorCount();
                }
            }
            
            if (!_serverDefine.RpcApi.Contains("admin"))
                return -1;

            try {
                using (var client = new HttpClient()) {
                    var resp = client.PostAsync(
                        new Uri(_serverDefine.Rpc),
                        new StringContent(
                            JsonConvert.SerializeObject(
                                new JsonRpcReq {
                                    Method = "admin_peers"
                                }),
                            Encoding.UTF8,
                            "application/json")).Result;

                    if (!resp.IsSuccessStatusCode)
                        return -1;

                    var jobj = JArray.Parse(resp.Content.ReadAsStringAsync().Result);

                    ResetErrorCount();

                    _peerCount = jobj.Count;

                    return _peerCount;
                }
            } catch (Exception ex) {
                Logger.Error(ex.Message);
                AddErrorCount();
            }

            return 0;
        }

        private void CheckEnd() {
            _timer.Start();
        }

        private long UpdateEthBlockNumber() {
            try {
                using (var client = new HttpClient()) {
                    var resp = client.PostAsync(
                        new Uri(_serverDefine.Rpc),
                        new StringContent(
                            JsonConvert.SerializeObject(
                                new JsonRpcReq {
                                    Method = "eth_blockNumber"
                                }),
                            Encoding.UTF8,
                            "application/json")).Result;

                    if (!resp.IsSuccessStatusCode)
                        return -1;

                    var jobj = JObject.Parse(resp.Content.ReadAsStringAsync().Result);

                    var ethBlockNumber = (long) BigInteger.Parse(jobj.Value<string>("result").Replace("0x", ""), NumberStyles.HexNumber);

                    ResetErrorCount();

                    return ethBlockNumber;
                }
            } catch (Exception ex) {
                Logger.Error(ex.Message);
                AddErrorCount();
            }

            return -1;
        }

        private long UpdateLastBlockTime() {
            try {
                using (var client = new HttpClient()) {
                    var resp = client.PostAsync(
                        new Uri(_serverDefine.Rpc),
                        new StringContent(
                            JsonConvert.SerializeObject(
                                new JsonRpcReq {
                                    Method = "eth_getBlockByNumber",
                                    Params = new object[] {"latest", false}
                                }),
                            Encoding.UTF8,
                            "application/json")).Result;

                    if (!resp.IsSuccessStatusCode)
                        return -1;

                    var jobj = JObject.Parse(resp.Content.ReadAsStringAsync().Result);

                    var lastBlockTime = (long) BigInteger.Parse(jobj["result"].Value<string>("timestamp").Replace("0x", ""), NumberStyles.HexNumber);

                    ResetErrorCount();

                    return lastBlockTime;
                }
            } catch (Exception ex) {
                Logger.Error(ex.Message);
                AddErrorCount();
            }

            return -1;
        }

        private void AddErrorCount() {
            _lastErrorCount++;

            if (_lastErrorCount >= Startup.GetConfig<int>("NodeCheck.ErrorThreshold")) {
                _isOnline = false;

                Logger.Info($"cid:[{_chainId}] rpc:[{_serverDefine.Rpc}] offline");
            }
        }

        private void ResetErrorCount() {
            if (!_isOnline)
                Logger.Info($"cid:[{_chainId}] rpc:[{_serverDefine.Rpc}] online");

            _isOnline = true;

            _lastErrorCount = 0;
        }


        #region Web socket

        private async Task WsRun() {
            while (!_isClosed) {
                await WsConnect();

                await WsReceive();
            }
        }

        private async Task WsConnect() {
            try {
                _ws?.Dispose();
                _ws = null;

                _ws = new ClientWebSocket();
                

                if (_ws.State != WebSocketState.Closed && _ws.State != WebSocketState.None)
                    await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);

                await _ws.ConnectAsync(new Uri(_serverDefine.Ws), CancellationToken.None);

                await WsSendSub();
            } catch (Exception ex) {
                Logger.Error(ex.Message);
            }
        }

        /// <summary>
        ///     https://github.com/ethereum/go-ethereum/wiki/RPC-PUB-SUB
        /// </summary>
        /// <returns></returns>
        private async Task WsSendSub() {
            var buffer = Encoding.UTF8.GetBytes("{\"id\": 1, \"method\": \"eth_subscribe\", \"params\": [\"newHeads\"]}");
            await _ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            buffer = Encoding.UTF8.GetBytes("{\"id\": 2, \"method\": \"eth_subscribe\", \"params\": [\"syncing\"]}");
            await _ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task WsReceive() {
            try {
                _binaryWriter.Seek(0, SeekOrigin.Begin);

                var buffer = new byte[2048];
                while (_ws.State == WebSocketState.Open) {
                    if (_isClosed)
                        return;

                    var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    switch (result.MessageType) {
                        case WebSocketMessageType.Text: {
                            _binaryWriter.Write(buffer, 0, result.Count);

                            if (!result.EndOfMessage)
                                continue;

                            var msg = Encoding.UTF8.GetString(_memoryStream.ToArray());
                            WsProcessMsg(msg);

                            _memoryStream.SetLength(0);
                            _binaryWriter.Seek(0, SeekOrigin.Begin);
                        }

                            break;
                        case WebSocketMessageType.Binary:
                            break;
                        case WebSocketMessageType.Close:

                            await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                            break;
                    }
                }
            } catch (Exception ex) {
                Logger.Error(ex);
            }
        }

        private void WsProcessMsg(string msg) {
            Logger.Debug(msg);

            try {
                var jobj = JObject.Parse(msg);

                switch (jobj.Value<int>("id")) {
                    case 1:
                        _wsSubScriptionId[1] = jobj.Value<string>("result");
                        break;
                    case 2:
                        _wsSubScriptionId[2] = jobj.Value<string>("result");
                        break;
                }


                if (jobj.Value<string>("method") == "eth_subscription") {
                    if (_wsSubScriptionId.ContainsKey(1) && jobj["params"]?.Value<string>("subscription") == _wsSubScriptionId[1]) {
                        //newHeads
                        //{
                        // "jsonrpc": "2.0",
                        // "method": "eth_subscription",
                        // "params": {
                        // "result": {
                        // "difficulty": "0x15d9223a23aa",
                        // "extraData": "0xd983010305844765746887676f312e342e328777696e646f7773",
                        // "gasLimit": "0x47e7c4",
                        // "gasUsed": "0x38658",
                        // "logsBloom": "0x00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",
                        // "miner": "0xf8b483dba2c3b7176a3da549ad41a48bb3121069",
                        // "nonce": "0x084149998194cc5f",
                        // "number": "0x1348c9",
                        // "parentHash": "0x7736fab79e05dc611604d22470dadad26f56fe494421b5b333de816ce1f25701",
                        // "receiptRoot": "0x2fab35823ad00c7bb388595cb46652fe7886e00660a01e867824d3dceb1c8d36",
                        // "sha3Uncles": "0x1dcc4de8dec75d7aab85b567b6ccd41ad312451b948a7413f0a142fd40d49347",
                        // "stateRoot": "0xb3346685172db67de536d8765c43c31009d0eb3bd9c501c9be3229203f15f378",
                        // "timestamp": "0x56ffeff8",
                        // "transactionsRoot": "0x0167ffa60e3ebc0b080cdb95f7c0087dd6c0e61413140e39d94d3468d7c9689f"
                        // },
                        // "subscription": "0x9ce59a13059e417087c02d3236a0b1cc"
                        // }
                        // }

                        var number = BigInteger.Parse(jobj["params"]["result"].Value<string>("number").Replace("0x", ""), NumberStyles.HexNumber);

                        WsOnNewBlockNumber(number);
                    }

                    if (_wsSubScriptionId.ContainsKey(2) && jobj["params"]?.Value<string>("subscription") == _wsSubScriptionId[2]) {
                        //syncing
                        //{"subscription":"0xe2ffeb2703bcf602d42922385829ce96","result":{"syncing":true,"status":{"startingBlock":674427,"currentBlock":67400,"highestBlock":674432,"pulledStates":0,"knownStates":0}}}}

                        dynamic result = jobj["params"]["result"];

                        if (result is JObject) {
                            bool syncing = result.syncing;
                            int currentBlock = result.status.CurrentBlock;
                            int highestBlock = result.status.HighestBlock;

                            WsOnSyncing(syncing, currentBlock, highestBlock);
                        }
                    }
                }
            } catch (Exception ex) {
                Logger.Error(ex);
            }
        }

        private void WsOnSyncing(bool syncing, int currentBlock, int highestBlock) {
            Logger.Info($"on syncing {syncing} cur {currentBlock}  highest {highestBlock}");
        }

        private void WsOnNewBlockNumber(BigInteger number) {
            if (number > _curBlockNumber)
                _curBlockNumber = (long) number;

            Logger.Info($"on new block {number} cur {_curBlockNumber}");
        }

        #endregion
    }
}