using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("SenvivDept")]
    public class SenvivDept
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AppId { get; set; }
    }
}
