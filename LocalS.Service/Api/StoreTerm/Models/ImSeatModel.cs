using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class ImSeatModel
    {
        public ImSeatModel()
        {
            this.CharTags = new List<string>();
        }

        public string UserId { get; set; }
        public string NickName { get; set; }
        public string Avatar { get; set; }
        public List<string> CharTags { get; set; }
        public string BriefDes { get; set; }
        public string ImUserName { get; set; }
        public string ImStatus { get; set; }
    }
}
