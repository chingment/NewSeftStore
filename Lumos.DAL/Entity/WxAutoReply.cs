using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.DbRelay
{

    public enum E_WxAutoReplyType
    {
        Unknow = 0,
        Subscribe = 1,
        Keyword = 2,
        MenuClick = 3,
        NotKeyword = 4
    }

    [Table("WxAutoReply")]
    public class WxAutoReply
    {
        [Key]
        public string Id { get; set; }

        public E_WxAutoReplyType Type { get; set; }

        public string Keyword { get; set; }

        public string ReplyContent { get; set; }

        public bool IsDelete { get; set; }

        public string Creator { get; set; }

        public DateTime CreateTime { get; set; }

        public string Mender { get; set; }

        public DateTime? MendTime { get; set; }
    }
}
