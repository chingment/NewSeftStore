using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.DbRelay
{
    [Table("SysMenu")]
    public class SysMenu
    {
        [Key]
        public string Id { get; set; }
        [MaxLength(128)]
        [Required]
        public string Name { get; set; }
        public string PId { get; set; }
        [MaxLength(256)]
        public string Path { get; set; }
        [MaxLength(256)]
        public string Title { get; set; }
        [MaxLength(256)]
        public string Icon { get; set; }
        [MaxLength(256)]
        public string Component { get; set; }
        [MaxLength(512)]
        public string Description { get; set; }
        public int Priority { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
        public Enumeration.BelongSite BelongSite { get; set; }
        public bool IsRouter { get; set; }
        public bool IsSidebar { get; set; }
        public bool IsNavbar { get; set; }
        public int Depth { get; set; }
    }
}
