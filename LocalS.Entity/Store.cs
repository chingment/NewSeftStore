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
        public string DisplayImgUrls { get; set; }
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

        public bool IsTestMode { get; set; }


        //店铺模式  T1：配送，T2:店铺自取，T3:配送+店铺自取
        //线上商城 F+(Tn)
        //线下机器 K +(Tn)
        //线上商城+线下机器 FK+(Tn)
        public string SctMode { get; set; }
    }
}
