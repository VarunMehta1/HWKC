﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace HardwareKeyCreator
{
  
    /*
     
     Static Class

     
     
     */
  public static  class clsGlobalClass
    {
        //Static dataset to hold value for Orders
        public static DataTable dtOrderInfo;
        //Static dataset to hold value for KeyDetails
        public static DataTable dtKeyInfo;
        //Static dataset to hold value for LicDetails
        public static DataTable dtLicenseInfo;
        //Static dataset to hold value for order details 
        public static DataTable dtAllOrderInfo;
        //Static XML Schema to check Validation on XML
        public static string _xmlSchemaToValidate = "";
    }

   
}
