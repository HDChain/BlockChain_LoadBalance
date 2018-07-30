﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using LoadBalance.Models;

namespace LoadBalance.NodeCheckers
{
    public interface INodeChecker {

        void SetConfig(int chainid, ChainType chainType, ServerDefine server);

        ServerDefine GetConfig();

        int GetChainId();

        /// <summary>
        /// Get Chain Server Current Block Chain Number
        /// </summary>
        /// <returns></returns>
        long GetBlockNumber();

        /// <summary>
        /// Get Server Connected Peers Count
        /// </summary>
        /// <returns></returns>
        int GetPeersCount();

        /// <summary>
        /// Get Heightest Block Created time
        /// </summary>
        /// <returns></returns>
        long GetLastestBlockTimeTicker();


        /// <summary>
        /// Get Transaction Count Of Some User
        ///
        /// for ethereum , user transactions perfer send to same server 
        ///
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        int GetUserPendingTransactionCount(string address);

        Timer GetTimer();

        void Start();

        void Destory();

    }
}
