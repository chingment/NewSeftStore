using LocalS.BLL.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RopDeviceHandleRunExItems
    {
        public RopDeviceHandleRunExItems()
        {
            this.ExItems = new List<ExItem>();
            this.ExReasons = new List<ExReason>();
        }

        public string MerchId { get; set; }
        public string DeviceId { get; set; }
        public List<ExItem> ExItems { get; set; }
        public List<ExReason> ExReasons { get; set; }
        public string AppId { get; set; }
    }
}
