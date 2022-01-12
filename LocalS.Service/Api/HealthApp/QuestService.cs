using LocalS.BLL;
using LocalS.BLL.Biz;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.HealthApp
{
    public class QuestService : BaseService
    {
        //初始页面-资料填写
        public CustomJsonResult InitFill(string operater, string userId)
        {
            var ret = new
            {
                AppInfo = BizFactory.Senviv.GetWxAppInfoByUserId(userId),
            };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult Fill(string operater, string userId, RopQuestFill rop)
        {

            var d_UserDevice = CurrentDb.SenvivUserDevice.Where(m => m.SvUserId == userId).FirstOrDefault();

            if (d_UserDevice != null)
            {
                d_UserDevice.InfoFillTime = DateTime.Now;
                d_UserDevice.BindTime = DateTime.Now;
                d_UserDevice.BindStatus = Entity.SenvivUserDeviceBindStatus.Bind;
                d_UserDevice.Mender = operater;
                d_UserDevice.MendTime = DateTime.Now;
                CurrentDb.SaveChanges();
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
        }
    }
}
