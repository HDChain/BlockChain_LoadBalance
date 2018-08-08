using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Microsoft.Extensions.Configuration;

namespace LoadBalance.Db
{
    public class DbMgr : Singleton<DbMgr> {
        private IRDb Db = null;

        private const string DefaultChainDbName = "ChainMain";
        private static readonly ILog Logger = LogManager.GetLogger(Log4NetCore.CoreRepository, typeof(DbMgr));

        

        public void Init() {
            switch (Startup.GetConfig<string>("Db.Type")) {
                case "mysql":
                    Db = new MySqlDb();

                    while (!Db.CheckDbReady()) {
                        Logger.Info("my sql is not ready");                

                        System.Threading.Thread.Sleep(10000);
                    }

                    Logger.Info("my sql is ready");

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
