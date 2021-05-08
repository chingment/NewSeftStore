using LocalS.BLL;
using LocalS.Service.Api.Account;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiAccount.Controllers
{
    public class OwnController : OwnApiBaseController
    {

        public string FormatAppId(string appId)
        {
            if (string.IsNullOrEmpty(appId))
                return AppId.ACCOUNT;

            appId = appId.ToLower();

            if (appId == AppId.ADMIN)
            {
                return AppId.ADMIN;
            }
            else if (appId == AppId.MERCH)
            {
                return AppId.MERCH;
            }
            else if (appId == AppId.AGENT)
            {
                return AppId.AGENT;
            }

            return AppId.ACCOUNT;
        }
        [HttpPost]
        [AllowAnonymous]
        public OwnApiHttpResponse LoginByAccount([FromBody]RopOwnLoginByAccountInWebSite rop)
        {

            LogUtil.Info("RedirectUrl:" + rop.RedirectUrl);

            var myRop = new RopOwnLoginByAccount();
            myRop.UserName = rop.UserName;
            myRop.Password = rop.Password;
            myRop.Ip = CommonUtil.GetIpAddress(this.HttpRequest);
            myRop.LoginPms = rop.LoginPms;
            myRop.AppId = FormatAppId(rop.AppId);
            myRop.LoginWay = Lumos.DbRelay.Enumeration.LoginWay.Website;
            IResult result = AccountServiceFactory.Own.LoginByAccount(myRop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetInfo([FromUri]RupOwnGetInfo rup)
        {
            var result = AccountServiceFactory.Own.GetInfo(this.CurrentUserId, this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Logout([FromBody]RopOwnLogout rop)
        {
            if (rop == null)
            {
                rop = new RopOwnLogout();
            }

            rop.Token = this.Token;
            rop.AppId = rop.AppId;
            rop.Ip = CommonUtil.GetIP();
            rop.BelongId = this.BelongId;
            rop.LoginWay = Lumos.DbRelay.Enumeration.LoginWay.Website;
            IResult result = AccountServiceFactory.Own.Logout(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse CheckPermission([FromUri]RupOwnCheckPermission rup)
        {
            IResult result = AccountServiceFactory.Own.CheckPermission(this.CurrentUserId, this.CurrentUserId, this.Token, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse UploadFingerVeinData([FromUri]LocalS.Service.Api.Account.RopUploadFingerVeinData rop)
        {
            var result = LocalS.Service.Api.Account.AccountServiceFactory.Own.UploadFingerVeinData(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

    }
}
