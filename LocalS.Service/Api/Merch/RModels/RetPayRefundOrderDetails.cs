using LocalS.BLL.Biz;
using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetPayRefundOrderDetails
    {
        public RetPayRefundOrderDetails()
        {
            this.ReceiveModes = new List<ReceiveMode>();
        }

        public string Id { get; set; }
        public string ClientUserName { get; set; }
        public string ClientUserId { get; set; }
        public string StoreName { get; set; }
        public string SubmittedTime { get; set; }
        public string ChargeAmount { get; set; }
        public string DiscountAmount { get; set; }
        public string OriginalAmount { get; set; }
        public int Quantity { get; set; }
        public StatusModel Status { get; set; }
        public string SourceName { get; set; }
        public bool CanHandleEx { get; set; }
        public string ExHandleRemark { get; set; }
        public bool ExIsHappen { get; set; }
        public string ReceiveModeName { get; set; }
        public string RefundedAmount { get; set; }
        public string RefundingAmount { get; set; }
        public string RefundableAmount { get; set; }

        public List<ReceiveMode> ReceiveModes { get; set; }
        public class ReceiveMode
        {
            public ReceiveMode()
            {
                this.Items = new List<object>();
            }

            public string Name { get; set; }
            public E_ReceiveMode Mode { get; set; }
            public int Type { get; set; }
            public List<object> Items { get; set; }
        }
    }
}
