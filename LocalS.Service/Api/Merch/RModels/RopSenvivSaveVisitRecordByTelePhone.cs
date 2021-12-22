using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopSenvivSaveVisitRecordByTelePhone
    {
        public object VisitContent { get; set; }
        public string VisitTime { get; set; }
        public string NextTime { get; set; }
        public string UserId { get; set; }

        public string ReportId { get; set; }
    }
}
