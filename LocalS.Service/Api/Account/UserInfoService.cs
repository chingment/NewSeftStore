using LocalS.BLL;
using Lumos;
using Lumos.DbRelay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.Account
{
    public class UserInfoService : BaseDbContext
    {
        public CustomJsonResult Save(string operater, string userId, RopUserInfoSave rop)
        {

            CustomJsonResult result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {
                var user = CurrentDb.SysUser.Where(m => m.Id == userId).FirstOrDefault();

                if (!string.IsNullOrEmpty(rop.Password))
                {
                    user.PasswordHash = PassWordHelper.HashPassword(rop.Password);
                }

                user.FullName = rop.FullName;
                user.Email = rop.Email;
                user.MendTime = DateTime.Now;
                user.Mender = operater;


                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            }


            return result;


        }
    }
}
