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

namespace HardwareKeyCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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



                DataSet dataSet = new DataSet();
                dataSet.ReadXml(txtXMLPath.Text, XmlReadMode.InferSchema);
                dgvInfo.ItemsSource = new DataView(dataSet.Tables[0]);
                XDocument doc = XDocument.Load(txtXMLPath.Text);
                //var allElements = doc.Descendants();var matchingElements = doc.Descendants() .Where(x => x.Attribute("Key Name") != null);
                read();
            }
            catch (XmlException xe)
            {
                MessageBox.Show(xe.Message.ToString(), "BrowseXMLFile");
                return;
            }
            catch (Exception ee)
            { MessageBox.Show(ee.Message.ToString(), "BrowseXMLFile"); }


        }
        public void read()
        {
            XDocument myxml = XDocument.Load(txtXMLPath.Text);
            string strFilename = txtXMLPath.Text;
            XmlDocument doc = new XmlDocument();
            doc.Load(strFilename);

            XmlNodeList nodeList = doc.SelectNodes("/Order/Keys/Key");
            DataTable dt = new DataTable();
            dt.Columns.Add("Key", typeof(string));
            dt.Columns.Add("ExpireDate", typeof(string));
            dt.Columns.Add("Name", typeof(string));

            foreach (XmlNode node in nodeList)
            {
                //Fetch the Node and Attribute values.
                DataRow dtrow = dt.NewRow();
                dtrow["Key"] = node.Attributes["assetAction"].Value;
                dtrow["ExpireDate"] = node.Attributes["expirationDate"].Value;
                dtrow["Name"] = node.Attributes["name"].Value;
                dt.Rows.Add(dtrow);
            }
            dgvKey.ItemsSource = new DataView(dt);
            nodeList = doc.SelectNodes("/Order/Keys/Key/Licenses/License");
            //Loop through the selected Nodes.

            dt = new DataTable();
            dt.Columns.Add("name", typeof(string));
            dt.Columns.Add("assetId", typeof(string));
            dt.Columns.Add("activationCode", typeof(string));
            dt.Columns.Add("smartName", typeof(string));
            dt.Columns.Add("smartId", typeof(string));
            foreach (XmlNode node in nodeList)
            {
                //Fetch the Node and Attribute values.
                DataRow dtrow = dt.NewRow();
                dtrow["name"] = node.Attributes["name"].Value;
                dtrow["assetId"] = node.Attributes["assetId"].Value;
                dtrow["activationCode"] = node.Attributes["activationCode"].Value;
                dtrow["smartName"] = node.Attributes["smartName"].Value;
                dtrow["smartId"] = node.Attributes["smartId"].Value;
                dt.Rows.Add(dtrow);

            }
            dgvLic.ItemsSource = new DataView(dt);
        }

    }
}
