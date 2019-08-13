using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("ProductSkuSubject")]
    public class ProductSkuSubject
    {
        [Key]
        public string Id { get; set; }
        public string ProductSubjectId { get; set; }
        public string ProductSkuId { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
