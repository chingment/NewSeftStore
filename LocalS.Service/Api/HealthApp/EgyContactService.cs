using LocalS.BLL;
using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.HealthApp
{
    public class EgyContactService : BaseService
    {
        public CustomJsonResult GetDetails(string operater, string userId, string contactId)
        {
            var d_Contact = CurrentDb.SysUserContact.Where(m => m.Id == contactId && m.UserId == userId).FirstOrDefault();
            if (d_Contact == null)
              return  new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到");


            var ret = new
            {
                Id = d_Contact.Id,
                FullName = d_Contact.FullName,
                PhoneNumber = d_Contact.PhoneNumber
            };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult Save(string operater, string userId, RopEgyContactSave rop)
        {
            if (string.IsNullOrEmpty(rop.Id))
            {
                var d_SysUserContact = new SysUserContact();
                d_SysUserContact.Id = IdWorker.Build(IdType.NewGuid);
                d_SysUserContact.UserId = userId;
                d_SysUserContact.Type = 1;
                d_SysUserContact.IsEnable = true;
                d_SysUserContact.FullName = rop.FullName;
                d_SysUserContact.PhoneNumber = rop.PhoneNumber;
                d_SysUserContact.CreateTime = DateTime.Now;
                d_SysUserContact.Creator = operater;
                CurrentDb.SysUserContact.Add(d_SysUserContact);
                CurrentDb.SaveChanges();

            }
            else
            {
                var d_SysUserContact = CurrentDb.SysUserContact.Where(m => m.UserId == userId && m.Id == rop.Id).FirstOrDefault();
                d_SysUserContact.FullName = rop.FullName;
                d_SysUserContact.PhoneNumber = rop.PhoneNumber;
                d_SysUserContact.IsEnable = rop.IsEnable;
                d_SysUserContact.MendTime = DateTime.Now;
                d_SysUserContact.Mender = operater;
                CurrentDb.SaveChanges();
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
        }
    }
}
