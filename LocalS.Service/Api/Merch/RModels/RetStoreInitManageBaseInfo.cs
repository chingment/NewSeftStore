using LocalS.BLL.Biz;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetStoreInitManageBaseInfo
    {
        public RetStoreInitManageBaseInfo()
        {

        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string ContactName { get; set; }
        public string ContactAddress { get; set; }
        public string ContactPhone { get; set; }
        public string MainImgUrl { get; set; }
        public string BriefDes { get; set; }
        public string SctMode { get; set; }

    }
}
