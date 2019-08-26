using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_StoreStatus
    {

        Unknow = 0,
        Valid = 1,
        Invalid = 2
    }

    [Table("Store")]
    public class Store
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        [MaxLength(128)]
        public string Name { get; set; }

        public string MainImgUrl { get; set; }

        public string DispalyImgUrls { get; set; }
        [MaxLength(128)]
        public string Address { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public bool IsOpen { get; set; }
        public bool IsDelete { get; set; }
        public string BriefDes { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
