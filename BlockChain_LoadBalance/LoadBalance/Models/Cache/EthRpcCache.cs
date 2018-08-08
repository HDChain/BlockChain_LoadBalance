using System;
using LoadBalance.Db;
using LoadBalance.Models.Config;
using LoadBalance.Redis;

namespace LoadBalance.Models.Cache {
    public class EthRpcCache : Singleton<EthRpcCache> {
        public (bool hasCache, string result, RpcCacheStrategy strategy) CheckCache(int chainid, JsonRpcClientReq req) {
            if (!EthCacheConfig.Methods.ContainsKey(req.Method))
                return (false, string.Empty,RpcCacheStrategy.NotCache);

            var config = EthCacheConfig.Methods[req.Method];

            switch (config.RpcCacheStrategy) {
                case RpcCacheStrategy.NotCache:
                    return (false, string.Empty,config.RpcCacheStrategy);
                case RpcCacheStrategy.CacheInMemory: {
                    {
                        var checkCache = RedisHelper.Instance.GetRpcCache(chainid, req.Method, config.ParamDelegate?.Invoke(req));
                        return (checkCache.hasCache, checkCache.result, config.RpcCacheStrategy);
                    }
                }
                case RpcCacheStrategy.CacheInDb: {
                    var checkCache = Db.DbMgr.Instance.GetRpcCache(chainid, req.Method, config.ParamDelegate?.Invoke(req));
                    return (checkCache.hasCache, checkCache.result, config.RpcCacheStrategy);
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void AddCache(int chainid,JsonRpcClientReq req, string result) {
            if (!EthCacheConfig.Methods.ContainsKey(req.Method))
                return;

            var config = EthCacheConfig.Methods[req.Method];
            switch (config.RpcCacheStrategy) {
                case RpcCacheStrategy.NotCache:
                    break;
                case RpcCacheStrategy.CacheInMemory:
                    RedisHelper.Instance.AddRpcCache(chainid,req.Method,config.ParamDelegate?.Invoke(req),result,config.CacheOption > 0 ? TimeSpan.FromSeconds(config.CacheOption): TimeSpan.Zero);
                    break;
                case RpcCacheStrategy.CacheInDb:
                    DbMgr.Instance.AddRpcCache(chainid,req.Method,config.ParamDelegate?.Invoke(req),result,config.CacheOption > 0 ? TimeSpan.FromSeconds(config.CacheOption): TimeSpan.Zero);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}