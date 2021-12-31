using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopSenvivSaveUserDetail
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
        public DateTime? Birthday { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public E_SenvivUserCareMode CareMode { get; set; }
        public Dictionary<string, string> Pregnancy { get; set; }
    }
}
