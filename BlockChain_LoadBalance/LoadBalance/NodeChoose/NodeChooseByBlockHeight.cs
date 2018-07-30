using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoadBalance.NodeChoose
{
    public class NodeChooseByBlockHeight : INodeChoose
    {
        public string ChooseServer(int chainid, string address) {
            var nodes = NodeChecker.Instance.FindCheckerByChainId(chainid);
            if (nodes.Count == 0) {
                return String.Empty;
            }

            var first = nodes.OrderByDescending(n => n.GetBlockNumber()).First();

            return first.GetConfig().Rpc;
        }
    }
}
