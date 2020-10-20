using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;



namespace HardwareKeyOrderProcessor
{
    public class XMLFileParser : IXmlFileParser
    {
        /// <summary>
        /// Create log object to log error information in file
        /// </summary>
        private static readonly log4net.ILog log = LoggerUtility.GetLogger();

        /// <summary>
        /// Create Dictionary object to hold different keys object in given xml file
        /// </summary>
        public Dictionary<string, XmlNodeList> xNodeList;



        /// <summary>
        /// Get Xml file name as input
        /// Read Xml file using XmlDocument
        /// Get different nodes like Order,Keys,License and store in Dictionary Object
        /// Returns Dictionary object
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>



        public Dictionary<string, XmlNodeList> ReadXmlFile(string fileName)
        {
            XmlNodeList xOrderList = null;
            XmlNodeList xkeysList = null;
            XmlNodeList xLicenseList = null;
            xNodeList = new Dictionary<string, XmlNodeList>();

            try
            {
                //Load xml file in doc object
                XmlDocument doc = new XmlDocument();
                doc.Load(fileName);

                //Read Order node and store information in Dictionary Object
                xOrderList = doc.SelectNodes("/Order");
                xNodeList.Add("Order", xOrderList);

                //Read Keys node and store information in Dictionary Object
                xkeysList = doc.SelectNodes("/Order/Keys/Key");
                xNodeList.Add("Keys", xkeysList);

                //Read Licenses node and store information in Dictionary Object
                xLicenseList = doc.SelectNodes("/Order/Keys/Key/Licenses/License");
                xNodeList.Add("License", xLicenseList);
            }
            catch (XmlException xe)
            {
                string error = $"There was a general error while reading xml file " + xe.Message;
                log.Error(error);
            }
            catch (Exception ex)
            {
                string error = $"There was a general error when retrieving info from file.. Error: " + ex.Message;
                log.Error(error);

            }



            return xNodeList;
        }
    }
    /// <summary>
    /// Not sure if I need to again write same LoggerUtility class here,as its already available 
    /// in HardwareKeyLicenseManager project but I dont want to add reference of that project just for 
    /// LoggerUtility class, need to seperate out this class in common kind of project
    /// </summary>
    public class LoggerUtility
    {
        internal static log4net.ILog GetLogger([CallerFilePath] string fileName = "")
        {
            return log4net.LogManager.GetLogger(fileName);
        }
    }



}
