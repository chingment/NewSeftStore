using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Account
{
    public class LoginLogService : BaseService
    {
        public string GetLoginWayText(Lumos.DbRelay.Enumeration.LoginWay loginWay)
        {
            string text = "";
            switch (loginWay)
            {
                case Lumos.DbRelay.Enumeration.LoginWay.Website:
                    text = "网站";
                    break;
                case Lumos.DbRelay.Enumeration.LoginWay.Android:
                    text = "安卓";
                    break;
                case Lumos.DbRelay.Enumeration.LoginWay.Ios:
                    text = "苹果";
                    break;
                case Lumos.DbRelay.Enumeration.LoginWay.Term:
                    text = "终端设备";
                    break;
                case Lumos.DbRelay.Enumeration.LoginWay.Wxmp:
                    text = "微信小程序";
                    break;
            }
            return text;
        }
        public CustomJsonResult GetList(string operater, string userId, RupLoginLogGetList rup)
        {
            var result = new CustomJsonResult();

            DateTime? startDate = CommonUtil.ConverToStartTime(rup.StartDate);
            DateTime? endDate = CommonUtil.ConverToEndTime(rup.EndDate);

            var query = (from u in CurrentDb.SysUserLoginHis
                         where u.UserId == userId&&
                          (startDate == null || u.LoginTime >= startDate) &&
                          (endDate == null || u.LoginTime <= endDate)
                         select new { u.Id, u.LoginTime, u.Ip, u.City, u.LoginWay, u.Description });


            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderByDescending(r => r.LoginTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {

                olist.Add(new
                {
                    Id = item.Id,
                    LoginTime = item.LoginTime.ToUnifiedFormatDateTime(),
                    LoginWay = GetLoginWayText(item.LoginWay),
                    Ip = item.Ip,
                    Location = item.City,
                    Description = item.Description
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }
    }
}
