using LocalS.Service.Api.HealthApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiHealthApp.Controllers
{
    public class DeviceController : OwnApiBaseController
    {
        //初始页面-设备绑定
        [HttpGet]
        public OwnApiHttpResponse InitBind(string deviceId = null, string requestUrl = null)
        {
            var result = HealthAppServiceFactory.Device.InitBind(this.CurrentUserId, this.CurrentUserId, deviceId, requestUrl);
            return new OwnApiHttpResponse(result);
        }

        //初始页面-设备信息
        [HttpGet]
        public OwnApiHttpResponse InitManage()
        {
            var result = HealthAppServiceFactory.Device.InitManage(this.CurrentUserId, this.CurrentUserId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitInfo(string deviceId)
        {
            var result = HealthAppServiceFactory.Device.InitInfo(this.CurrentUserId, this.CurrentUserId, deviceId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse InfoEdit(RopDeviceInfoEdit rop)
        {
            var result = HealthAppServiceFactory.Device.InfoEdit(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse BindSerialNo(RopDeviceBindSerialNo rop)
        {
            var result = HealthAppServiceFactory.Device.BindSerialNo(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse BindPhoneNumber(RopDeviceBindPhoneNumber rop)
        {
            var result = HealthAppServiceFactory.Device.BindPhoneNumber(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse UnBind(RopDeviceUnBind rop)
        {
            var result = HealthAppServiceFactory.Device.UnBind(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse GetPhoneValidCode(RopOwnGetPhoneVaildCode rop)
        {
            var result = HealthAppServiceFactory.Device.GetPhoneValidCode(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }


        [HttpGet]
        public OwnApiHttpResponse InitFill()
        {
            var result = HealthAppServiceFactory.Device.InitFill(this.CurrentUserId, this.CurrentUserId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Fill(RopDeviceFill rop)
        {
            var result = HealthAppServiceFactory.Device.Fill(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }


    }
}
