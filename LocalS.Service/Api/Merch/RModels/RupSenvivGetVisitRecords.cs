﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RupSenvivGetVisitRecords : RupBaseGetList
    {
        public string SvUserId { get; set; }

        public string TaskId { get; set; }

        public string ReportId { get; set; }

    }
}
