using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HardwareKeyOrderProcessor
{
    public interface IXmlFileParser
    {
        Dictionary<string, XmlNodeList> ReadXmlFile(string fileName);
    }


    
}
