using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("InsCarCompanyRule")]
    public class InsCarCompanyRule
    {
        [Key]
        public string Id { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyImgUrl { get; set; }
        public int Priority { get; set; }
        public int CommissionRate { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
