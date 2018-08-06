using System;
using log4net;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace LoadBalance.Redis {
    public class RedisHelper : Singleton<RedisHelper> {
        public static ConnectionMultiplexer RedisEventClient;
        private static readonly ILog Logger = LogManager.GetLogger(Log4NetCore.CoreRepository, typeof(RedisHelper));
        private const string RedisUserRpcCacheKey = "Ld:UserRpc:chain{0}:{1}";
        private static int RedisCacheTimeOutInSecond = 15;

        public void Init() {
            try {
                RedisEventClient = ConnectionMultiplexer.Connect(Startup.Configuration.GetValue<string>("RedisConn"));
                RedisCacheTimeOutInSecond = Startup.Configuration.GetValue<int>("RedisCacheTimeOutInSecond");
            } catch (Exception ex) {
                Logger.Error(ex);
            }
        }

        public static IDatabase GetDatabase() {
            return RedisEventClient.GetDatabase();
        }

        public void AddUserRpcCache(int chainid, string address, string rpc) {
            AddUserRpcCache(chainid,address,rpc,RedisCacheTimeOutInSecond);
        }

        public void AddUserRpcCache(int chainid,string address,string rpc,int cacheTimeInSecond) {
            try {
                var db = GetDatabase();
                db.StringSet(string.Format(RedisUserRpcCacheKey,chainid, address), rpc, TimeSpan.FromSeconds(cacheTimeInSecond));
            } catch (Exception ex) {
                Logger.Error(ex);
            }
        }

        public string GetUserRpcCache(int chainid,string address) {
            try {
                var db = GetDatabase();

                var rpc = db.StringGet(string.Format(RedisUserRpcCacheKey,chainid, address));

                if (rpc.HasValue) {
                    return rpc;
                }

                return string.Empty;
            } catch (Exception ex) {
                Logger.Error(ex);
                return string.Empty;
            }
        }

        public void Destory() {
            RedisEventClient?.Dispose();
        }

        public (bool hasCache,string result) GetRpcCache(int chainid,string reqMethod, string rpcParams) {
            var key = string.IsNullOrEmpty(rpcParams) ? $"chain:{chainid}:{reqMethod}" : $"chain:{chainid}:{reqMethod}:{rpcParams}";

            try {
                var db = GetDatabase();

                var val = db.StringGet(key);
                if (!val.HasValue) {
                    return (false, string.Empty);
                }

                return (true, val);

            } catch (Exception ex) {
                Logger.Error(ex);

                return (false, string.Empty);
            }
        }

        public void AddRpcCache(int chainid, string reqMethod, string rpcParams, string result,TimeSpan expiry) {
            var key = string.IsNullOrEmpty(rpcParams) ? $"chain:{chainid}:{reqMethod}" : $"chain:{chainid}:{reqMethod}:{rpcParams}";
            
            try {
                var db = GetDatabase();
                if (expiry != TimeSpan.Zero) {
                    db.StringSet(key, result, expiry);
                } else {
                    db.StringSet(key, result);
                }
                
            } catch (Exception ex) {
                Logger.Error(ex);
            }
        }
    }
}