using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.InsApp
{
    public class SwiperModel
    {
        public SwiperModel()
        {
            this.Imgs = new List<ImgModel>();
        }

        public List<ImgModel> Imgs { get; set; }
    }
}
