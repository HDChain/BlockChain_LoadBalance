using System;
using System.Collections.Generic;

namespace LoadBalance.Db {
    public interface IRDb {
        string[] GetDataBaseList();

        bool CheckDbReady();

        bool CreateDatabase(string db);
        bool CheckAndCreateDatabase(string db);

        int ExecSql(int chainid, string sql, object sqlparams);

        List<T> Query<T>(int chainid, string sql, object sqlparams);

        void CreateRpcCacheTable(int chainid);

        void CreateBlockTable(int chainid);
        (bool hasCache, string result) GetRpcCache(int chainid, string reqMethod, string rpcParams);
        void AddRpcCache(int chainid, string reqMethod, string reqParams, string result, TimeSpan timeSpan);
    }
}