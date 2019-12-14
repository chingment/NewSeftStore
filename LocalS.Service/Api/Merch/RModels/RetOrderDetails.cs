using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetOrderDetails
    {
        public RetOrderDetails()
        {
            this.SellChannelDetails = new List<SellChannelDetail>();
        }

        public string Id { get; set; }
        public string Sn { get; set; }
        public string ClientUserName { get; set; }
        public string ClientUserId { get; set; }
        public string StoreName { get; set; }
        public string SubmitTime { get; set; }
        public string ChargeAmount { get; set; }
        public string DiscountAmount { get; set; }
        public string OriginalAmount { get; set; }
        public int Quantity { get; set; }
        public string CreateTime { get; set; }
        public StatusModel Status { get; set; }
        public string SourceName { get; set; }
        public List<SellChannelDetail> SellChannelDetails { get; set; }
        public class SellChannelDetail
        {
            public SellChannelDetail()
            {
                this.DetailItems = new List<object>();
            }

            public string Name { get; set; }
            public E_SellChannelRefType Type { get; set; }
            public int DetailType { get; set; }
            public List<object> DetailItems { get; set; }
        }

        public class PickupSku
        {
            public PickupSku()
            {
                this.PickupLogs = new List<PickupLog>();
            }

            public string Id { get; set; }
            public string MainImgUrl { get; set; }
            public string Name { get; set; }
            public int Quantity { get; set; }
            public StatusModel Status { get; set; }
            public List<PickupLog> PickupLogs { get; set; }
        }

        public class PickupLog
        {
            public PickupLog()
            {
                this.ImgUrls = new List<string>();
            }

            public string Timestamp { get; set; }
            public string Content { get; set; }
            public string ImgUrl { get; set; }
            public List<string> ImgUrls { get; set; }
        }
    }
}
