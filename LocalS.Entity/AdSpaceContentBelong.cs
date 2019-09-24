using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_AdSpaceContentBelongStatus
    {
        Unknow = 0,
        Normal = 1,
        Deleted = 2
    }

    [Table("AdSpaceContentBelong")]
    public class AdSpaceContentBelong
    {
        [Key]
        public string Id { get; set; }
        public string AdSpaceContentId { get; set; }
        public E_AdSpaceBelongType BelongType { get; set; }
        public string BelongId { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
