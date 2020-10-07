using Aladdin.HASP;
using System;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.XPath;

namespace HardwareKeyCreationTool
{
    internal class HaspSessionWrapper
    {
        //Imports for the hidden memory functions from Gemalto's LDK .dll
        [DllImport("hasp_windows_113677.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int hasp_login(uint featureId, string vendorCode, out uint haspHandle);
        [DllImport("hasp_windows_113677.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int hasp_logout(uint haspHandle);

        //Vendor code that is used to login to hidden memory key and open Hasp session
        private static readonly string vendorcode =
                "GlNmbERpux9LlegBuqOktPuo9X1nkhfI2yKWJJCBgwgqODRs5biXnnQIUds4YT4maahZL7WcLrSXEj45" +

        "QwbTnaYpYGPz994o/sBUFERWsVnJxskhCpcUB6DFaz0gt7kIjv/aUifpnWTbSOaDQxqmwqqZLHpyUXnv" +

        "G3aPcUiflfXVRkeDhCu8zIy4+nPaMNfpYLNmLpkHzKrAd2brio+Xop9aPOU9kYFk/oi1G0+VsBpoSrmJ" +

        "FZAeAqA6CW7zIiEcxyKU39HT/5K+dhJ9ErQPXF7/8ve57ZcjWl0n2W4TARXvALedYAFodBODVCcuFnJh" +

        "s7NMs8r1VZ4xSfWPZ64O1jx+90vqZNzgbVr8bctv275nDj9K4UdnAugtoXF0X6agHtocIHJo17413Z1l" +

        "HV8pDXm5vgPqKVExD1VE5vD5lG9uy5pf3SUg6oDl0OAnGPy0Pn8cdFhHSvZaXMQk31Vg6zdOvgA42IFz" +

        "41ghU1LRfqGNcJyHsk34r3orJRPyOaozK0fmIC3VTJsoFxJlEOF7qgnXu/RlhC2NW7htdmBM5IKCcWmi" +

        "+flKdFMxsFaEXMYSDqNYfVOXdK/xJkaPM2IT60mYx0UlovRYBszAiFTl3BlCw+P8WhJAWY7CftKsDDTm" +

        "7+zMJbq5rRYGmDfpUED9+8d5VUEHX/VCbPOlQtTyTsHLGW1UMO15Ckqc4nDGvqHOej2N0Ms0alZZx/Tr" +

        "HAW/dJbsYOYx3+KiMWkX/Dnnnv/M3W0W8rXUVVTN9aN9TIJaOXlcQrZab8owGyDMBIw3Weqn3peBjpwz" +

        "EIBQtS6t0pOcglzAaAzuRky6Rj7rSl5Vfyd8xR6q7q/27fPd5RjqrAsmWkswKzvinynu+YyaUSvlCUut" +

        "RTOMsP/TzPlPhytQH25QEqgUXfskNye5V8htd+9fC7KAzbL3fKQt6HKL/G/4HpyCuLsTiGHIdGUnVKbv" +

        "hJIUkv4oZwK2w7AY73jgrQ==";

        private static readonly log4net.ILog log = LoggerUtility.GetLogger();

        //The memory address/handle for the hasp login session
        internal uint haspHandle { get; set; }

        internal HaspSessionWrapper()
        {
            haspHandle = 0;
        }
        internal bool isLoggedIn
        {
            get
            {
                if (haspHandle == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        //Function to renew the hidden memory and hasp handles if they have never been opened or have been previously closed.
        internal void Renew(uint featureId = 1)
        {
            if (haspHandle == 0)
            {
                haspHandle = HaspLogin(featureId);
            }
            else
            {
                HaspLogout();
                HaspLogin();
            }
        }

        internal void HaspLogout()
        {
            int rc = hasp_logout(haspHandle);
            haspHandle = 0;
        }

        /// <summary>
        /// Function to login, and open hasp session
        /// </summary>
        /// <returns>A unsigned int with the address of the hasp session/memory handle</returns>
        private uint HaspLogin(uint featureId = 1)
        {
            int rc = hasp_login(featureId, vendorcode, out uint haspHandle);
            if (rc != 0)
            {
                hasp_logout(haspHandle);
                string error = "There was an issue with the hasp login,  hasp library exited with code " + rc.ToString();
                log.Error(error);
                throw new HardwareConfiguratorException(rc, error);
            }
            return haspHandle;
        }

        internal string GetToken()
        {
            HaspLogout();
            HaspFeature feature = HaspFeature.Default;
            using (Hasp hasp = new Hasp(feature))
            {
                HaspStatus status = hasp.Login(vendorcode);
                //First check to see if the hasp library was able to login, if not then there is no key inserted
                if (status != HaspStatus.StatusOk)
                {
                    string error = $"The attempt to get the token/fingerprint from the hardware key failed with error code {(int)status}";
                    log.Error(error);
                    throw new HardwareConfiguratorException((int)status, error);
                }
                string format =
    "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +

        "<haspformat root=\"hasp_info\">" +

        "    <hasp>" +

        "        <attribute name=\"id\" />" +

        "        <attribute name=\"type\" />" +

        "        <feature>" +

        "            <attribute name=\"id\" />" +

        "        </feature>" +

        "    </hasp>" +

        "</haspformat>";
                string info = "";
                status = hasp.GetSessionInfo(format, ref info);
                if (status.Equals(HaspStatus.StatusOk))
                {
                    try
                    {
                        XmlDocument response = new XmlDocument();
                        response.LoadXml(info);
                        XmlAttributeCollection nodeAttributes = response.SelectSingleNode("hasp_info/hasp").Attributes;
                        string token = nodeAttributes.Item(0).InnerText;
                        if (string.IsNullOrEmpty(token))
                        {
                            string error = "The fingerprint on the hardware key was returned as a empty string";
                            log.Error(error);
                            throw new HardwareConfiguratorException((int)NonHaspExceptions.KEY_FINGERPRINT_EMPTY, error);
                        }
                        log.Info($"There was a hardware key detected with token {token}");
                        Renew();
                        return token;
                    }
                    catch (SystemException e)
                    {
                        if (e is XmlException || e is XPathException)
                        {
                            string error = "There was an error parsing the token from the xml returned from the hardware key.";
                            log.Error(error);
                            Renew();
                            throw new HardwareConfiguratorException((int)NonHaspExceptions.KEY_FINGERPRINT_XML_FAIL, error);
                        }
                        else
                        {
                            string error = "There was a general system error when parsing the token/fingerprint from the xml returned from the hardware key";
                            log.Error(error);
                            Renew();
                            throw new HardwareConfiguratorException((int)NonHaspExceptions.GENERAL_FAIL, error);
                        }
                    }
                }
                else
                {
                    string error = $"There was a general error when retrieving the token from the hardware key, failed with status code {(int)status}";
                    log.Error(error);
                    throw new HardwareConfiguratorException((int)status, error);
                }
            }
        }
    }
}
