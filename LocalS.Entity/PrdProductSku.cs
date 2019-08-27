using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LocalS.Entity
{
    [Table("PrdProductSku")]
    public class PrdProductSku
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string PrdProductId { get; set; }
        public string Name { get; set; }
        public decimal SalePrice { get; set; }
        public string SpecDes { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? MendTime { get; set; }
        public string Mender { get; set; }
    }
}
