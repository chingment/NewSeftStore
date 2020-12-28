using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("StoreSelfPickAddress")]
    public class StoreSelfPickAddress
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string SelfPickAddressId { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
