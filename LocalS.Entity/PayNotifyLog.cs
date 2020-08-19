using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LocalS.Entity
{
    public enum E_PayTransLogNotifyType
    {

        Unknow = 0,
        PayTrans = 1,
        PayRefund = 2
    }
    public enum E_PayTransLogNotifyFrom
    {
        Unknow = 0,
        NotifyUrl = 2,
        Query = 3
    }

    [Table("PayNotifyLog")]
    public class PayNotifyLog
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string PayTransId { get; set; }
        public string PayRefundId { get; set; }
        public E_PayPartner PayPartner { get; set; }
        public string PayPartnerPayTransId { get; set; }
        public string PayPartnerPayRefundId { get; set; }
        public E_PayTransLogNotifyType NotifyType { get; set; }
        public E_PayTransLogNotifyFrom NotifyFrom { get; set; }
        public string NotifyContent { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
