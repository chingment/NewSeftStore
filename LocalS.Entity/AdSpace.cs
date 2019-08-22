using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("AdSpace")]
    public class AdSpace
    {
        [Key]
        public E_AdSpaceId Id { get; set; }
        public string Name { get; set; }
        public E_AdSpaceBelong Belong { get; set; }
        public string Description { get; set; }
        public E_AdSpaceType Type { get; set; }
        public E_AdSpaceStatus Status { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
