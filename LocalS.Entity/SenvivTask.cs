﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_SenvivTaskType
    {
        Unknow = 0,
        FisrtDay = 1,
        SeventhDay = 2,
        FourteenthDay = 3,
        PerMonth = 4
    }

    public enum E_SenvivTaskStatus
    {
        Unknow = 0,
        WaitHandle = 1,
        Handling = 2,
        Handled = 3
    }

    [Table("SenvivTask")]
    public class SenvivTask
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public E_SenvivTaskType TaskType { get; set; }
        public string SvUserId { get; set; }
        public string Title { get; set; }
        public string HandleTime { get; set; }
        public string Handler { get; set; }
        public string Params { get; set; }
        public E_SenvivTaskStatus Status { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}