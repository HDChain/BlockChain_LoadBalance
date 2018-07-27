using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoadBalance.Models;

namespace LoadBalance.NodeCheckers
{
    public class EthServerChecker : INodeChecker {
        private ServerDefine _serverDefine;
        public EthServerChecker(ServerDefine define) {
            _serverDefine = define;
        }
        

        public long GetBlockNumber() {
            throw new NotImplementedException();
        }

        public int GetPeersCount() {
            throw new NotImplementedException();
        }

        public long GetLastestBlockTimeTicker() {
            throw new NotImplementedException();
        }

        public int GetUserPendingTransactionCount(string address) {
            throw new NotImplementedException();
        }
    }
}
