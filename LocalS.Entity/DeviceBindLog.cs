using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_DeviceBindType
    {
        Unknow = 0,
        BindOnMerch = 1,
        BindOffMerch = 2,
        BindOnStore = 3,
        BindOffStore = 4,
        BindOnShop = 5,
        BindOffShop = 6
    }

    [Table("DeviceBindLog")]
    public class DeviceBindLog
    {
        [Key]
        public string Id { get; set; }
        public string DeviceId { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string ShopId { get; set; }
        public E_DeviceBindType BindType { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string RemarkByDev { get; set; }
    }
}
