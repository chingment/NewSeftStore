using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class SenvivSaveMonthReportSug
    {
        public string ReportId { get; set; }
        public string RptSummary { get; set; }
        public string RptSuggest { get; set; }
        //public string SugBySm { get; set; }
        //public string SugByQxyl { get; set; }
        public bool IsSend { get; set; }
    }
}
