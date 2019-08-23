using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LocalS.Entity
{
    public enum E_StoreSellChannelRefType
    {
        Unknow = 0,
        Express = 1,
        SelfTake = 2,
        Machine = 3
    }

    [Table("StoreSellChannel")]
    public class StoreSellChannel
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string Name { get; set; }
        public E_StoreSellChannelRefType RefType { get; set; }
        public string RefId { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
