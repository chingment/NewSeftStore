﻿using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RupOrderGetList : RupBaseGetList
    {
        public string ClientUserName { get; set; }
        public string StoreId { get; set; }
        public string OrderId { get; set; }
        public E_OrderStatus OrderStatus { get; set; }
        public string DeviceId { get; set; }
        public string DeviceCumCode { get; set; }
        public string ClientUserId { get; set; }
        public bool IsHasEx { get; set; }
        public E_ReceiveMode ReceiveMode { get; set; }
        public int PickupTrgStatus { get; set; }
        public string[] SubmittedTimeArea { get; set; }
    }
}
