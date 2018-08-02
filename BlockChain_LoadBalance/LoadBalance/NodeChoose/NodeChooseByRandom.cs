using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoadBalance.NodeChoose
{
    public class NodeChooseByRandom : INodeChoose
    {
        private static Random Random = new Random();

        public string ChooseServer(int chainid, string address) {
            var nodes = NodeChecker.Instance.FindCheckerByChainId(chainid);
            if (nodes.Count == 0) {
                return String.Empty;
            }

            return nodes[Random.Next(0, nodes.Count)].GetConfig().Rpc;
        }

        public string ChooseServer(int chainid) {
            var nodes = NodeChecker.Instance.FindCheckerByChainId(chainid);
            if (nodes.Count == 0) {
                return String.Empty;
            }

            return nodes[Random.Next(0, nodes.Count)].GetConfig().Rpc;
        }
    }
}
