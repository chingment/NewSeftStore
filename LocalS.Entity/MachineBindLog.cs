using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_MachineBindType
    {
        Unknow = 0,
        BindOnMerch = 1,
        BindOffMerch = 2,
        BindOnStore = 3,
        BindOffStore = 4
    }

    [Table("MachineBindLog")]
    public class MachineBindLog
    {
        [Key]
        public string Id { get; set; }
        public string MachineId { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public E_MachineBindType BindType { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string RemarkByDev { get; set; }
    }
}
