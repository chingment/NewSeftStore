using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreSvcChat
{
    public class OwnService : BaseDbContext
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


            var machines = CurrentDb.Machine.Where(m => rop.UserNames.Contains(m.ImUserName) && m.ImUserName != null).ToList();

            foreach (var machine in machines)
            {
                var isExt = ret.Contacts.Where(m => m.Username == machine.ImUserName).FirstOrDefault();
                if (isExt == null)
                {
                    ret.Contacts.Add(new RetOwnGetContactInfos.ContactModel { Username = machine.ImUserName, Nickname = "[机器]" + machine.Id, Avatar = machine.LogoImgUrl });
                }
            }


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

        }
    }
}
