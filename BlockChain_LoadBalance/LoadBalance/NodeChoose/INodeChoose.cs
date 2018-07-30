using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoadBalance.NodeChoose
{
    public interface INodeChoose {
        string ChooseServer(int chainid, string address);
    }
}
