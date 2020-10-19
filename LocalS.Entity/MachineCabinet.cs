using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LocalS.Entity
{
    [Table("MachineCabinet")]
    public class MachineCabinet
    {
        [Key]
        public string Id { get; set; }
        [MaxLength(128)]
        public string MachineId { get; set; }
        /// <summary>
        /// 命名命令规则  用于发起取货调用方式解释
        /// DSX01N01:DS代表德尚机器 X01 代表机器型号 N01 代表主柜 N02 代表副柜
        /// ZSX01N01:ZS代表中顺机器 X01 代表机器型号 N01 代表主柜 N02 代表副柜
        /// </summary>
        public string CabinetId { get; set; }
        public string CabinetName { get; set; }
        public string RowColLayout { get; set; }
        //public string PendantRows { get; set; }
        public bool IsUse { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
        public int Priority { get; set; }
        public string ComId { get; set; }
    }
}
