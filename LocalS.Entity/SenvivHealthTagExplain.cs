using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("SenvivHealthTagExplain")]
    public class SenvivHealthTagExplain
    {
        [Key]
        public int Id { get; set; }
        public int TagId { get; set; }
        public string TagName { get; set; }
        public string Explain { get; set; }
        public string CnExplain { get; set; }
        public string Suggest { get; set; }
    }
}
