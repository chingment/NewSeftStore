﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{

    [Table("PrdKind")]
    public class PrdKind
    {
        [Key]
        public string Id { get; set; }
        [MaxLength(128)]
        public string PId { get; set; }
        public string Name { get; set; }
        [MaxLength(128)]
        public string MerchId { get; set; }
        public string IconImgUrl { get; set; }
        public string DispalyImgUrls { get; set; }
        public string MainImgUrl { get; set; }
        [MaxLength(512)]
        public string Description { get; set; }
        public bool IsDelete { get; set; }
        public int Priority { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
        public int Depth { get; set; }

    }
}
