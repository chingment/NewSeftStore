﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_MerchMachineUseStatus
    {

        Unknow = 0,
        Normarl = 1,
        Stop = 2
    }

    [Table("MerchMachine")]
    public class MerchMachine
    {
        public string Id { get; set; }
        [Key]
        [Column(Order = 1)]
        public string MerchId { get; set; }
        [Key]
        [Column(Order = 2)]
        public string MachineId { get; set; }
        public string CurUseStoreId { get; set; }
        public string CurUseShopId { get; set; }
        public string CumCode { get; set; }
        [MaxLength(128)]
        public string Name { get; set; }
        public string LogoImgUrl { get; set; }
        //停止使用，当商户合约到期不使用
        public bool IsStopUse { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
