using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("MachineOperateLog")]
    public class MachineOperateLog
    {
        [Key]
        public string Id { get; set; }
        public string MachineId { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string OperateUserId { get; set; }
        public string Action { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
