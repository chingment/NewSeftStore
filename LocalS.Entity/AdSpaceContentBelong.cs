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
        public E_AdSpaceId AdSpaceId { get; set; }
        public string AdSpaceContentId { get; set; }
        public string MerchId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public int Priority { get; set; }
        public E_AdSpaceBelongType BelongType { get; set; }
        public E_AdSpaceContentBelongStatus Status { get; set; }
        public string BelongId { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
