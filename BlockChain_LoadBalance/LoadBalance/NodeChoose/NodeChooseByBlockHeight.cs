using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoadBalance.Redis;

namespace LoadBalance.NodeChoose
{
    public class NodeChooseByBlockHeight : INodeChoose
    {
        public string ChooseServer(int chainid, string address) {

            var cache = RedisHelper.Instance.GetUserRpcCache(chainid, address);
            if (!string.IsNullOrEmpty(cache)) {
                return cache;
            }
            
            var nodes = NodeChecker.Instance.FindCheckerByChainId(chainid);
            if (nodes.Count == 0) {
                return String.Empty;
            }

            var first = nodes.OrderByDescending(n => n.GetBlockNumber()).First();

            var rpc = first.GetConfig().Rpc;

            RedisHelper.Instance.AddUserRpcCache(chainid,address,rpc);

            return rpc;
        }
    }
}
