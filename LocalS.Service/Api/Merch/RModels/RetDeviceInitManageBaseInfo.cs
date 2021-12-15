using LocalS.BLL.Biz;
using LocalS.Service.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetDeviceInitManageBaseInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CumCode { get; set; }
        public string LogoImgUrl { get; set; }
        public string LastRequestTime { get; set; }
        public FieldModel Status { get; set; }
        public string AppVersion { get; set; }
        public string CtrlSdkVersion { get; set; }
        public string ShopId { get; set; }
        public string ShopName { get; set; }
        public bool IsStopUse { get; set; }
        public List<OptionNode> OptionsShop { get; set; }
    }
}
