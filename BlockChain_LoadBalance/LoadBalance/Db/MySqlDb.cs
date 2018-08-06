﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using log4net;
using MySql.Data.MySqlClient;

namespace LoadBalance.Db
{
    public class MySqlDb : IRDb
    {
        private static string SysConnectionString = "Data Source=mysql;port=3306;Initial Catalog=sys;user id=root;password=mysqlP@ssw0rd;Charset=utf8";
        private const string ChainConnectionString = "Data Source=mysql;port=3306;Initial Catalog=Chain_{0};user id=root;password=mysqlP@ssw0rd;Charset=utf8";
        private static readonly ILog Logger = LogManager.GetLogger(Log4NetCore.CoreRepository, typeof(MySqlDb));

        public string[] GetDataBaseList() {

            try {
                using (var db = new MySqlConnection(SysConnectionString)) {
                    return  db.Query<string>("show databases;").ToArray();
                }
            } catch (Exception ex) {
                Logger.Error(ex);
                return new string[0];
            }
        }

        public bool CreateDatabase(string dbname) {
            try {
                using (var db = new MySqlConnection(SysConnectionString)) {
                    db.Execute($"create database {dbname};");
                }
                
                return true;
            } catch (Exception ex) {
                Logger.Error(ex);
                return false;
            }
        }

        public bool CheckAndCreateDatabase(string dbname) {
            try {
                using (var db = new MySqlConnection(SysConnectionString)) {
                    var dbs = db.Query<string>("show databases;").ToArray();
                    if (dbs.Contains(dbname)) {
                        return true;
                    }

                    db.Execute($"create database {dbname};");
                }
                
                return true;
            } catch (Exception ex) {
                Logger.Error(ex);
                return false;
            }
        }
        
        public void CreateRpcCacheTable(int chainid) {
            try {

                using (var db = new MySqlConnection(GetChainConnectStringById(chainid))) {
                    db.Execute(MySqlTxt.RpcCacheTableSql, null);
                }


            } catch (Exception ex) {
                Logger.Error(ex);
            }
        }

        public void CreateBlockTable(int chainid) {
            
            try {

                using (var db = new MySqlConnection(GetChainConnectStringById(chainid))) {
                    db.Execute(MySqlTxt.ChainBlockTable, null);
                }
                
            } catch (Exception ex) {
                Logger.Error(ex);
            }
        }

        public (bool hasCache, string result) GetRpcCache(int chainid, string reqMethod, string rpcParams) {
            try {
                
                using (var db = new MySqlConnection(GetChainConnectStringById(chainid))) {

                    var row = db.QueryFirst("select * from RpcCache where method=@method and params=@params;", new {
                        method = reqMethod,
                        @params = rpcParams
                    });

                    if (row == null) {
                        return (false, string.Empty);
                    }

                    if (row.expiretime < DateTime.Now.Ticks) {

                        db.Execute("delete from RpcCache where id=@id",new {
                            row.id
                        });

                        return (false, string.Empty);
                    }

                    return (true ,row.result);
                }

            } catch (Exception ex) {
                Logger.Error(ex);

                return (false, string.Empty);
            }
        }

        public void AddRpcCache(int chainid, string reqMethod, string reqParams, string result, TimeSpan timeSpan) {
            try {
                using (var db = new MySqlConnection(GetChainConnectStringById(chainid))) {
                    




                }
            } catch (Exception ex) {
                Logger.Error(ex);
            }
        }

        public int ExecSql(int chainid,string sql,object sqlparams) {
            try {
                using (var db = new MySqlConnection(GetChainConnectStringById(chainid))) {
                    return db.Execute(sql, sqlparams);
                }

            } catch (Exception ex) {
                Logger.Error(ex);

                return -1;
            }
        }

        public List<T> Query<T>(int chainid, string sql, object sqlparams) {
            try {
                using (var db = new MySqlConnection(GetChainConnectStringById(chainid))) {
                    return db.Query<T>(sql, sqlparams).ToList();
                }

            } catch (Exception ex) {
                Logger.Error(ex);

                return new List<T>();
            }
        }


        private string GetChainConnectStringById(int chainid){
            return string.Format(ChainConnectionString,chainid);
        }
    }
}
