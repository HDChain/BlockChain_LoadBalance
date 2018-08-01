using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using log4net;
using LoadBalance.Models;
using LoadBalance.NodeCheckers;
using Newtonsoft.Json.Linq;

namespace LoadBalance
{
    public class NodeChecker :Singleton<NodeChecker> {

        private List<INodeChecker>_checkers = new List<INodeChecker>();
        
        private static readonly ILog Logger = LogManager.GetLogger(Log4NetCore.CoreRepository, typeof(NodeChecker));

        public NodeChecker() {
           LoadConfig();
            
        }

        private void LoadConfig() {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chainconfig.json");

            if (!File.Exists(path)) {
                throw new ArgumentException($"config file : [{path}] not exists");
            }

            var json = JObject.Parse(File.ReadAllText(path)).ToObject<ChainConfigs>();

            foreach (var chain in json.Chains) {
                foreach (var server in chain.Servers) {

                    switch (chain.ChainType) {
                        case ChainType.Eth:

                            INodeChecker checker = new EthServerChecker();
                            checker.SetConfig(chain.ChainId,chain.ChainType,server);

                            _checkers.Add(checker);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }



                }
            }
        }


        

        public void Start() {
            foreach (var nodeChecker in _checkers) {
                nodeChecker.Start();
            }

        }

        public List<INodeChecker> FindCheckerByChainId(int chainid) {
            List<INodeChecker> node = new List<INodeChecker>();

            foreach (var nodeChecker in _checkers) {
                if (nodeChecker.GetChainId() == chainid) {
                    node.Add(nodeChecker);
                }
            }
            
            return node;
        }

        public List<INodeChecker> FindCheckerByChainType(ChainType chainType) {
            List<INodeChecker> node = new List<INodeChecker>();

            foreach (var nodeChecker in _checkers) {
                if (nodeChecker.GetChainType() == chainType) {
                    node.Add(nodeChecker);
                }
            }
            
            return node;
        }
    }
}
