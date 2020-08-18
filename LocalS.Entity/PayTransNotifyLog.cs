using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LocalS.Entity
{
    public enum E_PayTransLogNotifyType
    {

        Unknow = 0,
        Pay = 1,
        ReFund = 2
    }
    public enum E_PayTransLogNotifyFrom
    {
        Unknow = 0,
        NotifyUrl = 2,
        Query = 3
    }

    [Table("PayTransNotifyLog")]
    public class PayTransNotifyLog
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string PayTransId { get; set; }
        public E_PayPartner PayPartner { get; set; }
        public string PayPartnerPayTransId { get; set; }
        public E_PayTransLogNotifyType NotifyType { get; set; }
        public E_PayTransLogNotifyFrom NotifyFrom { get; set; }
        public string NotifyContent { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
