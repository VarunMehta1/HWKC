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



namespace HardwareKeyOrderProcessor
{
    public static class clsValidateXML
    {
        public static bool _result = false;

        //Function Info.
        /*
         ****************************************************************************************************
         *Function:compareXMLXSD
         *Purpose:To compare the uploaded XML, with XML Schema Set.If fails it reurn. 
         *Name: Varun Mehta
         *Change Date:15-Oct-2020
         *****************************************************************************************************
         */
        public static Boolean compareXMLXSD(string _xml, string _xsd)
        {

            try
            {
                var path = new Uri(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).LocalPath;
                XmlSchemaSet schema = new XmlSchemaSet();
                schema.Add("", path + "\\HardwareKeyOrders.xsd");
                XmlReader rd = XmlReader.Create(_xml);// (path + "\\input.xml");
                XDocument doc = XDocument.Load(rd);
                doc.Validate(schema, ValidationEventHandler);


                //doc.Validate(schema, (o, e) =>
              // {/
                    //Console.WriteLine("{0}", e.Message);
                    //throw new Exception(e.Message.ToString());
                  ////  errors = true;
                //});



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
        private static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            try
            {
                switch (e.Severity)
                {
                    case XmlSeverityType.Error:
                        throw new Exception((e.Exception.Message.ToString()));
                        //  Console.WriteLine("Error: {0}", e.Message);
                        break;
                    case XmlSeverityType.Warning:
                        throw new Exception((e.Exception.Message.ToString()));
                        //    Console.WriteLine("Warning {0}", e.Message);
                        break;
                }
            }
            catch (Exception ee) { throw new Exception(ee.Message.ToString()); }






            //XmlSeverityType type = XmlSeverityType.Warning;
            //if (Enum.TryParse<XmlSeverityType>("Error", out type))
            //{
            //    if (type == XmlSeverityType.Error)


            //    {
            //        throw new Exception("Validator Error: Value provided doesn't match with ");  //throw new Exception(e.Message);
            //    }

         //   }

        }
    }
}
