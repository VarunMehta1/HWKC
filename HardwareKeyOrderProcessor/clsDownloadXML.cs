using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data;
using System.Xml.Linq;
using HardwareKeyOrderProcessor;
using System.Xml.Schema;
using HardwareKeyCreator;

namespace HardwareKeyOrderProcessor
{
   public static class clsDownloadXML
    {
        private static bool _result = false;
        public static Boolean createXML_Output(string _outPutPath)
        {

            try
            {
                //Create XML Document
                XmlDocument xmlDoc = new XmlDocument();
                // Create Master Node: Order
                XmlNode nodeOrder = xmlDoc.CreateElement("OrderStatusDetails");
                //Adding elements to Master Node : order
                //XmlAttribute attPO = xmlDoc.CreateAttribute("po");
                //attPO.Value = clsGlobalClass.dtOrderInfo.Rows[0]["po"].ToString();
                XmlAttribute attStatus = xmlDoc.CreateAttribute("Status");
                attStatus.Value = "In Progress";
                XmlAttribute attDeliveryNumber = xmlDoc.CreateAttribute("deliveryNumber");
                attDeliveryNumber.Value = clsGlobalClass.dtOrderInfo.Rows[0]["deliveryNumber"].ToString();
                XmlAttribute attDeliverydate = xmlDoc.CreateAttribute("deliveryDate");
                attDeliverydate.Value = "000000";
                //XmlAttribute attCSN = xmlDoc.CreateAttribute("csn");
                //attCSN.Value = clsGlobalClass.dtOrderInfo.Rows[0]["csn"].ToString();
                //XmlAttribute attCustomerName = xmlDoc.CreateAttribute("customerName");
                //attCustomerName.Value = clsGlobalClass.dtOrderInfo.Rows[0]["customerName"].ToString();
                //XmlAttribute attLicenseCreateDate = xmlDoc.CreateAttribute("licenseCreateDate");
                //attLicenseCreateDate.Value = clsGlobalClass.dtOrderInfo.Rows[0]["licenseCreateDate"].ToString();
                XmlAttribute attOrderNo = xmlDoc.CreateAttribute("OrderNumber");
                attOrderNo.Value = clsGlobalClass.dtOrderInfo.Rows[0]["orderNumber"].ToString();
                XmlAttribute namespace1 = xmlDoc.CreateAttribute("xmlns:xsd");
                namespace1.Value = "http://www.w3.org/2001/Lic.xsd";
                XmlAttribute namespace2 = xmlDoc.CreateAttribute("xmlns:xsi");
                namespace2.Value = "http://www.w3.org/2001/Lic.xsd";
                // Adding attributes to Master Node

                nodeOrder.Attributes.Append(namespace1);
                nodeOrder.Attributes.Append(namespace2);
                // nodeOrder.Attributes.Append(attLicenseCreateDate);
                // nodeOrder.Attributes.Append(attCSN);
                // nodeOrder.Attributes.Append(attCustomerName);
                nodeOrder.Attributes.Append(attDeliveryNumber);
                nodeOrder.Attributes.Append(attStatus);
                nodeOrder.Attributes.Append(attDeliverydate);

                nodeOrder.Attributes.Append(attOrderNo);
                xmlDoc.AppendChild(nodeOrder);
                //    adding Node to master node
                XmlNode nodeKeys = xmlDoc.CreateElement("KeyStatusDetails");

                XmlAttribute attdevideID = xmlDoc.CreateAttribute("deviceID");
                attdevideID.Value = "Device ID";// clsGlobalClass.dtKeyInfo.Rows[0]["activationCode"].ToString();
                XmlAttribute attAction = xmlDoc.CreateAttribute("action");
                attAction.Value = "Action";//clsGlobalClass.dtKeyInfo.Rows[0]["activationCode"].ToString();
                XmlAttribute attNameKeys = xmlDoc.CreateAttribute("name");
                attNameKeys.Value = "Name";// clsGlobalClass.dtKeyInfo.Rows[0]["activationCode"].ToString();
                nodeKeys.Attributes.Append(attdevideID);
                nodeKeys.Attributes.Append(attAction);
                nodeKeys.Attributes.Append(attNameKeys);
                //    loop N Times:
                //N: No of licensee
                for (int i = 0; i < clsGlobalClass.dtKeyInfo.Rows.Count; i++)
                {
                    XmlNode nodeKey = xmlDoc.CreateElement(@"ActivationCodes");
                    nodeKeys.AppendChild(nodeKey);
                    XmlNode nodeLic = xmlDoc.CreateElement("ActivationCode");
                    nodeLic.InnerText = clsGlobalClass.dtLicenseInfo.Rows[i]["activationCode"].ToString();
                    nodeKey.AppendChild(nodeLic);
                }
                nodeOrder.AppendChild(nodeKeys);
                //Add new Node: CreatedImageFile
                XmlNode nodeCreatedImageFile = xmlDoc.CreateElement(@"CreatedImageFile");
                XmlAttribute attDescription = xmlDoc.CreateAttribute("Password");
                attDescription.Value = "Password";
                XmlAttribute attFileNAme = xmlDoc.CreateAttribute("FileNAme");
                attFileNAme.Value = "File Name";
                //Adding Attributes to node and append
                nodeCreatedImageFile.Attributes.Append(attDescription);
                nodeCreatedImageFile.Attributes.Append(attFileNAme);
                nodeOrder.AppendChild(nodeCreatedImageFile);
                //Add new Node: UpdatedSFDC
                XmlNode nodeUpdatedSFDC = xmlDoc.CreateElement(@"UpdatedSFDC");
                XmlAttribute attError = xmlDoc.CreateAttribute("error");
                attError.Value = "Object reference not set to an instance of an object.";
                //Adding Attributes to node and append
                nodeUpdatedSFDC.Attributes.Append(attError);
                nodeOrder.AppendChild(nodeUpdatedSFDC);
                //Add new Node: KeyStatus
                XmlNode nodeKeyStatus = xmlDoc.CreateElement(@"KeyStatus");
                XmlAttribute attKeyStatus = xmlDoc.CreateAttribute("Description");
                //Adding Attributes to node and append
                attKeyStatus.Value = "Error occured updating Sales Force for the keyless.";
                nodeKeyStatus.Attributes.Append(attKeyStatus);
                nodeOrder.AppendChild(nodeKeyStatus);
                //Save XML Output to BuildOutput. 
                xmlDoc.Save(System.IO.Path.GetFileName(_outPutPath));
                _result = true;
            }
            catch (Exception ex) { _result = false; 
            }
            return _result;
        }
    }
}
