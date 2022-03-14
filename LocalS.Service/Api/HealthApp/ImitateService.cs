using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.HealthApp
{
    public class ImitateService : BaseService
    {
        public CustomJsonResult LyingIn(string operater, string userId)
        {
            var d_ClientUser = CurrentDb.SysClientUser.Where(m => m.Id == userId).FirstOrDefault();

            DateTime dt = DateTime.Parse("2020-01-01 " + DateTime.Now.ToString("HH:mm:ss"));

            var d_SvImitateLyingIns = CurrentDb.SvImitateLyingIn.Where(m => m.MerchId == d_ClientUser.MerchId && m.StartTimeQt <= dt && m.EndTimeQt >= dt).ToList();

            if (d_SvImitateLyingIns != null && d_SvImitateLyingIns.Count > 0)
            {
                Random rd = new Random();
                int index = rd.Next(d_SvImitateLyingIns.Count);
                var model = d_SvImitateLyingIns[index];

                var ret = new
                {
                    Article = new
                    {
                        Title = model.Title,
                        Content = model.Content
                    }

                };

                return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
            }


            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "");
        }
    }
}
