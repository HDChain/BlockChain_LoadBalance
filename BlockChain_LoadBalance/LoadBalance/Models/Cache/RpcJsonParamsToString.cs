using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoadBalance.Models.Cache
{
    public class RpcJsonParamsToString 
    {
        public static string EmptyParams(JsonRpcClientReq req) {
            return string.Empty;
        }

        public static string StringSplitParam(JsonRpcClientReq req) {
            return string.Join("|",req.Params);
        }

    }
}
