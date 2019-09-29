﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("AdContentBelong")]
    public class AdContentBelong
    {
        [Key]
        public string Id { get; set; }
        public string AdContentId { get; set; }
        public string MerchId { get; set; }
        public E_AdSpaceId AdSpaceId { get; set; }
        public E_AdSpaceBelongType BelongType { get; set; }
        public string BelongId { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}