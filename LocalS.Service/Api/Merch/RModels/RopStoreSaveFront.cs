using LocalS.BLL.Biz;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopStoreSaveFront
    {
        public RopStoreSaveFront()
        {
            this.AddressPoint = new MapPoint();
        }
        public string Id { get; set; }
        public string StoreId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string BriefDes { get; set; }
        public List<ImgSet> DisplayImgUrls { get; set; }
        public bool IsOpen { get; set; }
        public MapPoint AddressPoint { get; set; }
    }
}
