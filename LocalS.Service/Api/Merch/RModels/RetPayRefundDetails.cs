using LocalS.BLL.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetPayRefundDetails
    {
        public RetPayRefundDetails()
        {
            this.Order = new _Order();
            this.Skus = new List<object>();
        }

        public string PayRefundId { get; set; }
        public string PayPartnerPayTransId { get; set; }
        public string PayTransId { get; set; }
        public string OrderId { get; set; }
        public FieldModel ApplyMethod { get; set; }
        public decimal ApplyAmount { get; set; }
        public string ApplyTime { get; set; }
        public string ApplyRemark { get; set; }
        public FieldModel Status { get; set; }
        public string HandleRemark { get; set; }
        public string HandleTime { get; set; }
        public string RefundedRemark { get; set; }
        public string RefundedTime { get; set; }
        public decimal RefundedAmount { get; set; }
        public List<object> Skus { get; set; }

        public _Order Order { get; set; }
        public class _Order
        {
            public _Order()
            {
                this.Skus = new List<object>();
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
            public FieldModel Status { get; set; }
            public string SourceName { get; set; }
            public List<object> Skus { get; set; }
            public string RefundedAmount { get; set; }
            public int RefundedQuantity { get; set; }
            public bool CanHandleEx { get; set; }
            public string ExHandleRemark { get; set; }
            public bool ExIsHappen { get; set; }
            public string DeviceCumCode { get; set; }
            public FieldModel PayWay { get; set; }
        }
    }
}
