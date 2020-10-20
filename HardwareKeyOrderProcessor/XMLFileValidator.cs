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
    public   class XMLFIleValidator
    {
        public   bool ResultOutput = false;

        //Function Info.
        /*
         ****************************************************************************************************
         *Function:XMLValidate
         *Purpose:To compare the uploaded XML, with XML Schema Set.If fails it reurn false. 
         *Name: Varun Mehta
         *Change Date:15-Oct-2020
         *****************************************************************************************************
         */
        public Boolean XMLValidate(string XML, string XSD)
        {

            try
            {
                var XSDPath = new Uri(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).LocalPath;

                XmlSchemaSet XMLSchema = new XmlSchemaSet();
                XMLSchema.Add("", XSDPath + "\\HardwareKeyOrders.xsd");

                XmlReader XMLReader = XmlReader.Create(XML);
                XDocument XMLDoc = XDocument.Load(XMLReader);

                XMLDoc.Validate(XMLSchema, ValidationEventHandler);

                ResultOutput = true;
            }
            catch (Exception ee)
            {
               
                return false; ;
            }
            return ResultOutput;
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
        private   void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            try
            {
                switch (e.Severity)
                {
                    case XmlSeverityType.Error:
                        throw new Exception((e.Exception.Message.ToString()));
                        
                        break;
                    case XmlSeverityType.Warning:
                        throw new Exception((e.Exception.Message.ToString()));
                      
                        break;
                }
            }
            catch (Exception ee) { throw new Exception(ee.Message.ToString()); }


        }
    }
}
