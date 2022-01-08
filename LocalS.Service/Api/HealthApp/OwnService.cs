using LocalS.BLL;
using LocalS.Entity;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.HealthApp
{
    public class OwnService : BaseService
    {
        public CustomJsonResult<SenvivUser> AuthInfo(string operater, string merchId, string openId, string nickName, string headImg)
        {
            var result = new CustomJsonResult();

            var d_SenvivUser = CurrentDb.SenvivUser.Where(m => m.WxOpenId == openId).FirstOrDefault();
            if (d_SenvivUser == null)
            {
                d_SenvivUser = new Entity.SenvivUser();
                d_SenvivUser.Id = IdWorker.Build(IdType.NewGuid);
                d_SenvivUser.MerchId = merchId;
                d_SenvivUser.WxOpenId = openId;
                d_SenvivUser.NickName = nickName;
                d_SenvivUser.Avatar = headImg;
                d_SenvivUser.CreateTime = DateTime.Now;
                d_SenvivUser.Creator = operater;
                CurrentDb.SenvivUser.Add(d_SenvivUser);
                CurrentDb.SaveChanges();
            }
            else
            {
                d_SenvivUser.NickName = nickName;
                d_SenvivUser.Avatar = headImg;
                d_SenvivUser.MendTime = DateTime.Now;
                d_SenvivUser.Mender = operater;
                CurrentDb.SaveChanges();
            }

            return new CustomJsonResult<SenvivUser>(ResultType.Success,ResultCode.Success,"", d_SenvivUser);
        }



        public CustomJsonResult InitInfo(string operater, string userId)
        {
            return null;
        }
    }
}
