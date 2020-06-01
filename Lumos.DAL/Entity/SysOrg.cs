using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.DbRelay
{
    public enum E_SysOrgStatus
    {

        Unknow = 0,
        Valid = 1,
        Invalid = 2
    }

    [Table("SysOrg")]
    public class SysOrg
    {
        [Key]
        public string Id { get; set; }
        public string PId { get; set; }
        [MaxLength(128)]
        public string Name { get; set; }

        public string FullName { get; set; }

        [MaxLength(512)]
        public string Description { get; set; }
        public bool IsDelete { get; set; }
        public E_SysOrgStatus Status { get; set; }

        public Enumeration.BelongSite BelongSite { get; set; }

        public string ReferenceId { get; set; }

        public int Priority { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
        public int Depth { get; set; }
    }
}
