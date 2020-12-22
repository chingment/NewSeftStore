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
        //自取
        SelfTake = 1,
        //系统赠送
        SysGive = 2,
        //后台工作人赠送
        WorGive = 3
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
        public string CouponId { get; set; }
        public DateTime ValidStartTime { get; set; }
        public DateTime ValidEndTime { get; set; }
        public E_ClientCouponStatus Status { get; set; }
        public E_ClientCouponSourceType SourceType { get; set; }
        public string SourceObjType { get; set; }
        public string SourceObjId { get; set; }
        public string SourcePoint { get; set; }
        public DateTime SourceTime { get; set; }
        [MaxLength(512)]
        public string SourceDes { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
