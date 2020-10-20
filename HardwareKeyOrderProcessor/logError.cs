using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareKeyOrderProcessor
{
    class logError
    {
        internal static log4net.ILog GetLogger(string fileName = "")
        {
            return log4net.LogManager.GetLogger(fileName);
        }
    }
}
