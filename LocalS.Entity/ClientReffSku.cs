using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LocalS.Entity
{
    public enum E_ClientReffSkuStatus
    {

        Unknow = 0,
        Valid = 1,
        InValid = 2
    }

    [Table("ClientReffSku")]
    public class ClientReffSku
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string ClientUserId { get; set; }
        public string SkuId { get; set; }
        public string SkuName { get; set; }
        public string SkuMainImgUrl { get; set; }
        public string SkuBarCode { get; set; }
        public string SkuCumCode { get; set; }
        public string SkuSpecDes { get; set; }
        public string SkuProducer { get; set; }
        public int Quantity { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string ReffClientUserId { get; set; }
        public string OrderId { get; set; }
        public string OrderSubId { get; set; }
        public E_ClientReffSkuStatus Status { get; set; }
    }
}
