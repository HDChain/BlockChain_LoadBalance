using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoadBalance.Models
{
    /// <summary>
    ///
    /// report error node
    ///
    /// when node status check
    /// 1. crash
    /// 2. not syncing 
    /// 3. last block time Excess threshold
    /// 
    /// </summary>
    public class AlarmMgr : Singleton<AlarmMgr>
    {
        public void OnServerNoResponse(int chainid,ServerDefine serverDefine) {

        }

        public void OnServerNotSyncing(int chainid,ServerDefine serverDefine) {

        }

        public void OnServerSyncTimeout(int chainid, ServerDefine serverDefine) {

        }


        #region smtp alarm

        





        #endregion


    }
}
