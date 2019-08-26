using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("PrdProductSubject")]
    public class PrdProductSubject
    {
        [Key]
        public string Id { get; set; }
        public string PrdSubjectId { get; set; }
        public string PrdProductId { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
