using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.HealthApp
{
    public class DeviceService : BaseService
    {
        public CustomJsonResult InitBind(string operater, string userId)
        {
            var result = new CustomJsonResult();

            var d_SenvivUser = CurrentDb.SenvivUser.Where(m => m.Id == userId).FirstOrDefault();

            var ret = new
            {
                UserInfo = new
                {
                    NickName = d_SenvivUser.NickName
                }

            };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult InitInfo(string operater, string userId)
        {
            return null;
        }
    }
}
