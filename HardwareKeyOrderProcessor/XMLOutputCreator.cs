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
    /*
         ****************************************************************************************************
         *Class:       clsDownloadXML
         *Purpose:     Download XML Output.Input XML file path as parameter.
         *Name:        Varun Mehta
         *CreatedDate: 19-Oct-2020
         *****************************************************************************************************
         */
    public  class XMLOutputCreator
    {
        
        private   bool ResultOutPut = false;
        /*
         ****************************************************************************************************
         *Function:    createXML_Output(string parameter)
         *Purpose:     take InputXMlPath and read and saves new XML to harddrive.
         *Name:        Varun Mehta
         *CreatedDate: 19-Oct-2020
         *****************************************************************************************************
         */
        
        public   Boolean CreateXMLOutput(string OutputPath)
        {

            try
            {
               
                   //Create XML Document
                   XmlDocument xmlDoc = new XmlDocument();

                // Create Master Node: Order
                XmlNode nodeOrder = xmlDoc.CreateElement("OrderStatusDetails");
                XmlAttribute attStatus = xmlDoc.CreateAttribute("Status");
                attStatus.Value = "In Progress";

                XmlAttribute attDeliveryNumber = xmlDoc.CreateAttribute("deliveryNumber");
                attDeliveryNumber.Value = XMLDataTable.DataTableOrders.Rows[0]["deliveryNumber"].ToString();

                XmlAttribute attDeliverydate = xmlDoc.CreateAttribute("deliveryDate");
                attDeliverydate.Value = "000000";

                XmlAttribute attOrderNo = xmlDoc.CreateAttribute("OrderNumber");
                attOrderNo.Value = XMLDataTable.DataTableOrders.Rows[0]["orderNumber"].ToString();

                XmlAttribute namespace1 = xmlDoc.CreateAttribute("xmlns:xsd");
                namespace1.Value = "http://www.w3.org/2001/Lic.xsd";

                XmlAttribute namespace2 = xmlDoc.CreateAttribute("xmlns:xsi");
                namespace2.Value = "http://www.w3.org/2001/Lic.xsd";
                // Adding attributes to Master Node

                nodeOrder.Attributes.Append(namespace1);
                nodeOrder.Attributes.Append(namespace2);
                nodeOrder.Attributes.Append(attDeliveryNumber);
                nodeOrder.Attributes.Append(attStatus);
                nodeOrder.Attributes.Append(attDeliverydate);
                nodeOrder.Attributes.Append(attOrderNo);
                xmlDoc.AppendChild(nodeOrder);
                //    adding Node to master node
                XmlNode nodeKeys = xmlDoc.CreateElement("KeyStatusDetails");

                XmlAttribute attdevideID = xmlDoc.CreateAttribute("deviceID");
                attdevideID.Value = "Device ID";

                XmlAttribute attAction = xmlDoc.CreateAttribute("action");
                attAction.Value = "Action";

                XmlAttribute attNameKeys = xmlDoc.CreateAttribute("name");
                attNameKeys.Value = "Name";

                nodeKeys.Attributes.Append(attdevideID);
                nodeKeys.Attributes.Append(attAction);
                nodeKeys.Attributes.Append(attNameKeys);
        
                for (int i = 0; i < XMLDataTable.DataTableKeys.Rows.Count; i++)
                {
                    XmlNode nodeKey = xmlDoc.CreateElement(@"ActivationCodes");
                    nodeKeys.AppendChild(nodeKey);

                    XmlNode nodeLic = xmlDoc.CreateElement("ActivationCode");
                    nodeLic.InnerText = XMLDataTable.DataTableLic.Rows[i]["activationCode"].ToString();

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
                attKeyStatus.Value = "Error occured updating Sales Force for the keyless.";

                //Adding Attributes to node and append
                nodeKeyStatus.Attributes.Append(attKeyStatus);
                nodeOrder.AppendChild(nodeKeyStatus);

                xmlDoc.Save((System.IO.Path.GetFileName(OutputPath)));
                ResultOutPut = true;
            }
            catch (Exception ex) {
                ResultOutPut = false;
                log.Error(ex.Message.ToString());
             
            }
            return ResultOutPut;
        }
        private   readonly log4net.ILog log = LoggerUtility.GetLogger();
    }
}
