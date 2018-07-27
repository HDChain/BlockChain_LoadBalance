using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoadBalance.Models
{
    public class ChainConfigs
    {
        public ChainDefine[] Chains { get; set; }
    }


    public enum ChainType {
        Eth,

    }


    public class ChainDefine {
        public int ChainId { get; set; }

        public ChainType ChainType { get; set; }


        public ServerDefine[] Servers { get; set; }

    }

    public class ServerDefine {
        
        /// <summary>
        /// Rpc Uri
        ///
        /// Eg : http://127.0.0.1:8545
        /// 
        /// </summary>
        public string Rpc { get; set; }

        /// <summary>
        ///
        /// Web Socket Uri
        ///
        /// eg : ws://127.0.0.1:8546
        /// 
        /// </summary>
        public string Ws { get; set; }



        /// <summary>
        ///
        /// Chain node enabled rpc api
        ///
        /// eg :    eth
        ///         admin
        ///       
        /// 
        /// </summary>
        public string[] RpcApi { get; set; }



    }

}
