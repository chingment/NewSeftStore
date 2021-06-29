using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetRptTodaySummary
    {
        public RetRptTodaySummary()
        {
            this.TodayGmvRl = new GmvRlModel();
            this.NowMonthGmvRl = new GmvRlModel();
            this.LastMonthGmvRl = new GmvRlModel();
        }

        public int SumExHdByDeviceSelfTake { get; set; }

        public GmvRlModel TodayGmvRl { get; set; }

        public GmvRlModel NowMonthGmvRl { get; set; }

        public GmvRlModel LastMonthGmvRl { get; set; }

        public class GmvRlModel
        {
            public string SumCount { get; set; }
            public string SumQuantity { get; set; }
            public string SumTradeAmount { get; set; }
        }

    }
}
