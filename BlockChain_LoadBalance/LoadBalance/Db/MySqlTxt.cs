using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoadBalance.Db
{
    public class MySqlTxt
    {
        public const string RpcCacheTableSql = @"
CREATE TABLE if not exists `RpcCache` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `method` varchar(45) DEFAULT NULL,
  `params` varchar(450) DEFAULT NULL,
  `expiretime` int(11) unsigned DEFAULT NULL,
  `result` varchar(4000) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci";

        public const string ChainBlockTable = @"
CREATE TABLE if not exists `ChainBlock` (
  `blocknumber` int(11) unsigned NOT NULL,
  `blockhash` varchar(100) DEFAULT NULL,
  `blocktime` int(11) unsigned DEFAULT NULL,
  PRIMARY KEY (`blocknumber`),
  UNIQUE KEY `blocknumber_UNIQUE` (`blocknumber`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci";


    }
}
