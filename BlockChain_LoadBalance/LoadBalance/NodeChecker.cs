using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using log4net;

namespace LoadBalance
{
    public class NodeChecker :Singleton<NodeChecker> {
        private Timer _timer = null;
        private static readonly ILog Logger = LogManager.GetLogger(Log4NetCore.CoreRepository, typeof(NodeChecker));

        public NodeChecker() {
           
        }

        private void LoadConfig() {

        }


        /// <summary>
        ///
        /// in time tick check follow list 
        /// 1. block chain number height 
        /// 2. syning status
        /// 3. 
        ///
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTimerOnElapsed(object sender, ElapsedEventArgs args) {

            _timer.Stop();


            try {





            } catch (Exception ex) {
                Logger.Error(ex);
            }


            _timer.Start();
        }


        public void Start() {
            _timer.Start();
        }


    }
}
