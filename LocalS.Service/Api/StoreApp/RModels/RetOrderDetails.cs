using LocalS.Entity;
using LocalS.BLL.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RetOrderDetails
    {
        public RetOrderDetails()
        {
            this.Tag = new FsTag();
            this.Blocks = new List<FsBlock>();
            this.FieldBlocks = new List<FsBlockByField>();
        }

        public string Id { get; set; }
        public E_OrderStatus Status { get; set; }
        public FsTag Tag { get; set; }
        public List<FsBlock> Blocks { get; set; }
        public List<FsBlockByField> FieldBlocks { get; set; }
    }
}
