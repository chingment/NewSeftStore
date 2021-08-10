using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopReportDeviceSalesHisGet
    {
        public string StoreName { get; set; }

        public string ShopName { get; set; }

        public string DeviceCumCode { get; set; }

        public string[] TradeDateTimeArea { get; set; }
    }
}
