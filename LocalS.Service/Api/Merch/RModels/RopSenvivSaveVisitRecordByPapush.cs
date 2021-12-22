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
        public E_SenvivVisitRecordVisitTemplate VisitTemplate { get; set; }
        public object VisitContent { get; set; }
        public string UserId { get; set; }
        public string ReportId { get; set; }
        public string TaskId { get; set; }
    }
}
