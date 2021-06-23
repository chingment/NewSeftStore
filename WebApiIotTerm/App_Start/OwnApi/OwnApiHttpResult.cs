using Lumos;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;


namespace WebApiIotTerm
{
    public class OwnApiHttpResult : IResult2
    {
        private string _code = "";
        private string _msg = "";
        private object _data = null;

        public OwnApiHttpResult()
        {

        }

        public OwnApiHttpResult(string code, string msg, object data = null)
        {
            _code = code;
            _msg = msg;
            _data = data;
        }

        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
            }
        }

        public string Msg
        {
            get
            {
                return _msg;
            }
            set
            {
                _msg = value;
            }
        }

        public object Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        public override string ToString()
        {
            StringBuilder json = new StringBuilder();
            json.Append("{");
            json.Append("\"code\":" + _code + ",");
            json.Append("\"msg\":" + JsonConvert.SerializeObject(_msg) + "");

            if (_data != null)
            {
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                jsonSerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat;
                jsonSerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";


                json.Append(",\"data\":" + JsonConvert.SerializeObject(_data, jsonSerializerSettings) + "");
            }

            json.Append("}");

            return json.ToString();
        }
    }
}