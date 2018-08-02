using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoadBalance.Models.Config
{
    public enum RpcMethodType
    {
        /// <summary>
        /// 
        /// </summary>
        Read,
        /// <summary>
        /// 
        /// </summary>
        Write,

    }

    public enum RpcCacheStrategy {

        NotCache,
        CacheInMemory,
        CacheInDb,
    }
}
