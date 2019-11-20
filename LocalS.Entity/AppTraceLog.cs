﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LocalS.Entity
{
    [Table("AppTraceLog")]
    public class AppTraceLog
    {
        [Key]
        public string Id { get; set; }
        public string AppChannel { get; set; }
        public string AppId { get; set; }
        public string AppVersion { get; set; }
        public string DeviceDensity { get; set; }
        public string DeviceId { get; set; }
        public string DeviceLocale { get; set; }
        public string DeviceMacAddr { get; set; }
        public string DeviceModel { get; set; }
        public string DeviceOsVersion { get; set; }
        public string DevicePlatform { get; set; }
        public string DeviceScreen { get; set; }
        public string IpAddr { get; set; }
        public bool Wifi { get; set; }
        public string AppActionTime { get; set; }
        public int AppActionType { get; set; }
        public string AppActionDesc { get; set; }
        public string PageId { get; set; }
        public string PageRefererPageId { get; set; }
        public string PageStartTime { get; set; }
        public string PageEndTime { get; set; }
        public string EventPageId { get; set; }
        public string EventRefererPageId { get; set; }
        public string EventName { get; set; }
        public string EventActionTime { get; set; }
        public string ExceptionInfos { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
