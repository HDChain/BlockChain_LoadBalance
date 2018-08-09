using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoadBalance.Models.Cache;

namespace LoadBalance.Models.Config
{
    public class EthCacheConfig
    {
        public static Dictionary<string,EthRpcMethod> Methods = new Dictionary<string, EthRpcMethod>() {
            //default option
            {"",new EthRpcMethod("",RpcMethodType.Read,RpcCacheStrategy.NotCache)},

            //web3
            #region Web3
            {"web3_clientVersion",new EthRpcMethod("web3_clientVersion",RpcMethodType.Read,RpcCacheStrategy.CacheInMemory,15)},
            {"web3_sha3",new EthRpcMethod("web3_sha3",RpcMethodType.Read,RpcCacheStrategy.NotCache)},

            #endregion

            //net

            #region net

            {"net_version",new EthRpcMethod("net_version",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"net_listening",new EthRpcMethod("net_listening",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"net_peerCount",new EthRpcMethod("net_peerCount",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            
            #endregion

            //eth

            #region eth

            {"eth_protocolVersion",new EthRpcMethod("eth_protocolVersion",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_syncing",new EthRpcMethod("eth_syncing",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_coinbase",new EthRpcMethod("eth_coinbase",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_mining",new EthRpcMethod("eth_mining",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_hashrate",new EthRpcMethod("eth_hashrate",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_gasPrice",new EthRpcMethod("eth_gasPrice",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_accounts",new EthRpcMethod("eth_accounts",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_blockNumber",new EthRpcMethod("eth_blockNumber",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_getBalance",new EthRpcMethod("eth_getBalance",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_getStorageAt",new EthRpcMethod("eth_getStorageAt ",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_getTransactionCount",new EthRpcMethod("eth_getTransactionCount",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_getBlockTransactionCountByHash",new EthRpcMethod("eth_getBlockTransactionCountByHash",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_getBlockTransactionCountByNumber",new EthRpcMethod("eth_getBlockTransactionCountByNumber",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_getUncleCountByBlockHash",new EthRpcMethod("eth_getUncleCountByBlockHash",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_getUncleCountByBlockNumber",new EthRpcMethod("eth_getUncleCountByBlockNumber",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_getCode",new EthRpcMethod("eth_getCode",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_sign",new EthRpcMethod("eth_sign",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_sendTransaction",new EthRpcMethod("eth_sendTransaction",RpcMethodType.Write,RpcCacheStrategy.NotCache)},
            {"eth_sendRawTransaction",new EthRpcMethod("eth_sendRawTransaction",RpcMethodType.Write,RpcCacheStrategy.NotCache)},
            {"eth_call",new EthRpcMethod("eth_call",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_estimateGas",new EthRpcMethod("eth_estimateGas",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_getBlockByHash",new EthRpcMethod("eth_getBlockByHash",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_getBlockByNumber",new EthRpcMethod("eth_getBlockByNumber",RpcMethodType.Read,RpcCacheStrategy.CacheInDb,0,RpcJsonParamsToString.StringSplitParam,RpcCacheFilter.EthCacheFilter)},
            {"eth_getTransactionByHash",new EthRpcMethod("eth_getTransactionByHash",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_getTransactionByBlockHashAndIndex",new EthRpcMethod("eth_getTransactionByBlockHashAndIndex",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_getTransactionByBlockNumberAndIndex",new EthRpcMethod("eth_getTransactionByBlockNumberAndIndex",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_getTransactionReceipt",new EthRpcMethod("eth_getTransactionReceipt",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_getUncleByBlockHashAndIndex",new EthRpcMethod("eth_getUncleByBlockHashAndIndex",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_getUncleByBlockNumberAndIndex",new EthRpcMethod("eth_getUncleByBlockNumberAndIndex",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_newFilter",new EthRpcMethod("eth_newFilter",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_newBlockFilter",new EthRpcMethod("eth_newBlockFilter",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_newPendingTransactionFilter",new EthRpcMethod("eth_newPendingTransactionFilter",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_uninstallFilter",new EthRpcMethod("eth_uninstallFilter",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_getFilterChanges",new EthRpcMethod("eth_getFilterChanges",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_getFilterLogs",new EthRpcMethod("eth_getFilterLogs",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_getLogs",new EthRpcMethod("eth_getLogs",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_getWork",new EthRpcMethod("eth_getWork",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_submitWork",new EthRpcMethod("eth_submitWork",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"eth_submitHashrate",new EthRpcMethod("eth_submitHashrate",RpcMethodType.Read,RpcCacheStrategy.NotCache)},




            #endregion

            //db

            #region db

            {"db_putString",new EthRpcMethod("db_putString",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"db_getString",new EthRpcMethod("db_getString",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"db_putHex",new EthRpcMethod("db_putHex",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"db_getHex",new EthRpcMethod("db_getHex",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            

            #endregion

            //shh

            #region shh

            {"shh_version",new EthRpcMethod("shh_version",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"shh_post",new EthRpcMethod("shh_post",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"shh_newIdentity",new EthRpcMethod("shh_newIdentity",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"shh_hasIdentity",new EthRpcMethod("shh_hasIdentity",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"shh_newGroup",new EthRpcMethod("shh_newGroup",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"shh_addToGroup",new EthRpcMethod("shh_addToGroup",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"shh_newFilter",new EthRpcMethod("shh_newFilter",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"shh_uninstallFilter",new EthRpcMethod("shh_uninstallFilter",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"shh_getFilterChanges",new EthRpcMethod("shh_getFilterChanges",RpcMethodType.Read,RpcCacheStrategy.NotCache)},
            {"shh_getMessages",new EthRpcMethod("shh_getMessages",RpcMethodType.Read,RpcCacheStrategy.NotCache)},

            #endregion

        };
        
    }


    public class EthRpcMethod {
        public delegate string ParamToString(JsonRpcClientReq req);

        public delegate bool CacheFilter(int chainid,JsonRpcClientReq req,string result);

        public EthRpcMethod(string method, RpcMethodType rpcMethodType, RpcCacheStrategy rpcCacheStrategy) {
            Method = method;
            RpcMethodType = rpcMethodType;
            RpcCacheStrategy = rpcCacheStrategy;
        }

        public EthRpcMethod(string method, RpcMethodType rpcMethodType, RpcCacheStrategy rpcCacheStrategy,int cacheOption , ParamToString paramDelegate = null,CacheFilter filter = null) {
            Method = method;
            RpcMethodType = rpcMethodType;
            RpcCacheStrategy = rpcCacheStrategy;
            CacheOption = cacheOption;
            ParamDelegate = paramDelegate;
            FilterDelegate = filter;
        }

        public string Method { get; set; }

        public RpcMethodType RpcMethodType { get; set; }

        public RpcCacheStrategy RpcCacheStrategy { get; set; }

        public int CacheOption { get; set; }

        public ParamToString ParamDelegate { get; set; }

        public CacheFilter FilterDelegate { get; set; }
    }

    

}
