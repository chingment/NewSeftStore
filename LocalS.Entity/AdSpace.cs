﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_AdSpaceId
    {
        Unknow = 0,
        MachineHome = 1,
        AppHomeTop = 2
    }
    public enum E_AdSpaceBelongType
    {

        Unknow = 0,
        App = 1,
        Machine = 2
    }

    [Table("AdSpace")]
    public class AdSpace
    {
        [Key]
        public E_AdSpaceId Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public E_AdSpaceBelongType BelongType { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
