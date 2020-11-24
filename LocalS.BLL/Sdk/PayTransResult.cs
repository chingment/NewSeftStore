﻿using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class PayTransResult
    {
        public bool IsPaySuccess { get; set; }
        public string PayTransId { get; set; }
        public string PayPartnerPayTransId { get; set; }
        public E_PayWay PayWay { get; set; }
        public string ClientUserName { get; set; }
    }
}