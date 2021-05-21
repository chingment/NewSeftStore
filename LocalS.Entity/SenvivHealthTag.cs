using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("SenvivHealthTag")]
    public class SenvivHealthTag
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
