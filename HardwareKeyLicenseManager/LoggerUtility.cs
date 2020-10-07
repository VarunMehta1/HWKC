using System.Runtime.CompilerServices;

namespace HardwareKeyCreationTool
{
    internal class LoggerUtility
    {
        /// <summary>
        /// Utility function to get a logger for the correct class
        /// </summary>
        /// <param name="fileName">Automatically retrieves caller path, no need to pass this parameter</param>
        /// <returns>An Ilog object used for logging </returns>
        internal static log4net.ILog GetLogger([CallerFilePath]string fileName = "")
        {
            return log4net.LogManager.GetLogger(fileName);
        }
    }
}
