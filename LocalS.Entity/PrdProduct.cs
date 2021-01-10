using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_SupReceiveMode
    {

        Unknow = 0,
        Delivery = 1,
        SelfTakeByStore = 2,
        DeliveryOrSelfTakeByStore = 3,
        SelfTakeByMachine = 4,
        FeeByMember = 5,
        ConsumeByStore = 6
    }

    [Table("PrdProduct")]
    public class PrdProduct
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string PrdKindIds { get; set; }
        public int PrdKindId1 { get; set; }
        public int PrdKindId2 { get; set; }
        public int PrdKindId3 { get; set; }
        public string Name { get; set; }
        public string SpuCode { get; set; }
        public string PinYinIndex { get; set; }
        public string MainImgUrl { get; set; }
        public string DisplayImgUrls { get; set; }
        public string DetailsDes { get; set; }
        public string BriefDes { get; set; }
        public string SpecItems { get; set; }
        public bool IsTrgVideoService { get; set; }
        public bool IsRevService { get; set; }
        public bool IsHardware { get; set; }
        public E_SupReceiveMode SupReceiveMode { get; set; }
        public string CharTags { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? MendTime { get; set; }
        public string Mender { get; set; }
        public string SupplierId { get; set; }
        public bool IsMavkBuy{ get; set; }
    }
}
