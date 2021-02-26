using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenvivSdk
{
    public class GetAccessTokenResult
    {
        public string deptid { get; set; }
        public string access_token { get; set; }
        public string appid { get; set; }
        public List<object> template_list { get; set; }
    }
}
