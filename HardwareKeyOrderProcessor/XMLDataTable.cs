using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace HardwareKeyCreator
{
       /*
         ****************************************************************************************************
         *Class:clsGlobalClass
         *Purpose:To read XML data to datatable , while XML uploading.Download XML as output with desired data.
         *Name: Varun Mehta
         *Change Date:15-Oct-2020
         *****************************************************************************************************
         */
  public static   class XMLDataTable
    {
        //Static dataset to hold value for Orders
        public static DataTable DataTableOrders;
        //Static dataset to hold value for KeyDetails
        public static DataTable DataTableKeys;
        //Static dataset to hold value for LicDetails
        public static DataTable DataTableLic;
        //Static dataset to hold value for order details 
        public static DataTable DataTableAllOrders;
        //Static XML Schema to check Validation on XML
        public static string _xmlSchemaToValidate = "";
    }

   
}
