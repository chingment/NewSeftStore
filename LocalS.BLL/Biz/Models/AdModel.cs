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
            this.Contents = new List<ContentModel>();

        }

        public E_AdSpaceId AdId { get; set; }

        public string Name { get; set; }

        public List<ContentModel> Contents { get; set; }

        public class ContentModel
        {
            public string DataType { get; set; }
            public string DataUrl { get; set; }
        }
    }
}
