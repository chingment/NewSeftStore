using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("SvImitateLyingIn")]
    public class SvImitateLyingIn
    {
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime StartTimeQt { get; set; }
        public DateTime EndTimeQt { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
