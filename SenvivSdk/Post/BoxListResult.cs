using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenvivSdk
{
    public class BoxListResult
    {
        public int count { get; set; }

        public List<DataModel> data { get; set; }

        public class DataModel
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
            public int status { get; set; }
            public string version { get; set; }
            public string remarks { get; set; }
            public string batch { get; set; }
            public string scmver { get; set; }
            public string sovn { get; set; }
            public string epromvn { get; set; }
            public string imsi { get; set; }
            public string imei { get; set; }
            public string longitude { get; set; }
            public string latitude { get; set; }
            public List<string> roles { get; set; }
            public string _id { get; set; }
            public string deptid { get; set; }
        }
    }
}
