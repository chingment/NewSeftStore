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
        public string SvUserId { get; set; }

        public string ReportId { get; set; }
        public string TaskId { get; set; }
    }
}
