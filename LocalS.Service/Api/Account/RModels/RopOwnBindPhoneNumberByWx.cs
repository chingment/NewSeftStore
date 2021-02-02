using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Account
{
    public class RopOwnBindPhoneNumberByWx
    {
        public string AppId { get; set; }
        public string MerchId { get; set; }
        public string OpenId { get; set; }
        public string PhoneNumber { get; set; }
    }
}
