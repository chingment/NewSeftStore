using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("StoreShop")]
    public class StoreShop
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string ShopId { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
