﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{

    public class RetStoreInitGetProductSkuList
    {
        public RetStoreInitGetProductSkuList()
        {
            this.SellChannels = new List<StoreSellChannelModel>();
        }

        public List<StoreSellChannelModel> SellChannels { get; set; }
    }
}
