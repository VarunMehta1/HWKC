using System;
using System.Text;

namespace HardwareKeyCreationTool
{
    internal class Utility
    {
        public string Base64Decode(string encodedString)
        {
            try
            {
                byte[] data = Convert.FromBase64String(encodedString);
                string decodedString = ASCIIEncoding.ASCII.GetString(data);
                return decodedString;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
