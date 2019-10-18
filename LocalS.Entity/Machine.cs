using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_MachineRunStatus
    {
        Unknow = 0,
        Running = 1,
        Stoped = 2,
        Setting = 3
    }

    [Table("Machine")]
    public class Machine
    {
        [Key]
        public string Id { get; set; }
        [MaxLength(128)]
        public string Name { get; set; }
        public string MainImgUrl { get; set; }
        [MaxLength(128)]
        public string MacAddress { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
        public float Lat { get; set; }
        public float Lng { get; set; }
        public string JPushRegId { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string LogoImgUrl { get; set; }
        public DateTime? LastRequestTime { get; set; }
        public E_MachineRunStatus RunStatus { get; set; }

        public string CabinetId_1 { get; set; }
        public string CabinetName_1 { get; set; }
        public int CabinetMaxRow_1 { get; set; }
        public int CabinetMaxCol_1 { get; set; }
    }
}
