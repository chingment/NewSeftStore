using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("PrdProductKind")]
    public class PrdProductKind
    {
        [Key]
        public string Id { get; set; }

        public string PrdKindId { get; set; }
        public string PrdProductId { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
