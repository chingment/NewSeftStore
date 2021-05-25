using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("SenvivHealthTagExplain")]
    public class SenvivHealthTagExplain
    {
        [Key]
        public string Id { get; set; }
        public string TagId { get; set; }
        public string Explain { get; set; }
        public string Suggest { get; set; }
    }
}
