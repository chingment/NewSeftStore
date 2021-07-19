using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_ErpReplenishPlan_Status
    {

        Unknow = 0,
        Building = 1,
        BuildSuccess = 2,
        BuildFailure = 3
    }

    [Table("ErpReplenishPlan")]
    public class ErpReplenishPlan
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string DocMakerUserId { get; set; }
        public string DocMakerFullName { get; set; }
        public DateTime MakeTime { get; set; }
        public DateTime BuildTime { get; set; }
        public E_ErpReplenishPlan_Status Status { get; set; }
        public string FailReason { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
