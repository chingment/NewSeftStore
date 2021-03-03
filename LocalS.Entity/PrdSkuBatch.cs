using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LocalS.Entity
{
    [Table("PrdSkuBatch")]
    public class PrdSkuBatch
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string SpuId { get; set; }
        public string SkuId { get; set; }
        public string BatchNumber { get; set; }
        public string ProductDate { get; set; }
        public string ValidDate { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
