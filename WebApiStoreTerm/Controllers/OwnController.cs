using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiStoreTerm.Controllers
{
    public class OwnController : OwnApiBaseController
    {
        [HttpPost]
        public OwnApiHttpResponse GetInfo([FromBody]LocalS.Service.Api.Account.RupOwnGetInfo rup)
        {
            var result = LocalS.Service.Api.Account.AccountServiceFactory.Own.GetInfo(this.CurrentUserId, this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse UploadFingerVeinData([FromBody]LocalS.Service.Api.Account.RopUploadFingerVeinData rop)
        {
            var result = LocalS.Service.Api.Account.AccountServiceFactory.Own.UploadFingerVeinData(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse DeleteFingerVeinData()
        {
            var result = LocalS.Service.Api.Account.AccountServiceFactory.Own.DeleteFingerVeinData(this.CurrentUserId, this.CurrentUserId);
            return new OwnApiHttpResponse(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public OwnApiHttpResponse LoginByFingerVein([FromBody]LocalS.Service.Api.StoreTerm.RopOwnLoginByFingerVein rop)
        {
            var _rop = new LocalS.Service.Api.Account.RopOwnLoginByFingerVein();
            _rop.VeinData = rop.VeinData;
            _rop.LoginWay = rop.LoginWay;
            _rop.AppId = rop.AppId;
            _rop.Ip = CommonUtil.GetIP();
            _rop.LoginPms.Add("deviceId", rop.DeviceId);

            var result = LocalS.Service.Api.Account.AccountServiceFactory.Own.LoginByFingerVein(_rop);
            return new OwnApiHttpResponse(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public OwnApiHttpResponse LoginByAccount([FromBody]LocalS.Service.Api.StoreTerm.RopOwnLoginByAccount rop)
        {
            var _rop = new LocalS.Service.Api.Account.RopOwnLoginByAccount();
            _rop.UserName = rop.UserName;
            _rop.Password = rop.Password;
            _rop.LoginWay = rop.LoginWay;
            _rop.AppId = rop.AppId;
            _rop.Ip = CommonUtil.GetIP();
            _rop.LoginPms.Add("deviceId", rop.DeviceId);
            var result = LocalS.Service.Api.Account.AccountServiceFactory.Own.LoginByAccount(_rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Logout([FromBody]LocalS.Service.Api.StoreTerm.RopOwnLogout rop)
        {
            var _rop = new LocalS.Service.Api.Account.RopOwnLogout();

            _rop.Ip = CommonUtil.GetIP();
            _rop.AppId = AppId.STORETERM;
            _rop.Token = this.Token;
            _rop.LoginPms.Add("deviceId", rop.DeviceId);
            _rop.BelongId = rop.DeviceId;

            var result = LocalS.Service.Api.Account.AccountServiceFactory.Own.Logout(this.CurrentUserId, this.CurrentUserId, _rop);
            return new OwnApiHttpResponse(result);
        }

    }
}
