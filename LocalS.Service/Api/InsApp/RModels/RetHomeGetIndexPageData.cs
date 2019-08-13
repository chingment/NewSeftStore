using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.InsApp
{
    public class RetHomeGetIndexPageData
    {
        public RetHomeGetIndexPageData()
        {
            this.LNavGrids = new List<LNavGridModel>();
            this.Swiper = new SwiperModel();
        }

        public List<LNavGridModel> LNavGrids { get; set; }

        public SwiperModel Swiper { get; set; }
    }
}
