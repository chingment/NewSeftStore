﻿using LocalS.BLL;
using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.InsApp
{
    public class OwnService : BaseService
    {
        public CustomJsonResult LoginByUrlParams(string mId, string tppId)
        {
            var result = new CustomJsonResult();
            var ret = new RetOwnLoginByUrlParams();


            if (string.IsNullOrEmpty(mId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "您好，应用无法访问，造成的原因：商户标识参数为空");
            }

            if (string.IsNullOrEmpty(tppId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "您好，应用无法访问，造成的原因：用户标识参数为空");
            }

            var agent = CurrentDb.Agent.Where(m => m.Id == mId).FirstOrDefault();

            if (agent == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "您好，应用无法访问，造成的原因：商户信息无法解释");
            }


            var agentUser = CurrentDb.SysAgentUser.Where(m => m.AgentId == mId && m.TppId == tppId).FirstOrDefault();
            if (agentUser == null)
            {
                agentUser = new SysAgentUser();
                agentUser.Id = IdWorker.Build(IdType.NewGuid);
                agentUser.UserName = IdWorker.Build(IdType.NewGuid);
                agentUser.PasswordHash = PassWordHelper.HashPassword("Caskujn");
                agentUser.SecurityStamp = IdWorker.Build(IdType.NewGuid);
                agentUser.RegisterTime = DateTime.Now;
                agentUser.IsDisable = false;
                agentUser.BelongType = Enumeration.BelongType.Agent;
                agentUser.CreateTime = DateTime.Now;
                agentUser.Creator = agentUser.Id;
                agentUser.AgentId = agent.Id;
                agentUser.TppId = tppId;
                CurrentDb.SysAgentUser.Add(agentUser);
                CurrentDb.SaveChanges();
            }

            ret.MId = agentUser.AgentId;
            ret.UId = agentUser.Id;

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult LoginByAccount(RopOwnLoginByAccount rop)
        {
            var result = new CustomJsonResult();
            var ret = new RetOwnLoginByUrlParams();

            var agentUser = CurrentDb.SysAgentUser.Where(m => m.UserName == rop.UserName).FirstOrDefault();

            if (agentUser == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "账号不存在");
            }

            if (!PassWordHelper.VerifyHashedPassword(agentUser.PasswordHash, rop.Password))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "账号密码不正确");
            }

            if (agentUser.IsDisable)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该账号已被禁用");
            }

            ret.MId = agentUser.AgentId;
            ret.UId = agentUser.Id;

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
