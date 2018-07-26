using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace LoadBalance
{
    public class EthNodeChecker {
        private Timer _timer = null;

        public EthNodeChecker(double checkInternal) {
            _timer = new Timer(checkInternal);

            _timer.Elapsed += OnTimerOnElapsed;
        }

        private void OnTimerOnElapsed(object sender, ElapsedEventArgs args) {








        }


        public void Start() {
            _timer.Start();
        }


    }
}
