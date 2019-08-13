using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("InsCarPlateNoSearchHis")]
    public class InsCarPlateNoSearchHis
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string CarPlateNo { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
