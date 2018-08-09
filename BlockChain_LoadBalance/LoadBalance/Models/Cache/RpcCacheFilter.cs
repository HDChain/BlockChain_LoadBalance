using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using LoadBalance.NodeCheckers;

namespace LoadBalance.Models.Cache
{
    public class RpcCacheFilter
    {
        public static bool EthCacheFilter(int chainid,JsonRpcClientReq req,string result) {

            switch (req.Method) {
                case "eth_getBlockByNumber":
                    return Check_eth_getBlockByNumber(chainid,req);




            }

            return false;
        }

        
        /// <summary>
        /// check cur block number should be cache ?
        /// if block is new just by pass
        /// </summary>
        /// <param name="chainid"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        private static bool Check_eth_getBlockByNumber(int chainid,JsonRpcClientReq req) {
            if (req.Params.Length != 2) {
                return false;
            }

            if (!BigInteger.TryParse(req.Params[0].ToString().Replace("0x", ""), out BigInteger result)) {
                return false;
            }

            var nodes = NodeChecker.Instance.FindCheckerByChainId(chainid);
            if (nodes.Count == 0) {
                return false;
            }

            var first = nodes.OrderByDescending(n => n.GetBlockNumber()).First();

            var curBlockCount = first.GetBlockNumber();
            var i = Startup.GetConfig<int>("Cache.Eth_IrrevocableBlockCount");
            if (result + i < curBlockCount) {
                return true;
            }

            return false;
        }
    }
}
