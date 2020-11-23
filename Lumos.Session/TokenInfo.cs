using System;
using Lumos.DbRelay;

namespace Lumos.Session
{
    public class TokenInfo
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string BelongId { get; set; }
        public Enumeration.BelongType BelongType { get; set; }
        public string MctMode { get; set; }
    }
}
