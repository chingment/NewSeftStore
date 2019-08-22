using LocalS.Service.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class OrderModel
    {
        public OrderModel()
        {
            this.Tag = new FsTag();
            this.Tip = new FsText();
            this.Blocks = new List<FsBlock>();
            this.Buttons = new List<FsButton>();
        }
        public string Id { get; set; }

        public string Sn { get; set; }

        public FsTag Tag { get; set; }

        public List<FsBlock> Blocks { get; set; }

        public string ChargeAmount { get; set; }
        
        public FsText Tip { get; set; }

        public List<FsButton> Buttons { get; set; }


    }
}
