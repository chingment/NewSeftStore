using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopAdContentCopy2Belongs
    {
        public string AdContentId { get; set; }
        public List<string> BelongIds { get; set; }
        public string[] ValidDate { get; set; }
    }
}
