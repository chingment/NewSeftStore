using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LocalS.Entity
{
    [Table("PrdProductSkuBatch")]
    public class PrdProductSkuBatch
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string PrdProductId { get; set; }
        public string PrdProductSkuId { get; set; }
        public string BatchNumber { get; set; }
        public string ProductDate { get; set; }
        public string ValidDate { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
