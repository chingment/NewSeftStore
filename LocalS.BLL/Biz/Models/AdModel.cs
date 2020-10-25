using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class AdModel
    {
        public AdModel()
        {
            this.Contents = new List<AdContentModel>();

        }

        public E_AdSpaceId AdId { get; set; }

        public string Name { get; set; }

        public List<AdContentModel> Contents { get; set; }
    }
}
