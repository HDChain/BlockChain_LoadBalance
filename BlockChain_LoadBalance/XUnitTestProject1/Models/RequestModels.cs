using Newtonsoft.Json;

namespace XUnitTestProject1.Models {

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

    
    
}