using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopStoreAdd
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string BriefDes { get; set; }
        public List<ImgSet> DispalyImgUrls { get; set; }
    }
}
