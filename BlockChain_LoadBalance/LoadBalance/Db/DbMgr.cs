using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace LoadBalance.Db
{
    public class DbMgr : Singleton<DbMgr> {
        private IRDb Db = null;

        private const string DefaultChainDbName = "ChainMain";

        

        public void Init() {
            switch (Startup.GetConfig<string>("Db.Type")) {
                case "mysql":
                    Db = new MySqlDb();
                    Db.CheckAndCreateDatabase(DefaultChainDbName);
                    break;
                default:
                    throw new ArgumentException("db type error");
            }

            
        }

        /// <summary>
        /// each chain has a table 
        /// </summary>
        public void CreateChainDb(int chainid) {
            Db.CheckAndCreateDatabase($"Chain_{chainid}");
            Db.CreateRpcCacheTable(chainid);
            Db.CreateBlockTable(chainid);
        }

        /// <summary>
        /// 
        /// </summary>
        public void CreateChainBlockTable() {

        }

        public (bool hasCache, string result) GetRpcCache(int chainid, string reqMethod, string rpcParams) {
            return Db.GetRpcCache(chainid, reqMethod, rpcParams);
        }

        public void AddRpcCache(int chainid, string reqMethod, string reqParams, string result, TimeSpan timeSpan) {
            Db.AddRpcCache(chainid,reqMethod,reqParams,result,timeSpan);
        }
    }
}
