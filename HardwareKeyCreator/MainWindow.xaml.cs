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
namespace HardwareKeyCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Dictionary Object which will return key-value list from XMLFileParser
        /// </summary>

        Dictionary<string, XmlNodeList> XNodeList;



        public MainWindow()
        {
            InitializeComponent();
        }



        private void BrowseXmlFile(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.CheckFileExists = true;
                dlg.Filter = "XML Files (*.xml)|*.xml|All Files(*.*)|*.*";
                dlg.Multiselect = false;



                if (dlg.ShowDialog() == true) { txtXMLPath.Text = dlg.FileName; }


               

                BindXmlInfo();
            }
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



        /// <summary>
        /// Call to ReadXmlFile() function of XMLFileParser
        /// Return Dictionary objects which has file nodes information
        /// Iterate over each of the nodelist object and bind it to gridview
        /// </summary>

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





        private void createXML_Output(object sender, RoutedEventArgs e)
        {
           
            DataSet ds = new DataSet();
            DataTable dtXMLValue = new DataTable();
            System.IO.StringWriter writer = new System.IO.StringWriter();
            ////dtReadFromGlobal.WriteXml(writer, XmlWriteMode.WriteSchema, false);

            //DataSet dataSet = new DataSet("ds");
            //DataTable table1 = dataSet.Tables.Add("Order");
            //table1 = clsGlobalClass.dtOrderInfo;
            //DataTable table2 = dataSet.Tables.Add("Student");

            //table2 = clsGlobalClass.dtOrderInfo;





            ////ds.Tables.Add(clsGlobalClass.dtOrderInfo);
            ////ds.Tables[0].TableName = "Order";

            ////ds.Tables.Add(clsGlobalClass.dtKeyInfo);
            ////ds.Tables[1].TableName = "Key";

            ////ds.Tables.Add(clsGlobalClass.dtLicenseInfo);
            ////ds.Tables[2].TableName = "Lic";

            ////ds.WriteXml("order.xml");
      



            //  XElement xOrder = new XElement("Order", "Adventure Works");// new XElement("Order", new XElement("Cost", 324.50));
            XElement contacts =
      new XElement("Order",
          new XElement("Keys",
              new XElement("Name", "Patrick Hines"),
              new XElement("Phone", "206-555-0144"),
              new XElement("Address",
                  new XElement("Street1", "123 Main St"),
                  new XElement("City", "Mercer Island"),
                  new XElement("State", "WA"),
                  new XElement("Postal", "68042")
              )
          )
      );


            MessageBox.Show(contacts.ToString());
            test();

        }


        public void test()
        {
            //Create XML Document
            XmlDocument xmlDoc = new XmlDocument();
            // Create Master Node: Order
            XmlNode Order = xmlDoc.CreateElement("Order");
            //Adding elements to Master Node : order
            XmlAttribute po = xmlDoc.CreateAttribute("po");
            po.Value = clsGlobalClass.dtOrderInfo.Rows[0]["po"].ToString();
            XmlAttribute deliveryNumber = xmlDoc.CreateAttribute("deliveryNumber");
            deliveryNumber.Value = clsGlobalClass.dtOrderInfo.Rows[0]["deliveryNumber"].ToString();
            XmlAttribute csn = xmlDoc.CreateAttribute("csn");
            csn.Value = clsGlobalClass.dtOrderInfo.Rows[0]["csn"].ToString();
            XmlAttribute customerName = xmlDoc.CreateAttribute("customerName");
            customerName.Value = clsGlobalClass.dtOrderInfo.Rows[0]["customerName"].ToString();
            XmlAttribute licenseCreateDate = xmlDoc.CreateAttribute("licenseCreateDate");
            licenseCreateDate.Value = clsGlobalClass.dtOrderInfo.Rows[0]["licenseCreateDate"].ToString();

            // Adding attributes to Master Node
            Order.Attributes.Append(po);
            Order.Attributes.Append(deliveryNumber);
            Order.Attributes.Append(csn);
            Order.Attributes.Append(customerName);
            Order.Attributes.Append(licenseCreateDate);
            xmlDoc.AppendChild(Order);


            // adding Node to master node

            XmlNode he = xmlDoc.CreateElement("Keys");
         
            // loop N Times:
            // N: No of licensee
            for (int i = 0; i < clsGlobalClass.dtKeyInfo.Rows.Count; i++)
            {
               
                XmlNode Key = xmlDoc.CreateElement(@"Key");

                // Adding attributes to Key Node
                XmlAttribute assetAction = xmlDoc.CreateAttribute("assetAction");
                assetAction.Value = clsGlobalClass.dtKeyInfo.Rows[i]["assetAction"].ToString();

                XmlAttribute expirationDate = xmlDoc.CreateAttribute("expirationDate");
                expirationDate.Value = clsGlobalClass.dtKeyInfo.Rows[i]["ExpireDate"].ToString();

                XmlAttribute name = xmlDoc.CreateAttribute("assetAction");
                name.Value = clsGlobalClass.dtKeyInfo.Rows[i]["assetAction"].ToString();

                Key.Attributes.Append(assetAction);
                Key.Attributes.Append(expirationDate);
                Key.Attributes.Append(name);
                he.AppendChild(Key);

                XmlNode Lic = xmlDoc.CreateElement("Licenses");
                // declaring  attributes to Lic Node
                XmlAttribute lic_name = xmlDoc.CreateAttribute("name");
                lic_name.Value = clsGlobalClass.dtLicenseInfo.Rows[i]["name"].ToString();

                XmlAttribute assetId = xmlDoc.CreateAttribute("assetId");
                assetId.Value = clsGlobalClass.dtLicenseInfo.Rows[i]["assetId"].ToString();

                XmlAttribute activationCode = xmlDoc.CreateAttribute("activationCode");
                activationCode.Value = clsGlobalClass.dtLicenseInfo.Rows[i]["activationCode"].ToString();


                XmlAttribute smartName = xmlDoc.CreateAttribute("smartName");
                smartName.Value = clsGlobalClass.dtLicenseInfo.Rows[i]["smartName"].ToString();

                XmlAttribute smartID = xmlDoc.CreateAttribute("smartId");
                smartID.Value = clsGlobalClass.dtLicenseInfo.Rows[i]["smartId"].ToString();

                // adding attributes to Lic Node
                Lic.Attributes.Append(lic_name);
                Lic.Attributes.Append(assetId);
                Lic.Attributes.Append(activationCode);
                Lic.Attributes.Append(smartName);
                Lic.Attributes.Append(smartName);
                Key.AppendChild(Lic);
            }
            Order.AppendChild(he);
            xmlDoc.Save(System.IO.Path.GetFileName(txtXMLPath.Text));



        }
    }
}