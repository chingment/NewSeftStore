using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("SvHealthTagExplain")]
    public class SvHealthTagExplain
    {
        [Key]
        public int Id { get; set; }

        public string MerchId { get; set; }
        public int TagId { get; set; }
        public string TagName { get; set; }
        public string ProExplain { get; set; }
        public string TcmExplain { get; set; }
        public string Suggest { get; set; }
    }
}
