using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LocalS.Entity
{
    public enum E_OrderNotifyLogNotifyType
    {

        Unknow = 0,
        Pay = 1,
        ReFund = 2
    }
    public enum E_OrderNotifyLogNotifyFrom
    {
        Unknow = 0,
        WebApp = 1,
        NotifyUrl = 2,
        OrderQuery = 3
    }

    [Table("OrderNotifyLog")]
    public class OrderNotifyLog
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }

        public string OrderId { get; set; }

        public string OrderSn { get; set; }

        public E_OrderNotifyLogNotifyType NotifyType { get; set; }

        public E_OrderNotifyLogNotifyFrom NotifyFrom { get; set; }

        public string NotifyContent { get; set; }

        public string Creator { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
