using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Biz
{
    public class RopOrderReserve
    {
        public RopOrderReserve()
        {
            this.Skus = new List<Sku>();
        }

        public string StoreId { get; set; }
        public string ClientUserId { get; set; }
        public E_OrderSource Source { get; set; }
        public E_ReserveMode ReserveMode { get; set; }
        public string SellChannelRefId { get; set; }
        public E_StoreSellChannelRefType SellChannelRefType { get; set; }
        public string Receiver { get; set; }
        public string ReceiverPhone { get; set; }
        public string ReceptionAddress { get; set; }
        public List<Sku> Skus { get; set; }
        public class Sku
        {
            public string CartId { get; set; }
            public string Id { get; set; }
            public int Quantity { get; set; }
        }
    }
}
