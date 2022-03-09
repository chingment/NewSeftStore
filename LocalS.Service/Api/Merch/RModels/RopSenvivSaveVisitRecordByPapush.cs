using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopSenvivSaveVisitRecordByPapush
    {
        public E_SvVisitRecordVisitTemplate VisitTemplate { get; set; }
        public object VisitContent { get; set; }
        public string SvUserId { get; set; }
        public string ReportId { get; set; }
        public string TaskId { get; set; }
    }
}
