﻿using LocalS.BLL;
using LocalS.BLL.Mq;
using LocalS.Entity;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.Merch
{
    public class ClientUserService : BaseService
    {
        public CustomJsonResult GetList(string operater, string merchId, RupClientGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.SysClientUser
                         where
                          (rup.UserName == null || u.UserName.Contains(rup.UserName)) &&
                         (rup.NickName == null || u.NickName.Contains(rup.NickName)) &&
                         (rup.PhoneNumber == null || u.PhoneNumber.Contains(rup.PhoneNumber)) &&
                         u.IsDelete == false &&
                         u.MerchId == merchId
                         select new { u.Id, u.UserName, u.NickName, u.Avatar, u.FullName, u.Email, u.PhoneNumber, u.CreateTime, u.IsDelete, u.IsDisable });


            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                string userName = item.UserName;
                if (userName.Length == 34 && userName.IndexOf("wx") > -1)
                {
                    userName = "未设置";
                }

                olist.Add(new
                {
                    Id = item.Id,
                    UserName = userName,
                    FullName = item.FullName,
                    Email = item.Email,
                    PhoneNumber = item.PhoneNumber,
                    Avatar = item.Avatar,
                    NickName = item.NickName,
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        public CustomJsonResult InitDetails(string operater, string merchId, string clientUserId)
        {
            var result = new CustomJsonResult();

            var ret = new RetClientUserInitManageBaseInfo();

            var clientUser = CurrentDb.SysClientUser.Where(m => m.Id == clientUserId).FirstOrDefault();

            ret.Id = clientUser.Id;
            ret.UserName = clientUser.UserName;
            ret.PhoneNumber = clientUser.PhoneNumber;
            ret.FullName = clientUser.FullName;
            ret.NickName = clientUser.NickName;
            ret.Avatar = clientUser.Avatar;


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult InitDetailsBaseInfo(string operater, string merchId, string clientUserId)
        {
            var ret = new RetClientUserInitManageBaseInfo();

            var clientUser = CurrentDb.SysClientUser.Where(m => m.Id == clientUserId).FirstOrDefault();

            ret.Id = clientUser.Id;
            ret.UserName = clientUser.UserName;
            ret.PhoneNumber = clientUser.PhoneNumber;
            ret.FullName = clientUser.FullName;
            ret.NickName = clientUser.NickName;
            ret.Avatar = clientUser.Avatar;
            ret.IsHasProm = clientUser.IsHasProm;
            ret.IsStaff = clientUser.IsStaff;

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public CustomJsonResult Edit(string operater, string merchId, RopClientUserEdit rop)
        {

            CustomJsonResult result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {
                var d_SysClientUser = CurrentDb.SysClientUser.Where(m => m.MerchId == merchId && m.Id == rop.Id).FirstOrDefault();


                d_SysClientUser.IsStaff = rop.IsStaff;
                d_SysClientUser.IsHasProm = rop.IsHasProm;

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

                MqFactory.Global.PushOperateLog(operater, AppId.MERCH, merchId, EventCode.clientuser_edit, string.Format("保存客户账号（{0}）信息成功", d_SysClientUser.UserName), rop);

            }


            return result;


        }

        public CustomJsonResult GetAvatars(string operater, string merchId, RupClientGetAvatars rup)
        {
            var result = new CustomJsonResult();

            var d_SysClientUsers = (from u in CurrentDb.SysClientUser
                                    where
                                    rup.ClientUserIds.Contains(u.Id)
                                    select new { u.Id, u.UserName, u.NickName, u.Avatar, u.FullName, u.Email, u.PhoneNumber, u.CreateTime, u.IsDelete, u.IsDisable }).ToList();

            List<object> olist = new List<object>();

            foreach (var item in d_SysClientUsers)
            {
                olist.Add(new
                {
                    Id = item.Id,
                    UserName = item.UserName,
                    FullName = item.FullName,
                    Email = item.Email,
                    PhoneNumber = item.PhoneNumber,
                    Avatar = item.Avatar,
                    NickName = item.NickName
                });
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", olist);

            return result;
        }
    }
}
