using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YsyInscarSdk
{
    public class QueryCarInfoApi : IApiGetRequest<QueryCarInfoResult>
    {
        private string _license_plate = "";
        private string _vehicle_frame_no = "";
        private string _engine_number = "";
        private string _transaction_id = "";
        public string ApiName
        {
            get
            {
                return "idmp-product-car/queryRenewalInfo";
            }
        }

        public string UserCode
        {
            get;set;
        }
        public string Key
        {
            get; set;
        }

        public QueryCarInfoApi(string license_plate)
        {
            _license_plate = license_plate;
        }

        public static string GetMD5(Dictionary<string, string> parameters,string key)
        {
  
            var dicSort = from objDic in parameters orderby objDic.Key ascending select objDic;

            StringBuilder sb = new StringBuilder();

            foreach (var dic in dicSort)
            {

                sb.Append(dic.Key + "=" + dic.Value + "&");
            }

            string user_key = key;

            sb.Append("key=" + user_key);

            byte[] result = Encoding.UTF8.GetBytes(sb.ToString());    //tbPass为输入密码的文本框  
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            string s_output = BitConverter.ToString(output).Replace("-", "");

            return s_output;
        }

        public static string GetTimestamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public IDictionary<string, string> GetUrlParameters()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            //parameters.Add("user_code", this.UserCode);
            //parameters.Add("supplier_code", "pingan");
            //parameters.Add("license_plate", "粤A-88888");

            //parameters.Add("timestamp", "20170420122055");
            //parameters.Add("transaction_id", "20170420122055");


            parameters.Add("user_code", this.UserCode);
            parameters.Add("license_plate", _license_plate);

            if (!string.IsNullOrEmpty(_vehicle_frame_no))
            {
                parameters.Add("vehicle_frame_no", _vehicle_frame_no);
            }

            if (!string.IsNullOrEmpty(_engine_number))
            {
                parameters.Add("engine_number", _engine_number);
            }

            parameters.Add("timestamp", GetTimestamp());
            parameters.Add("transaction_id", GetTimestamp());


            string sign = GetMD5(parameters, this.Key);

            parameters.Add("sign", sign);

            return parameters;
        }
    }
}
