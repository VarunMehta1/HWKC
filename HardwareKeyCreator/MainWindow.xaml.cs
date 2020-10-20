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
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }
        public bool ResultOutput;

        //Function Info.
         
        
       
        
        //Function Info.
        /*
         ****************************************************************************************************
         *Function:clear
         *Purpose:To clear all the controls on the form.
         *Name: Varun Mehta
         *Change Date:15-Oct-2020
         *****************************************************************************************************
         */
         public void ClearControls()
        {
            //XMLDataTable ObjXMlDataTable = new XMLDataTable();
            //ObjXMlDataTable.dtAllOrderInfo = new DataTable();
            //ObjXMlDataTable.dtKeyInfo = new DataTable();
            //ObjXMlDataTable.dtLicenseInfo = new DataTable();
            //ObjXMlDataTable.dtOrderInfo = new DataTable();

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
            Microsoft.Win32.OpenFileDialog OpenFileDialog = new Microsoft.Win32.OpenFileDialog();

                OpenFileDialog.CheckFileExists = true;
                OpenFileDialog.Filter = "XML Files (*.xml)|*.xml|All Files(*.*)|*.*";
                OpenFileDialog.Multiselect = false;
              
                if (OpenFileDialog.ShowDialog() == true)
                { txtXMLPath.Text = OpenFileDialog.FileName; }
                XMLFIleValidator ObjectValidateXMLClass = new XMLFIleValidator();
                if (ObjectValidateXMLClass.XMLValidate(txtXMLPath.Text, "") == true)
                { BindXmlInfo(); }

                else
                { ClearControls(); MessageBox.Show("This XML" + txtXMLPath.Text +" is Not acceptable.");  return; }
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
            DataTable DataTableOrder = new DataTable();

            DataTableOrder.Columns.Add("PO", typeof(string));
            DataTableOrder.Columns.Add("DeliveryNumber", typeof(string));
            DataTableOrder.Columns.Add("OrderNumber", typeof(string));
            DataTableOrder.Columns.Add("CSN", typeof(string));
            DataTableOrder.Columns.Add("CustomerName", typeof(string));
            DataTableOrder.Columns.Add("LicenseCreateDate", typeof(string));

            foreach (XmlNode node in xOrderList)
            {
                //Fetch the Node and Attribute values.
                DataRow DataRowOrder = DataTableOrder.NewRow();

                DataRowOrder["PO"] = node.Attributes["po"].Value;
                DataRowOrder["DeliveryNumber"] = node.Attributes["deliveryNumber"].Value;

                DataRowOrder["OrderNumber"] = node.Attributes["orderNumber"].Value;
                DataRowOrder["CSN"] = node.Attributes["csn"].Value;

                DataRowOrder["CustomerName"] = node.Attributes["customerName"].Value;
                DataRowOrder["LicenseCreateDate"] = node.Attributes["licenseCreateDate"].Value;

                DataTableOrder.Rows.Add(DataRowOrder);
            }
            //Bind order information data table to gridview
            dgvInfo.ItemsSource = new DataView(DataTableOrder);

            // Save Order info to Dataset. This dataset can be further used to download XML
            
            XMLDataTable.DataTableOrders = DataTableOrder;
            // Keys Information and its binding
            DataTable DataTableKeys = new DataTable();

            DataTableKeys.Columns.Add("AssetAction", typeof(string));
            DataTableKeys.Columns.Add("ExpireDate", typeof(string));
            DataTableKeys.Columns.Add("Name", typeof(string));

            foreach (XmlNode node in xKeysList)
            {
                //Fetch the Node and Attribute values.
                DataRow DataRowKey = DataTableKeys.NewRow();

                DataRowKey["AssetAction"] = node.Attributes["assetAction"].Value;
                DataRowKey["ExpireDate"] = node.Attributes["expirationDate"].Value;
                DataRowKey["Name"] = node.Attributes["name"].Value;

                DataTableKeys.Rows.Add(DataRowKey);
            }

            //Bind key information data table to gridview
            dgvKeyInfo.ItemsSource = new DataView(DataTableKeys);

            // Save Key info to Dataset. This dataset can be further used to download XML

            XMLDataTable.DataTableKeys = DataTableKeys;
        
            //License Information and its binding
            DataTable DataTableKeyLicenses = new DataTable();

            DataTableKeyLicenses.Columns.Add("Name", typeof(string));
            DataTableKeyLicenses.Columns.Add("AssetId", typeof(string));
            DataTableKeyLicenses.Columns.Add("ActivationCode", typeof(string));
            DataTableKeyLicenses.Columns.Add("SmartName", typeof(string));
            DataTableKeyLicenses.Columns.Add("SmartId", typeof(string));

            foreach (XmlNode node in xLicenseList)
            {
                //Fetch the Node and Attribute values.
                DataRow DataRowLic = DataTableKeyLicenses.NewRow();

                DataRowLic["Name"] = node.Attributes["name"].Value;
                DataRowLic["AssetId"] = node.Attributes["assetId"].Value;
                DataRowLic["ActivationCode"] = node.Attributes["activationCode"].Value;
                DataRowLic["SmartName"] = node.Attributes["smartName"].Value;
                DataRowLic["SmartId"] = node.Attributes["smartId"].Value;

                DataTableKeyLicenses.Rows.Add(DataRowLic);
            }
            //Bind License information data table to gridview
            dgvLicenseInfo.ItemsSource = new DataView(DataTableKeyLicenses);

            // Save Lic info to Dataset. This dataset can be further used to download XML
           
            XMLDataTable.DataTableLic = DataTableKeyLicenses;

        }
        //To create XML as output
        private void CreateXMLOutput(object sender, RoutedEventArgs e)
        {
            var path = new Uri(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).LocalPath;

            XMLOutputCreator XmlOutput = new XMLOutputCreator();

            if(XmlOutput.CreateXMLOutput(txtXMLPath.Text) == true)            
            { MessageBox.Show("XML Saved at " +path + "Information");
                XMLDataTable.DataTableAllOrders = null;
                XMLDataTable.DataTableKeys = null;
                XMLDataTable.DataTableLic = null;
                XMLDataTable.DataTableOrders = null;
            }
            else MessageBox.Show("XML failed to Saved at " + path, "Error");

        }
           private void GetAllOrders(object sender, RoutedEventArgs e)
        {
            this.Hide();
            frmAllOrders ObjAllOrders = new frmAllOrders();
            ObjAllOrders.Show();
        
        }
      
    }
}