using LocalS.BLL.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RetPersonalPageData
    {
        public UserInfoModel UserInfo { get; set; }

        public Badge BadgeByWaitPayOrders { get; set; }

        public ProServiceModel ProService { get; set; }
    }
}
