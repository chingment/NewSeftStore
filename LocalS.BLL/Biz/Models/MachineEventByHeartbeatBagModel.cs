﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class MachineEventByHeartbeatBagModel : MachineEventBaseModel
    {
        public string Status { get; set; }
        public string Remark { get; set; }
    }
}