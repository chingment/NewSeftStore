using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Account
{
    public class HomeService : BaseDbContext
    {
        public CustomJsonResult GetIndexPageData(string operater, string userId)
        {
            var result = new CustomJsonResult();

            var ret = new RetHomeGetIndexPageData();


            var sysUser = CurrentDb.SysUser.Where(m => m.Id == userId).FirstOrDefault();

            switch(sysUser.BelongSite)
            {
                case Lumos.DbRelay.Enumeration.BelongSite.Agent:
                    ret.Appcaltions.Add(new RetHomeGetIndexPageData._Appcaltion() { Name = "商户代理系统", Url = "http://agent.ins-uplink.com/", ImgUrl = "http://file.17fanju.com/Upload/img_merch.png", Describe = "商家客户使用", });
                    break;
                case Lumos.DbRelay.Enumeration.BelongSite.Admin:
                    ret.Appcaltions.Add(new RetHomeGetIndexPageData._Appcaltion() { Name = "后台管理系统", Url = "http://admin.ins-uplink.com/", ImgUrl = "http://file.17fanju.com/Upload/img_admin.png", Describe = "后端用户，公司内部使用", });
                    break;
            }
           
            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);

            return result;
        }
    }
}
