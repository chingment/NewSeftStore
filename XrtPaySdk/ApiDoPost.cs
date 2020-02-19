using Lumos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XrtPaySdk
{
    public class ApiDoPost
    {
        private string responseString = null;

        public string GetSeviceUrl()
        {
            return System.Configuration.ConfigurationManager.AppSettings["custom:XrtPayServerUrl"];
        }
        public ApiDoPost()
        {

        }

        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }

        public string ConvertXml(Dictionary<string, string> m_values)
        {
            string xml = "<xml>";
            foreach (KeyValuePair<string, string> pair in m_values)
            {
                if (!string.IsNullOrEmpty(pair.Value))
                {
                    xml += "<" + pair.Key + ">" + "<![CDATA[" + pair.Value + "]]></" + pair.Key + ">";
                }
            }
            xml += "</xml>";
            return xml;
        }

        public static T DeserializeToObject<T>(string xml)
        {
            T myObject;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringReader reader = new StringReader(xml);
            myObject = (T)serializer.Deserialize(reader);
            reader.Close();
            return myObject;
        }

        public T DoPost<T>(IApiPostRequest<T> request)
        {

            string str_PostData = ConvertXml(request.PostData);

            string requestUrl = GetSeviceUrl();

            WebUtils webUtils = new WebUtils();
            LogUtil.Info(string.Format("XrtPaySdk-PostUrl->{0}", requestUrl));
            LogUtil.Info(string.Format("XrtPaySdk-PostData->{0}", str_PostData));
            string requestResult = webUtils.DoPost(requestUrl, str_PostData);

            this.responseString = requestResult;

            LogUtil.Info(string.Format("XrtPaySdk-PostResult->{0}", requestResult));


            T rsp = DeserializeToObject<T>(requestResult);

            return rsp;
        }
    }
}
