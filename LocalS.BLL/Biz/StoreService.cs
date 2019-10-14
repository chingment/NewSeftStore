using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class StoreService : BaseDbContext
    {

        public StoreInfoModel GetOne(string id)
        {
            var model = new StoreInfoModel();

            return model;
        }


        public List<StoreInfoModel> GetAll(string merchId)
        {
            var model = new List<StoreInfoModel>();

            return model;
        }
    }
}
