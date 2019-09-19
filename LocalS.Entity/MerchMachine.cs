using System;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("MerchMachine")]
    public class MerchMachine
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string MachineId { get; set; }
        public string StoreId { get; set; }
        [MaxLength(128)]
        public string Name { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string LogoImgUrl { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
