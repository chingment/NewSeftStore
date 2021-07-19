using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_ErpReplenishPlan_Status
    {

        Unknow = 0,
        Submit = 1,
        Building = 2,
        BuildSuccess = 3,
        BuildFailure = 4
    }

    [Table("ErpReplenishPlan")]
    public class ErpReplenishPlan
    {
        [Key]
        public string Id { get; set; }
        public string CumCode { get; set; }
        public string MerchId { get; set; }
        public string MakerName { get; set; }
        public string MakerId { get; set; }
        public string MakeDate { get; set; }
        public DateTime MakeTime { get; set; }
        public DateTime? BuildTime { get; set; }
        public E_ErpReplenishPlan_Status Status { get; set; }

        public string FailReason { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
