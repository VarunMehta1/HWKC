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
using System.Windows.Shapes;
using System.IO;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using HardwareKeyOrderProcessor;
using System.Windows.Navigation;
using MahApps.Metro.Controls;

namespace HardwareKeyCreator
{
    /// <summary>
    /// Interaction logic for frmAllOrders.xaml
    /// </summary>
    public partial class frmAllOrders : MetroWindow
    {
        
        public frmAllOrders()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }
        Dictionary<string, XmlNodeList> XNodeList;
       
        /*
        ****************************************************************************************************
        *Function:loadOrders()
        *Purpose:To read all XML from "C:\M5Files\Input" and load to grid..
        *Name: Varun Mehta
        *Change Date:15-Oct-2020
        *****************************************************************************************************
        */
        public void XmlLoadOrders()
        {
            if (Directory.Exists(@"C:\M5Files\Input"))
            {
                XMLFileParser objParser = new XMLFileParser();

                string[] fileEntries = Directory.GetFiles(@"C:\M5Files\Input");

             
                var fileCount = (from file in Directory.EnumerateFiles(@"C:\M5Files\Input", "*.xml", SearchOption.AllDirectories)
                                 select file).Count();

                lblXMLCount.Content = fileCount.ToString();

                System.Data.DataTable DataTableShowOrderInfoAll = new System.Data.DataTable();
                DataTableShowOrderInfoAll.Columns.Add("OrderID", typeof(string));
                DataTableShowOrderInfoAll.Columns.Add("Customer Name", typeof(string));
                DataTableShowOrderInfoAll.Columns.Add("CSN", typeof(string));
                DataTableShowOrderInfoAll.Columns.Add("No of Keys", typeof(Int32));
                DataTableShowOrderInfoAll.Columns.Add("Status", typeof(string));
                DataTableShowOrderInfoAll.Columns.Add("File Name", typeof(string));


                for (int _fileCount = 0; _fileCount < fileCount; _fileCount++)
                {
                    
                    DataRow DataRowOrder = DataTableShowOrderInfoAll.NewRow();
                    DataRowOrder["File Name"] =  fileEntries[_fileCount];

                    XNodeList = objParser.ReadXmlFile(DataRowOrder["File Name"].ToString());
                    XmlNodeList xOrderList = XNodeList["Order"];
                    XmlNodeList xKeysList = XNodeList["Keys"];

                    foreach (XmlNode XMLOrderNode in xOrderList)
                    {
                        DataRowOrder["OrderID"] = XMLOrderNode.Attributes["orderNumber"].Value;
                        DataRowOrder["csn"] = XMLOrderNode.Attributes["csn"].Value;
                        DataRowOrder["customer Name"] = XMLOrderNode.Attributes["customerName"].Value;
                        DataRowOrder["status"] = "Not Started";
                        DataRowOrder["File Name"] = System.IO.Path.GetFileName(fileEntries[_fileCount])  ;

                        foreach (XmlNode XMLKeyNode in xKeysList)
                        {
                            DataRowOrder["No of Keys"] = xKeysList.Count.ToString();
                        }
                        DataTableShowOrderInfoAll.Rows.Add(DataRowOrder);
                    }
                    this.dgvOrders.ItemsSource = new DataView(DataTableShowOrderInfoAll);
                }



            }
            else
            {
                MessageBox.Show("No XML Files Found.");
                return;
            }
        }
          
        private void XmlLoadOrders(object sender, RoutedEventArgs e)
        {
            //Load Top 10 files, order by latest
            XmlLoadOrders();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow _main = new MainWindow();
            _main.Show();
            this.Hide();
        }
    }

  
}
