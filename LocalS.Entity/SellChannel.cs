using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LocalS.Entity
{
    public enum E_SellChannelRefType
    {
        Unknow = 0,
        Express = 1,
        SelfTake = 2,
        Machine = 3
    }

    [Table("SellChannel")]
    public class SellChannel
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string RefName { get; set; }
        public E_SellChannelRefType RefType { get; set; }
        public string RefId { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
