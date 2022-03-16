﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_SvTaskType
    {
        Unknow = 0,
        Health_Monitor_FisrtDay = 1,
        Health_Monitor_SeventhDay = 2,
        Health_Monitor_FourteenthDay = 3,
        //Health_Monitor_PerDay = 4,
        Health_Monitor_PerMonth = 5
    }

    public enum E_SvTaskStatus
    {
        Unknow = 0,
        WaitHandle = 1,
        Handling = 2,
        Handled = 3
    }

    [Table("SvTask")]
    public class SvTask
    {
        [Key]
        public string Id { get; set; }
        public E_SvTaskType TaskType { get; set; }
        public string SvUserId { get; set; }
        public string Title { get; set; }
        public string HandleTime { get; set; }
        public string Handler { get; set; }
        public string HandleContent { get; set; }
        public string ReportId { get; set; }
        public string Params { get; set; }
        public E_SvTaskStatus Status { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}