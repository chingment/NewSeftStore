using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LocalS.Entity
{
    [Table("Shop")]
    public class Shop
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string Name { get; set; }
        public string MainImgUrl { get; set; }
        public string DisplayImgUrls { get; set; }
        public string AreaName { get; set; }
        public string AreaCode { get; set; }
        public string Address { get; set; }
        public string ContactName { get; set; }
        public string ContactAddress { get; set; }
        public string ContactPhone { get; set; }
        public float Lat { get; set; }
        public float Lng { get; set; }
        public bool IsOpen { get; set; }
        public bool IsDelete { get; set; }
        public string BriefDes { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
