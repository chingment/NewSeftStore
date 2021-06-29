﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_StoreStatus
    {

        Unknow = 0,
        Valid = 1,
        Invalid = 2
    }

    [Table("Store")]
    public class Store
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string Name { get; set; }
        public string MainImgUrl { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactAddress { get; set; }
        public bool IsDelete { get; set; }
        public string BriefDes { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
        public bool IsTestMode { get; set; }
        //线上商城 F
        //线下设备 K
        //线上商城+线下设备 FK
        public string SctMode { get; set; }
    }
}
