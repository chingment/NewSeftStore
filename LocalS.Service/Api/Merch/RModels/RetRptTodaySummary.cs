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
            this.TodayGmvRl = new TodayGmvRlModel();
        }

        public int SumExWaitHandleCount { get; set; }

        public TodayGmvRlModel TodayGmvRl { get; set; }

        public class TodayGmvRlModel
        {
            public string SumCount { get; set; }
            public string SumTradeAmount { get; set; }
        }
    }
}
