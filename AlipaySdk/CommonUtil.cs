using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace MyAlipaySdk
{
    public static class CommonUtil
    {
        public static SortedDictionary<string, string> FormStringToDictionary(string str)
        {
            SortedDictionary<string, string> m_values = new SortedDictionary<string, string>();

            string[] arr_item = str.Split('&');

            foreach (var item in arr_item)
            {
                string[] keyValue = item.Split('=');
                if (keyValue.Length > 0)
                {

                    string value = "";
                    if (keyValue.Length > 1)
                    {
                        value = keyValue[1];
                        if (string.IsNullOrEmpty(value))
                        {
                            value = HttpUtility.UrlDecode(value);
                        }
                    }
                    m_values.Add(keyValue[0], value);
                }
            }
            //XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.XmlResolver = null;
            //xmlDoc.LoadXml(xml);
            //XmlNode xmlNode = xmlDoc.FirstChild;//获取到根节点<xml>
            //XmlNodeList nodes = xmlNode.ChildNodes;
            //foreach (XmlNode xn in nodes)
            //{
            //    XmlElement xe = (XmlElement)xn;
            //    m_values[xe.Name] = xe.InnerText;//获取xml的键值对到WxPayData内部的数据中
            //}

            //if (m_values["return_code"] != null)
            //{
            //    if (m_values["return_code"].ToString() != "SUCCESS")
            //    {
            //        return m_values;
            //    }
            //}

            return m_values;
        }
    }
}
