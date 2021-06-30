using LocalS.BLL;
using LocalS.Service.Api.StoreTerm;
using Lumos;
using System;
using System.IO;
using System.Web;
using System.Web.Http;


namespace WebApiStoreTerm.Controllers
{
    [OwnApiAuthorize]
    public class DeviceController : OwnApiBaseController
    {
        [HttpPost]
        public OwnApiHttpResponse InitData([FromBody]RopDeviceInitData rop)
        {
            var result = StoreTermServiceFactory.Device.InitData(rop);
            return new OwnApiHttpResponse(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public OwnApiHttpResponse ByPoint([FromBody]LocalS.BLL.Biz.RopByPoint rop)
        {
            var result = LocalS.BLL.Biz.BizFactory.ByPoint.Record(this.CurrentUserId, rop);

            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public OwnApiHttpResponse CheckUpdate([FromBody]RopDeviceCheckUpdate rup)
        {
            var result = StoreTermServiceFactory.Device.CheckUpdate(rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse EventNotify([FromBody]RopDeviceEventNotify rop)
        {
            var result = StoreTermServiceFactory.Device.EventNotify(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse GetRunExHandleItems([FromBody]RopDeviceGetRunExHandleItems rup)
        {
            var result = StoreTermServiceFactory.Device.GetRunExHandleItems(this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse HandleRunExItems([FromBody]RopDeviceHandleRunExItems rop)
        {
            rop.AppId = AppId.STORETERM;
            var result = StoreTermServiceFactory.Device.HandleRunExItems(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }


        public OwnApiHttpResponse Upload()
        {
            LogUtil.Info("调用Upload.Post");

            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
            HttpRequestBase request = context.Request;//定义传统request对象 
            CustomJsonResult r = new CustomJsonResult();


            try
            {
                string domain = System.Configuration.ConfigurationManager.AppSettings["custom:FilesServerUrl"];
                string rootPath = System.Configuration.ConfigurationManager.AppSettings["custom:FileServerUploadPath"];

                LogUtil.Info("文件Count:" + request.Files.Count);
                for (var i = 0; i < request.Files.Count; i++)
                {
                    LogUtil.Info("文件名称:" + request.Files[i].FileName);
                }

                LogUtil.Info("表单Count:" + request.Form.Count);
                for (var i = 0; i < request.Form.Count; i++)
                {
                    LogUtil.Info("文件名称:" + request.Form[i]);
                }


                string folder = "Common";                //默认保存在 Common 文件夹
                string fileName = Guid.NewGuid().ToString();  //默认文件名称

                if (request.Form["folder"] != null)
                {
                    string l_folder = request.Form["folder"].ToString();
                    if (!string.IsNullOrEmpty(l_folder))
                    {
                        folder = l_folder;
                    }
                }

                LogUtil.Info("folder:" + folder);

                if (request.Form["fileName"] != null)
                {
                    string l_fileName = request.Form["fileName"].ToString();
                    if (!string.IsNullOrEmpty(l_fileName))
                    {
                        fileName = l_fileName;
                    }
                }


                LogUtil.Info("fileName:" + fileName);

                string savePath = "/Upload/" + folder;

                LogUtil.Info("savePath:" + savePath);

                DirectoryInfo dir = new DirectoryInfo(rootPath + "/" + savePath);
                if (!dir.Exists)
                {
                    dir.Create();
                }

                byte[] fileData = null;
                using (var binaryReader = new BinaryReader(request.Files[0].InputStream))
                {
                    fileData = binaryReader.ReadBytes(request.Files[0].ContentLength);
                }
                if (fileData != null && fileData.Length > 0)
                {
                    string extension = Path.GetExtension(request.Files[0].FileName);

                    LogUtil.Info("extension:" + extension);


                    string serverSavePath = rootPath + "/" + savePath + "/" + fileName + extension;
                    string domainPathUrl = domain + "/" + savePath + "/" + fileName + extension;

                    LogUtil.Info("serverSavePath:" + serverSavePath);
                    LogUtil.Info("domainPathUrl:" + domainPathUrl);

                    FileStream fs = new FileStream(serverSavePath, FileMode.Create, FileAccess.Write);
                    fs.Write(fileData, 0, fileData.Length);
                    fs.Flush();
                    fs.Close();

                    r.Result = ResultType.Success;
                    r.Code = ResultCode.Success;
                    r.Data = new { name = fileName, url = domainPathUrl };
                    r.Message = "上传成功";
                }
            }
            catch (Exception ex)
            {
                r.Code = ResultCode.Exception;
                r.Result = ResultType.Exception;
                r.Message = "上传失败";
                LogUtil.Error("WebApi上传图片异常", ex);
            }

            return new OwnApiHttpResponse(r);
        }
    }
}