using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("Merch")]
    public class Merch
    {
        [Key]
        public string Id { get; set; }
        public string PId { get; set; }
        public string MerchUserId { get; set; }
        public string Name { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactAddress { get; set; }
        public string CsrQrCode { get; set; }
        public string CsrPhoneNumber { get; set; }
        public string CsrHelpTip { get; set; }
        public string BuildStockRptDate { get; set; }
        public int ImAccountLimit { get; set; }
        public bool IsOpenMemberRight { get; set; }
        public bool IsOpenCouponRight { get; set; }
        public string SenvivDepts { get; set; }
        public string IotApiSecret { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
