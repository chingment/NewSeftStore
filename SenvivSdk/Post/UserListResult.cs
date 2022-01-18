using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenvivSdk
{
    public class UserListResult
    {
        public int count { get; set; }

        public List<DataModel> data { get; set; }

        public class DataModel
        {

            public List<ProductModel> products { get; set; }
            public string userid { get; set; }
            public string code { get; set; }
            public string wechatid { get; set; }
            public List<object> weChatFriends { get; set; }
            public string mobile { get; set; }
            public string Email { get; set; }
            public string pwd { get; set; }
            public string nick { get; set; }
            public string account { get; set; }
            public string headimgurl { get; set; }
            public string language { get; set; }
            public string sex { get; set; }
            public string birthday { get; set; }
            public string height { get; set; }
            public string weight { get; set; }
            public string TargetValue { get; set; }
            public string createtime { get; set; }
            public string updateTime { get; set; }
            public List<object> contacts { get; set; }
            public string remarks { get; set; }
            public string lastloginTime { get; set; }
            public string loginCount { get; set; }
            public string lastReportId { get; set; }
            public string lastReportTime { get; set; }
            public string details { get; set; }
            public string status { get; set; }
            public string SAS { get; set; }
            public string BreathingMachine { get; set; }
            public string Perplex { get; set; }
            public string OtherPerplex { get; set; }
            public string Medicalhistory { get; set; }
            public string OtherFamilyhistory { get; set; }
            public string Medicine { get; set; }
            public string OtherMedicine { get; set; }
            public List<string> roles { get; set; }
            public Dictionary<string,object> dic { get; set; }
            public string _id { get; set; }
            public string deptid { get; set; }
        }

        public class ProductModel
        {
            public string name { get; set; }
            public string deptName { get; set; }
            public string sn { get; set; }
            public string qrcodeUrl { get; set; }
            public string deviceQRCode { get; set; }
            public string tcpAddress { get; set; }
            public string webUrl { get; set; }
            public string LastOnlineTime { get; set; }
            public string model { get; set; }
            public string userid { get; set; }
            public string bindtime { get; set; }
            public string createtime { get; set; }
            public string status { get; set; }
            public string version { get; set; }
            public string remarks { get; set; }
            public string batch { get; set; }
            public string scmver { get; set; }
            public string sovn { get; set; }
            public string epromvn { get; set; }
            public string imsi { get; set; }
            public string longitude { get; set; }
            public string latitude { get; set; }
            public List<string> roles { get; set; }
            public string _id { get; set; }
            public string deptid { get; set; }
        }
    }
}
