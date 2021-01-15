using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{


    [Table("MerchOperateLog")]
    public class MerchOperateLog
    {

        [Key]
        public string Id { get; set; }
        public string AppId { get; set; }
        public string TrgerId { get; set; }
        public string TrgerName { get; set; }
        public string OperateUserId { get; set; }
        public string OperateUserName { get; set; }
        public string EventCode { get; set; }
        public string EventName { get; set; }
        public string EventLevel { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
