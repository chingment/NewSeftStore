using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetStoreInitManage
    {
        public RetStoreInitManage()
        {
            this.CurStore = new StoreModel();
            this.Stores = new List<StoreModel>();
        }

        public StoreModel CurStore { get; set; }

        public List<StoreModel> Stores { get; set; }
    }
}
