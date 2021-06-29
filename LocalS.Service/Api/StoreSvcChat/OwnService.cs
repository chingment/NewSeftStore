using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreSvcChat
{
    public class OwnService : BaseService
    {
        public CustomJsonResult GetContactInfos(string operater, string userId, RopOwnGetContactInfos rop)
        {

            var ret = new RetOwnGetContactInfos();

            if (rop.UserNames == null && rop.UserNames.Count == 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "");
            }


            var sysUsers = CurrentDb.SysUser.Where(m => rop.UserNames.Contains(m.UserName)).ToList();

            foreach (var sysUser in sysUsers)
            {
                var isExt = ret.Contacts.Where(m => m.Username == sysUser.UserName).FirstOrDefault();
                if (isExt == null)
                {
                    ret.Contacts.Add(new RetOwnGetContactInfos.ContactModel { Username = sysUser.UserName, Nickname = sysUser.NickName, Avatar = sysUser.Avatar });
                }
            }


            var d_Devices = CurrentDb.Device.Where(m => rop.UserNames.Contains(m.ImUserName) && m.ImUserName != null).ToList();

            foreach (var d_Device in d_Devices)
            {
                var isExt = ret.Contacts.Where(m => m.Username == d_Device.ImUserName).FirstOrDefault();
                if (isExt == null)
                {
                    ret.Contacts.Add(new RetOwnGetContactInfos.ContactModel { Username = d_Device.ImUserName, Nickname = "[设备]" + d_Device.Id, Avatar = d_Device.LogoImgUrl });
                }
            }


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

        }
    }
}
