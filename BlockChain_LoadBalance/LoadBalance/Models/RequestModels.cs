using System;
using Newtonsoft.Json;

namespace LoadBalance.Models {

    public class JsonRpcReq {

        private static int _id;
        private static int GetId() {
            return _id++;
        }

        [JsonProperty("jsonrpc")]
        public string JsonRpc => "2.0";

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("params")]
        public object[] Params {
            get;
            set;
        }

        [JsonProperty("id")]
        public int Id => GetId();
    }

    public class JsonRpcClientReq {

        [JsonProperty("jsonrpc")]
        public string JsonRpc => "2.0";

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("params")]
        public object[] Params {
            get;
            set;
        }

        [JsonProperty("id")]
        public int Id { get; set; }
    }
     
    /// <summary>
    ///     请求基类
    /// </summary>
    public class BaseReq {
    }

    /// <summary>
    ///     响应基础类
    /// </summary>
    public class BaseResp {
        public bool Result { get; set; } = true;

        public string Error { get; set; } = string.Empty;
    }
    
}