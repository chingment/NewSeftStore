using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Lumos
{
    public class XmlUtil
    {
        public static T DeserializeToObject<T>(string xml)
        {
            T myObject;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringReader reader = new StringReader(xml);
            myObject = (T)serializer.Deserialize(reader);
            reader.Close();
            return myObject;
        }

        public static SortedDictionary<string, object> ToDictionary(string xml)
        {
            SortedDictionary<string, object> m_values = new SortedDictionary<string, object>();

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);
                XmlNode xmlNode = xmlDoc.FirstChild;//获取到根节点<xml>
                XmlNodeList nodes = xmlNode.ChildNodes;
                foreach (XmlNode xn in nodes)
                {
                    XmlElement xe = (XmlElement)xn;
                    m_values[xe.Name] = xe.InnerText;//获取xml的键值对到WxPayData内部的数据中
                }
            }
            catch(Exception ex)
            {

            }
            return m_values;
        }
    }
}
