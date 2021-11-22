using LocalS.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class DeviceService : BaseService
    {
        public string GetCode(string deviceId, string cumCode)
        {
            if (string.IsNullOrEmpty(cumCode))
                return deviceId;

            return cumCode;
        }
    }
}
