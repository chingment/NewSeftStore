using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Account
{
    public class HomeService : BaseService
    {
        public CustomJsonResult GetIndexPageData(string operater, string userId)
        {
            var result = new CustomJsonResult();

            var ret = new RetHomeGetIndexPageData();


            var sysUser = CurrentDb.SysUser.Where(m => m.Id == userId).FirstOrDefault();

            switch (sysUser.BelongType)
            {
                case Lumos.DbRelay.Enumeration.BelongType.Agent:
                    ret.Appcaltions.Add(new RetHomeGetIndexPageData._Appcaltion() { Name = "商户代理系统", Url = "http://agent.17fanju.com/", ImgUrl = "http://file.17fanju.com/Upload/img_merch.png", Describe = "商家使用", });
                    break;
                case Lumos.DbRelay.Enumeration.BelongType.Merch:
                    ret.Appcaltions.Add(new RetHomeGetIndexPageData._Appcaltion() { Name = "商户运营系统", Url = "http://merch.17fanju.com/", ImgUrl = "http://file.17fanju.com/Upload/img_merch.png", Describe = "商家使用", });
                    break;
                case Lumos.DbRelay.Enumeration.BelongType.Admin:
                    ret.Appcaltions.Add(new RetHomeGetIndexPageData._Appcaltion() { Name = "后台管理系统", Url = "http://admin.17fanju.com/", ImgUrl = "http://file.17fanju.com/Upload/img_admin.png", Describe = "后端用户，公司内部使用", });
                    break;
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
