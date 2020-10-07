using com.sntl.licensing;
using com.sntl.licgen.parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace HardwareKeyCreationTool
{
    public class HardwareKeyConfigurator : IHardwareKeyConfigurator
    {
        [DllImport("sntl_pm_windows.dll", CallingConvention = CallingConvention.StdCall)]
        static extern int sntl_pm_enum_key(UIntPtr memHandle, ref IntPtr keyList);
        [DllImport("sntl_pm_windows.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int sntl_pm_get_key_value(UIntPtr memHandle, string key, uint offset, uint length, byte[] data);
        [DllImport("sntl_pm_windows.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int sntl_pm_set_key_value(UIntPtr memHandle, string key, uint offset, uint length, byte[] data, uint passOne);
        [DllImport("sntl_pm_windows.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int sntl_pm_delete_key(UIntPtr memHandle, string key);
        [DllImport("sntl_pm_vendor_windows.dll", CallingConvention = CallingConvention.StdCall)]
        static extern int sntl_pm_create(uint haspHandle, uint sizeInMBs, uint passThirtyTwo, IntPtr password);
        [DllImport("sntl_pm_vendor_windows.dll", CallingConvention = CallingConvention.StdCall)]
        static extern int sntl_pm_delete(uint haspHandle, uint passTwo);
        [DllImport("sntl_pm_windows.dll", CallingConvention = CallingConvention.StdCall)]
        static extern int sntl_pm_close(UIntPtr memHandle);
        [DllImport("sntl_pm_windows.dll", CallingConvention = CallingConvention.StdCall)]
        static extern int sntl_pm_open(uint haspHandle, ref UIntPtr memHandle, IntPtr info);

        private readonly HaspSessionWrapper haspSession;
        private readonly Utility utility;
        //The memory address/handle for the hidden memory on the gemalto hardware key
        private UIntPtr hiddenMemoryHandle;
        public HardwareKeyConfigurator()
        {
            haspSession = new HaspSessionWrapper();
            utility = new Utility();
        }
        ~HardwareKeyConfigurator()
        {
            if (hiddenMemoryHandle != UIntPtr.Zero)
            {
                CloseHiddenMemory();
            }
            if (haspSession.haspHandle != 0)
            {
                haspSession.HaspLogout();
            }
        }
       private static readonly log4net.ILog log = LoggerUtility.GetLogger();

        private bool hiddenMemoryIsOpen
        {
            get
            {
                if (hiddenMemoryHandle == UIntPtr.Zero)
                {
                    return false;
                }
                return true;
            }
        }

        public List<string> GetSavedProducts()
        {
            if (!haspSession.isLoggedIn)
            {
                haspSession.Renew();
            }
            if (!hiddenMemoryIsOpen)
            {
                OpenHiddenMemory();
            }
            HashSet<string> products = new HashSet<string>();
            LicgenContext parser = new LicgenContext();
            char[] separator = { ';' };
            List<string> licenses = ReadLicenses();
            foreach (string license in licenses)
            {
                try
                {
                    string licInfo = parser.parse(license);
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(licInfo);
                    string productBase64 = xml.SelectSingleNode("license/feature/licenseVendorInfo").InnerText;
                    string product = utility.Base64Decode(productBase64).Split(separator)[1];
                    products.Add(product);
                }
                catch (LicensingException e)
                {
                    string error = $"There was an error thrown when parsing the license strings: \n {e.Message}";
                    log.Error(error);
                    throw new HardwareConfiguratorException(e.getStatusCode(), error);
                }
                catch (Exception e)
                {
                    string error = $"There was a general error thrown when parsing the license strings";
                    log.Error(error);
                    throw new HardwareConfiguratorException((int)NonHaspExceptions.GENERAL_FAIL, error);
                }
            }
            return products.ToList();
        }

        public void SaveInHiddenMemory(string[] licenses)
        {
            if (!haspSession.isLoggedIn)
            {
                haspSession.Renew();
            }
            if (!hiddenMemoryIsOpen)
            {
                OpenHiddenMemory();
            }
            string keylist = GetKeys();
            bool memExists = !keylist.Equals("no keys in hidden memory");
            Dictionary<string, uint> ParsedKeys = new Dictionary<string, uint>();
            if (memExists)
            {
                ParsedKeys = ParseKeyList(keylist);
            }
            LicgenContext parser = new LicgenContext();
            char[] separator = { ';' };
            foreach (string license in licenses)
            {
                string licInfo = parser.parse(license);
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(licInfo);
                string productBase64 = xml.SelectSingleNode("license/feature/licenseVendorInfo").InnerText;
                string featureName = xml.SelectSingleNode("license/feature/featureName").InnerText;
                string keyName = utility.Base64Decode(productBase64).Split(separator)[1] + featureName;
                if (ParsedKeys.ContainsKey(keyName))
                {
                    continue;
                }
                log.Info("Saving the license string for product-feature " + keyName + "to hidden memory");
                log.Info("The complete license string that will be saved is: \n " + license);
                SetHiddenMemory(keyName, license);
            }
        }
        private void SetHiddenMemory(string key, string value)
        {
            byte[] valueBytes = Encoding.ASCII.GetBytes(value);
            int rc = sntl_pm_set_key_value(hiddenMemoryHandle, key, 0, (uint)valueBytes.Length, valueBytes, 1);
            if (rc != 0)
            {
                CloseHiddenMemory();
                haspSession.HaspLogout();
                string error = $"The license for product-feature {key} was not correctly saved, exited with code {rc}";
                log.Error(error);
                throw new HardwareConfiguratorException(rc, error);
            }
        }
        private void DeleteKey(string key)
        {
            int rc = sntl_pm_delete_key(hiddenMemoryHandle, key);
            if (rc != 0)
            {
                CloseHiddenMemory();
                haspSession.HaspLogout();
                string error = $"There was an issue deleting the key {key} from hidden memory, exited with code {rc}";
                log.Error(error);
                throw new HardwareConfiguratorException(rc, error);
            }
        }
        public void WipeHiddenMemory()
        {
            if (!hiddenMemoryIsOpen)
            {
                OpenHiddenMemory();
            }
            Dictionary<string, uint> keyAndBytes = ParseKeyList(GetKeys());
            foreach (string key in keyAndBytes.Keys)
            {
                log.Info($"Deleting feature-product {key} from hidden memory");
                DeleteKey(key);
            }
        }
        public void CreateHiddenMemory(uint sizeInMBs)
        {
            if (!haspSession.isLoggedIn)
            {
                haspSession.Renew();
            }
            if (hiddenMemoryIsOpen)
            {
                CloseHiddenMemory();
            }
            IntPtr nullPtr = IntPtr.Zero;//Currently we have not spoken about implementing passwords for the creation of hidden memory on hardware CTO key, if we do this, I need to clarify some stuff with Jake first.
            int rc = sntl_pm_create(haspSession.haspHandle, sizeInMBs, 32, nullPtr);
            if (rc != 0)
            {
                CloseHiddenMemory();
                haspSession.HaspLogout();
                string error = $"There was an issue with creating the hidden memory, exited with code {rc}";
                log.Error(error);
                throw new HardwareConfiguratorException(rc, error);
            }
        }
        public void DeleteHiddenMemory()
        {
            if (!haspSession.isLoggedIn)
            {
                haspSession.Renew();
            }
            if (hiddenMemoryIsOpen)
            {
                CloseHiddenMemory();
            }
            int rc = sntl_pm_delete(haspSession.haspHandle, 2);
            if (rc != 0)
            {
                if (rc == 8038)
                {
                    return;
                }
                haspSession.HaspLogout();
                string error = $"There was an issue with deleting the hidden memory, exited with code {rc}";
                log.Error(error);
                throw new HardwareConfiguratorException(rc, error);
            }
        }
        /// <summary>
        /// Function to read the licenses stored in the hidden memory
        /// </summary>
        /// <returns>List of the license string stored in the hidden memory</returns>
        private List<string> ReadLicenses()
        {
            string keylist = GetKeys();
            Dictionary<string, uint> ParsedKeys = ParseKeyList(keylist);
            List<string> licenses = GetLicenses(ParsedKeys);
            return licenses;
        }


        /// <summary>
        /// Function to retrieve the keys for the hidden memory dictionary
        /// </summary>
        /// <returns>The keys for the hidden memory dictionary, returned in XML format</returns>
        private string GetKeys()
        {
            IntPtr keys = IntPtr.Zero;
            int rc = sntl_pm_enum_key(hiddenMemoryHandle, ref keys);
            if (rc != 0)
            {
                if (rc == 8024)
                {
                    string nokeys = "no keys in hidden memory";
                    log.Error("no keys found in the USB's hidden memory");
                    return nokeys;
                }
                CloseHiddenMemory();
                haspSession.HaspLogout();
                string error = $"There was an issue with retrieving the key list from the hidden memory, exited with code {rc}";
                log.Error(error);
                throw new HardwareConfiguratorException(rc, error);
            }
            string keylist = Marshal.PtrToStringAnsi(keys);
            return keylist;
        }
        /// <summary>
        /// Function to parse the xml key list for the keys to the hidden memory dictionary, and the associated number of bytes for each key.
        /// </summary>
        /// <param name="keylist">The key list for the hidden memory, in XML format</param>
        /// <returns>A dictionary with the keys, and the associated number of bytes for each key</returns>
        private Dictionary<string, uint> ParseKeyList(string keylist)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.LoadXml(keylist);
                XmlNodeList key_nodes = xml.SelectNodes("protected_memory/key_value");
                Dictionary<string, uint> keysAndBytes = new Dictionary<string, uint>();
                foreach (XmlNode xmlNode in key_nodes)
                {
                    string sizeText = xmlNode.SelectSingleNode("size").InnerText;
                    uint size = uint.Parse(sizeText);
                    if (size == 0)
                    {
                        CloseHiddenMemory();
                        haspSession.HaspLogout();
                        string error = "There was an issue parsing the keylist from hidden memory, a key value pair has a size of zero";
                        log.Error(error);
                        throw new HardwareConfiguratorException((int)NonHaspExceptions.KEY_VALUE_SIZE_ZERO_FAIL, error);
                    }
                    keysAndBytes.Add(xmlNode.SelectSingleNode("name").InnerText, size);
                }
                return keysAndBytes;
            }
            catch (SystemException e)
            {
                if (e is XmlException || e is XPathException)
                {
                    throw new HardwareConfiguratorException((int)NonHaspExceptions.HIDDEN_MEMORY_XML_FAIL, "There was an issue in parsing the XML retrieved from the hidden memory");
                }
                else
                {
                    throw new HardwareConfiguratorException((int)NonHaspExceptions.GENERAL_FAIL, "There was a general system failure when attempting to parse the XML retrieved from the hidden memory");
                }
            }
        }
        /// <summary>
        /// Private function to get a list of the license strings stored in the hardware key hidden memory
        /// </summary>
        /// <param name="keyValuePairs">Dictionary storing the keys used in the hidden memory, and the number of bytes associated with each key</param>
        /// <returns>List of the license strings stored in the hidden memory</returns>
        private List<string> GetLicenses(Dictionary<string, uint> keyValuePairs)
        {
            List<string> licenses = new List<string>();
            byte[] valueBuffer;
            foreach (KeyValuePair<string, uint> item in keyValuePairs)
            {
                valueBuffer = new byte[item.Value];
                int rc = sntl_pm_get_key_value(hiddenMemoryHandle, item.Key, 0, item.Value, valueBuffer);
                if (rc != 0)
                {
                    CloseHiddenMemory();
                    haspSession.HaspLogout();
                    string error = $"There was an issue with retrieving key-value pairs from hidden memory, exited with code {rc}";
                    log.Error(error);
                    throw new HardwareConfiguratorException(rc, error);
                }
                string licenseString = Encoding.UTF8.GetString(valueBuffer, 0, valueBuffer.Length);
                licenses.Add(licenseString);
            }
            return licenses;
        }
        /// <summary>
        /// Function to open the hidden memory
        /// </summary>
        /// <returns>Handle for the hidden memory</returns>
        private void OpenHiddenMemory()
        {
            IntPtr info = IntPtr.Zero;
            int rc = sntl_pm_open(haspSession.haspHandle, ref hiddenMemoryHandle, info);
            if (rc != 0)
            {
                if (hiddenMemoryHandle != UIntPtr.Zero)
                {
                    CloseHiddenMemory();
                }
                haspSession.HaspLogout();
                string error = $"There was an issue closing the hidden memory, hasp library exited with code {rc}";
                log.Error(error);
                throw new HardwareConfiguratorException(rc, error);
            }
        }
        /// <summary>
        /// Function to close the hasp session/hidden memory
        /// </summary>
        private void CloseHiddenMemory()
        {
            if (hiddenMemoryHandle == UIntPtr.Zero)
            {
                return;
            }
            int rc = sntl_pm_close(hiddenMemoryHandle);
            hiddenMemoryHandle = UIntPtr.Zero;
            if (rc != 0)
            {
                haspSession.HaspLogout();
                string error = $"There was an issue closing the hidden memory, hasp library exited with code {rc}";
                log.Error(error);
                throw new HardwareConfiguratorException(rc, error);
            }
        }
    }
}
