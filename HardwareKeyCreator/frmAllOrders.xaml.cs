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

namespace HardwareKeyCreator
{
    /// <summary>
    /// Interaction logic for frmAllOrders.xaml
    /// </summary>
    public partial class frmAllOrders : Window
    {
        public frmAllOrders()
        {
            InitializeComponent();
        }
        Dictionary<string, XmlNodeList> XNodeList;

        public void loadOrders()
        {
            if (Directory.Exists(@"C:\M5Files\Input"))
            {
                XMLFileParser objParser = new XMLFileParser();
                string[] fileEntries = Directory.GetFiles(@"C:\M5Files\Input");


                objParser = new XMLFileParser();





                //////
                var fileCount = (from file in Directory.EnumerateFiles(@"C:\M5Files\Input", "*.xml", SearchOption.AllDirectories)
                                 select file).Count();
                lblXMLCount.Content = fileCount.ToString();
                System.Data.DataTable dtShowOrderInfoAll = new System.Data.DataTable();
                dtShowOrderInfoAll.Columns.Add("OrderID", typeof(string));
                dtShowOrderInfoAll.Columns.Add("Customer Name", typeof(string));
                dtShowOrderInfoAll.Columns.Add("CSN", typeof(string));
                dtShowOrderInfoAll.Columns.Add("No of Keys", typeof(Int32));
                dtShowOrderInfoAll.Columns.Add("Status", typeof(string));
                dtShowOrderInfoAll.Columns.Add("File Name", typeof(string));
                ////////
                for (int _fileCount = 0; _fileCount < fileCount; _fileCount++)
                {
                    //Fetch the Node and Attribute values.

                    DataRow dtrow = dtShowOrderInfoAll.NewRow();
                    dtrow["File Name"] =  fileEntries[_fileCount];

                    XNodeList = objParser.ReadXmlFile(dtrow["File Name"].ToString());
                    XmlNodeList xOrderList = XNodeList["Order"];
                    XmlNodeList xKeysList = XNodeList["Keys"];

                    foreach (XmlNode node in xOrderList)
                    {


                        dtrow["OrderID"] = node.Attributes["orderNumber"].Value;
                        dtrow["csn"] = node.Attributes["csn"].Value;
                        dtrow["customer Name"] = node.Attributes["customerName"].Value;
                        dtrow["status"] = "Not Started";
                        dtrow["File Name"] = System.IO.Path.GetFileName(fileEntries[_fileCount])  ;

                        foreach (XmlNode keyNode in xKeysList)
                        {
                            dtrow["No of Keys"] = xKeysList.Count.ToString();
                        }
                        dtShowOrderInfoAll.Rows.Add(dtrow);
                    }
                    this.dgvOrders.ItemsSource = new DataView(dtShowOrderInfoAll);
                }



            }
            else
            {
                MessageBox.Show("No XML Files Found.");
                return;
            }
        }
            private void getInfoOrder(string _fileName )
        {
          




        }
        private void loadOrders(object sender, RoutedEventArgs e)
        {
            //Load Top 10 files, order by latest
            loadOrders();
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
