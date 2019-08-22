using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_ClientCouponStatus
    {

        Unknow = 0,
        WaitUse = 1,
        Used = 2,
        Expired = 3,
        Delete = 4,
        Frozen = 5
    }

    public enum E_ClientCouponSourceType
    {

        Unknow = 0,
        Receive = 1,
        Give = 2
    }

    public enum E_ClientCouponType
    {

        Unknow = 0,
        FullCut = 1,
        UnLimitedCut = 2,
        Discount = 3
    }

    [Table("ClientCoupon")]
    public class ClientCoupon
    {
        [Key]
        public string Id { get; set; }
        [MaxLength(128)]
        public string Sn { get; set; }
        public string MerchId { get; set; }
        public string ClientUserId { get; set; }
        [MaxLength(128)]
        public string Name { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public E_ClientCouponStatus Status { get; set; }
        public E_ClientCouponSourceType SourceType { get; set; }
        public string SourceUserId { get; set; }
        public DateTime? SourceTime { get; set; }
        [MaxLength(512)]
        public string SourceDescription { get; set; }
        public E_ClientCouponType Type { get; set; }
        public decimal LimitAmount { get; set; }
        public decimal Discount { get; set; }
        [MaxLength(1024)]
        public string LimitTarget { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
