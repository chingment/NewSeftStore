using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LocalS.Entity
{
    public enum E_ClientCartStatus
    {

        Unknow = 0,
        WaitSettle = 1,
        Settling = 2,
        SettleCompleted = 3,
        Deleted = 4
    }

    public enum E_ReceptionMode
    {
        Unknow = 0,
        Express = 1,
        Machine = 2
    }

    [Table("ClientCart")]
    public class ClientCart
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string ClientUserId { get; set; }
        public string ProductSkuId { get; set; }
        [MaxLength(128)]
        public string ProductSkuName { get; set; }
        [MaxLength(256)]
        public string ProductSkuMainImgUrl { get; set; }
        public int Quantity { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
        public bool Selected { get; set; }
        public E_ClientCartStatus Status { get; set; }
        public E_ReceptionMode ReceptionMode { get; set; }
    }
}
