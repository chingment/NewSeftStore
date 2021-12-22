using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class SenvivSaveMonthReportSug
    {
        public SenvivSaveMonthReportSug()
        {
            this.SugSkus = new List<SugSkuModel>();
        }

        public string TaskId { get; set; }
        public string ReportId { get; set; }
        public string RptSummary { get; set; }
        public string RptSuggest { get; set; }
        //public string SugBySm { get; set; }
        //public string SugByQxyl { get; set; }
        public bool IsSend { get; set; }

        public List<SugSkuModel> SugSkus { get; set; }
        public class SugSkuModel
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public string CumCode { get; set; }
        }
    }
}
