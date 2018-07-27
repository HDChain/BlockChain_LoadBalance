using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoadBalance.Models
{
    /// <summary>
    /// logic for report chain node exception event
    /// </summary>
    public class ChainNotify
    {

        public enum ErrorLevel {
            Error,
            Warn,
            Info,
        }



        public void ReportError(int chainId, ServerDefine server, string error,ErrorLevel level) {

        }
    }
}
