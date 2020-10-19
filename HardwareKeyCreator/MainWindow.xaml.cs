using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Data;
using System.Xml.Linq;
using HardwareKeyOrderProcessor;
using System.Xml.Schema;
using MahApps.Metro.Controls;
namespace HardwareKeyCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// Dictionary Object which will return key-value list from XMLFileParser
        /// </summary>

        Dictionary<string, XmlNodeList> XNodeList;



        public MainWindow()
        {
            InitializeComponent();
        }
        public bool _result;

        //Function Info.
        /*
         ****************************************************************************************************
         *Function:compareXMLXSD
         *Purpose:To compare the uploaded XML, with XML Schema Set.If fails it reurn. 
         *Name: Varun Mehta
         *Change Date:15-Oct-2020
         *****************************************************************************************************
         */
        private Boolean compareXMLXSD(string _xml, string _xsd)
        {

            try
            {
                var path = new Uri(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).LocalPath;
                XmlSchemaSet schema = new XmlSchemaSet();
                schema.Add("",path + "\\HardwareKeyOrders.xsd");
                XmlReader rd = XmlReader.Create(_xml);// (path + "\\input.xml");
                XDocument doc = XDocument.Load(rd);
                doc.Validate(schema, ValidationEventHandler);
                _result = true;
            }
            catch (Exception ee)
            {
                
                return false; ;
            }
            return _result;
        }
        //Function Info.
        /*
         ****************************************************************************************************
         *Function:ValidationEventHandler
         *Purpose:To validate the uploaded XML, with XML Schema Set.
         *Name: Varun Mehta
         *Change Date:15-Oct-2020
         *****************************************************************************************************
         */
        static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {

            XmlSeverityType type = XmlSeverityType.Warning;
            if (Enum.TryParse<XmlSeverityType>("Error", out type))
            {
                if (type == XmlSeverityType.Error) MessageBox.Show(e.Message.ToString(), "Validator: Value provided doesn't match with ");  //throw new Exception(e.Message);
            }

        }
        //Function Info.
        /*
         ****************************************************************************************************
         *Function:clear
         *Purpose:To clear all the controls on the form.
         *Name: Varun Mehta
         *Change Date:15-Oct-2020
         *****************************************************************************************************
         */
         public void clear()
        {
            dgvInfo.ItemsSource = null;
            dgvKeyInfo.ItemsSource = null;
            dgvLicenseInfo.ItemsSource = null;
        }
        //Function Info.
        /*
         ****************************************************************************************************
         *Function:BrowseXmlFile
         *Purpose:To read the xml file from physical, once xml is validated.
         *Name: Varun Mehta
         *Change Date:15-Oct-2020
         *****************************************************************************************************
         */
        private void BrowseXmlFile(object sender, RoutedEventArgs e)
    {
        try
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.Filter = "XML Files (*.xml)|*.xml|All Files(*.*)|*.*";
            dlg.Multiselect = false;



                if (dlg.ShowDialog() == true)
                { txtXMLPath.Text = dlg.FileName; }



                if (clsValidateXML.compareXMLXSD(txtXMLPath.Text, "") == true)
                { BindXmlInfo(); }

                //if (compareXMLXSD(txtXMLPath.Text,"") == true)
                //{ BindXmlInfo(); }
                else
                { clear(); MessageBox.Show("This XML" + txtXMLPath.Text +" is Not acceptable.");  return; }
               


            }//
            catch (XmlException xe)
            {
                MessageBox.Show(xe.Message.ToString(), "BrowseXMLFile");
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "BrowseXMLFile");
            }
        }
        //Function Info.
        /*
         ****************************************************************************************************
         *Function:BindXmlInfo
         *Purpose:To read the xml file and fill the datagrids.
         *Name: Varun Mehta
         *Change Date:15-Oct-2020
         *****************************************************************************************************
         */
        public void BindXmlInfo()
        {
            XMLFileParser objParser = new XMLFileParser();
            XNodeList = objParser.ReadXmlFile(txtXMLPath.Text.Trim());

            XmlNodeList xOrderList = XNodeList["Order"];
            XmlNodeList xKeysList = XNodeList["Keys"];
            XmlNodeList xLicenseList = XNodeList["License"];

            // Order information and Binding
            DataTable dtOrderInfo = new DataTable();
            dtOrderInfo.Columns.Add("PO", typeof(string));
            dtOrderInfo.Columns.Add("DeliveryNumber", typeof(string));
            dtOrderInfo.Columns.Add("OrderNumber", typeof(string));
            dtOrderInfo.Columns.Add("CSN", typeof(string));
            dtOrderInfo.Columns.Add("CustomerName", typeof(string));
            dtOrderInfo.Columns.Add("LicenseCreateDate", typeof(string));
            foreach (XmlNode node in xOrderList)
            {
                //Fetch the Node and Attribute values.
                DataRow dtrow = dtOrderInfo.NewRow();
                dtrow["po"] = node.Attributes["po"].Value;
                dtrow["deliveryNumber"] = node.Attributes["deliveryNumber"].Value;
                dtrow["orderNumber"] = node.Attributes["orderNumber"].Value;
                dtrow["csn"] = node.Attributes["csn"].Value;
                dtrow["customerName"] = node.Attributes["customerName"].Value;
                dtrow["licenseCreateDate"] = node.Attributes["licenseCreateDate"].Value;
                dtOrderInfo.Rows.Add(dtrow);
            }
            //Bind order information data table to gridview
            dgvInfo.ItemsSource = new DataView(dtOrderInfo);

            // Save Order info to Dataset. This dataset can be further used to download XML
            clsGlobalClass.dtOrderInfo = dtOrderInfo;
            // Keys Information and its binding
            DataTable dtKeyInfo = new DataTable();
            dtKeyInfo.Columns.Add("AssetAction", typeof(string));
            dtKeyInfo.Columns.Add("ExpireDate", typeof(string));
            dtKeyInfo.Columns.Add("Name", typeof(string));

            foreach (XmlNode node in xKeysList)
            {
                //Fetch the Node and Attribute values.
                DataRow dtKeyrow = dtKeyInfo.NewRow();
                dtKeyrow["AssetAction"] = node.Attributes["assetAction"].Value;
                dtKeyrow["ExpireDate"] = node.Attributes["expirationDate"].Value;
                dtKeyrow["Name"] = node.Attributes["name"].Value;
                dtKeyInfo.Rows.Add(dtKeyrow);
            }

            //Bind key information data table to gridview
            dgvKeyInfo.ItemsSource = new DataView(dtKeyInfo);
            // Save Key info to Dataset. This dataset can be further used to download XML
            clsGlobalClass.dtKeyInfo = dtKeyInfo;
            //License Information and its binding
            DataTable dtLicenseInfo = new DataTable();
            dtLicenseInfo.Columns.Add("Name", typeof(string));
            dtLicenseInfo.Columns.Add("AssetId", typeof(string));
            dtLicenseInfo.Columns.Add("ActivationCode", typeof(string));
            dtLicenseInfo.Columns.Add("SmartName", typeof(string));
            dtLicenseInfo.Columns.Add("SmartId", typeof(string));

            foreach (XmlNode node in xLicenseList)
            {
                //Fetch the Node and Attribute values.
                DataRow dtLicenserow = dtLicenseInfo.NewRow();
                dtLicenserow["name"] = node.Attributes["name"].Value;
                dtLicenserow["assetId"] = node.Attributes["assetId"].Value;
                dtLicenserow["activationCode"] = node.Attributes["activationCode"].Value;
                dtLicenserow["smartName"] = node.Attributes["smartName"].Value;
                dtLicenserow["smartId"] = node.Attributes["smartId"].Value;
                dtLicenseInfo.Rows.Add(dtLicenserow);
            }
            //Bind License information data table to gridview
            dgvLicenseInfo.ItemsSource = new DataView(dtLicenseInfo);

            // Save Lic info to Dataset. This dataset can be further used to download XML
            clsGlobalClass.dtLicenseInfo = dtLicenseInfo;

        }
        //To create XML as output
        private void createXML_Output(object sender, RoutedEventArgs e)
        {
            var path = new Uri(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).LocalPath;
            if (clsDownloadXML.createXML_Output(txtXMLPath.Text) == true)
            { MessageBox.Show("XML Saved at " +path + "Information"); }
            else
                MessageBox.Show("XML failed to Saved at " + path, "Error");
        }
    


        //public void createXML_Output()
        //{
        //    //Create XML Document
        //    XmlDocument xmlDoc = new XmlDocument();
        //    // Create Master Node: Order
        //    XmlNode nodeOrder = xmlDoc.CreateElement("OrderStatusDetails");
        //    //Adding elements to Master Node : order
        //    //XmlAttribute attPO = xmlDoc.CreateAttribute("po");
        //    //attPO.Value = clsGlobalClass.dtOrderInfo.Rows[0]["po"].ToString();
        //    XmlAttribute attStatus = xmlDoc.CreateAttribute("Status");
        //    attStatus.Value = "In Progress";
        //    XmlAttribute attDeliveryNumber = xmlDoc.CreateAttribute("deliveryNumber");
        //    attDeliveryNumber.Value = clsGlobalClass.dtOrderInfo.Rows[0]["deliveryNumber"].ToString();
        //    XmlAttribute attDeliverydate = xmlDoc.CreateAttribute("deliveryDate");
        //    attDeliverydate.Value = "000000";
        //    //XmlAttribute attCSN = xmlDoc.CreateAttribute("csn");
        //    //attCSN.Value = clsGlobalClass.dtOrderInfo.Rows[0]["csn"].ToString();
        //    //XmlAttribute attCustomerName = xmlDoc.CreateAttribute("customerName");
        //    //attCustomerName.Value = clsGlobalClass.dtOrderInfo.Rows[0]["customerName"].ToString();
        //    //XmlAttribute attLicenseCreateDate = xmlDoc.CreateAttribute("licenseCreateDate");
        //    //attLicenseCreateDate.Value = clsGlobalClass.dtOrderInfo.Rows[0]["licenseCreateDate"].ToString();
        //    XmlAttribute attOrderNo = xmlDoc.CreateAttribute("OrderNumber");
        //    attOrderNo.Value = clsGlobalClass.dtOrderInfo.Rows[0]["orderNumber"].ToString();
        //    XmlAttribute namespace1 = xmlDoc.CreateAttribute("xmlns:xsd");
        //    namespace1.Value = "http://www.w3.org/2001/Lic.xsd";
        //    XmlAttribute namespace2 = xmlDoc.CreateAttribute("xmlns:xsi");
        //    namespace2.Value = "http://www.w3.org/2001/Lic.xsd";
        //    // Adding attributes to Master Node

        //    nodeOrder.Attributes.Append(namespace1);
        //    nodeOrder.Attributes.Append(namespace2);
        //   // nodeOrder.Attributes.Append(attLicenseCreateDate);
        //   // nodeOrder.Attributes.Append(attCSN);
        //   // nodeOrder.Attributes.Append(attCustomerName);
        //    nodeOrder.Attributes.Append(attDeliveryNumber);
        //    nodeOrder.Attributes.Append(attStatus);
        //    nodeOrder.Attributes.Append(attDeliverydate);
          
        //     nodeOrder.Attributes.Append(attOrderNo);
        //    xmlDoc.AppendChild(nodeOrder);
        //    //    adding Node to master node
        //    XmlNode nodeKeys = xmlDoc.CreateElement("KeyStatusDetails");

        //    XmlAttribute attdevideID = xmlDoc.CreateAttribute("deviceID");
        //    attdevideID.Value = "Device ID";// clsGlobalClass.dtKeyInfo.Rows[0]["activationCode"].ToString();
        //    XmlAttribute attAction = xmlDoc.CreateAttribute("action");
        //    attAction.Value = "Action";//clsGlobalClass.dtKeyInfo.Rows[0]["activationCode"].ToString();
        //    XmlAttribute attNameKeys = xmlDoc.CreateAttribute("name");
        //    attNameKeys.Value = "Name";// clsGlobalClass.dtKeyInfo.Rows[0]["activationCode"].ToString();
        //    nodeKeys.Attributes.Append(attdevideID);
        //    nodeKeys.Attributes.Append(attAction);
        //    nodeKeys.Attributes.Append(attNameKeys);
        //    //    loop N Times:
        //    //N: No of licensee
        //    for (int i = 0; i < clsGlobalClass.dtKeyInfo.Rows.Count; i++)
        //    {
        //        XmlNode nodeKey = xmlDoc.CreateElement(@"ActivationCodes");
        //        nodeKeys.AppendChild(nodeKey);
        //        XmlNode nodeLic = xmlDoc.CreateElement("ActivationCode");
        //        nodeLic.InnerText = clsGlobalClass.dtLicenseInfo.Rows[i]["activationCode"].ToString();
        //        nodeKey.AppendChild(nodeLic);
        //    }
        //    nodeOrder.AppendChild(nodeKeys);
        //    //Add new Node: CreatedImageFile
        //    XmlNode nodeCreatedImageFile = xmlDoc.CreateElement(@"CreatedImageFile");
        //    XmlAttribute attDescription = xmlDoc.CreateAttribute("Password");
        //    attDescription.Value = "Password";
        //    XmlAttribute attFileNAme = xmlDoc.CreateAttribute("FileNAme");
        //    attFileNAme.Value = "File Name";
        //    //Adding Attributes to node and append
        //    nodeCreatedImageFile.Attributes.Append(attDescription);
        //    nodeCreatedImageFile.Attributes.Append(attFileNAme);
        //    nodeOrder.AppendChild(nodeCreatedImageFile);
        //    //Add new Node: UpdatedSFDC
        //    XmlNode nodeUpdatedSFDC = xmlDoc.CreateElement(@"UpdatedSFDC");
        //    XmlAttribute attError = xmlDoc.CreateAttribute("error");
        //    attError.Value = "Object reference not set to an instance of an object.";
        //    //Adding Attributes to node and append
        //    nodeUpdatedSFDC.Attributes.Append(attError);
        //    nodeOrder.AppendChild(nodeUpdatedSFDC);
        //    //Add new Node: KeyStatus
        //    XmlNode nodeKeyStatus = xmlDoc.CreateElement(@"KeyStatus");
        //    XmlAttribute attKeyStatus = xmlDoc.CreateAttribute("Description");
        //    //Adding Attributes to node and append
        //    attKeyStatus.Value = "Error occured updating Sales Force for the keyless.";
        //    nodeKeyStatus.Attributes.Append(attKeyStatus);
        //    nodeOrder.AppendChild(nodeKeyStatus);
        //    //Save XML Output to BuildOutput. 
        //    xmlDoc.Save(System.IO.Path.GetFileName(txtXMLPath.Text));
        //}

        private void getAllOrders(object sender, RoutedEventArgs e)
        {
            frmAllOrders objAllOrders = new frmAllOrders();
            
            objAllOrders.Show();
          //  this.Hide();
        }
      
    }
}