using log4net;
using Lumos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Http;

namespace WebUploadServer.Controllers
{
    public class UploadController : ApiController
    {
        private static ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public HttpResponseMessage Post()
        {
            log.Info("调用Upload.Post");

            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
            HttpRequestBase request = context.Request;//定义传统request对象 
            CustomJsonResult r = new CustomJsonResult();


            try
            {
                string domain = System.Configuration.ConfigurationManager.AppSettings["custom:FilesServerUrl"];
                string rootPath = System.Configuration.ConfigurationManager.AppSettings["custom:FileServerUploadPath"];

                log.Info("文件Count:" + request.Files.Count);
                for (var i = 0; i < request.Files.Count; i++)
                {
                    log.Info("文件名称:" + request.Files[i].FileName);
                }

                log.Info("表单Count:" + request.Form.Count);
                for (var i = 0; i < request.Form.Count; i++)
                {
                    log.Info("文件名称:" + request.Form[i]);
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

                log.Info("folder:" + folder);

                if (request.Form["fileName"] != null)
                {
                    string l_fileName = request.Form["fileName"].ToString();
                    if (!string.IsNullOrEmpty(l_fileName))
                    {
                        fileName = l_fileName;
                    }
                }

    
                log.Info("fileName:" + fileName);
            
                string savePath = "/Upload/" + folder;

                log.Info("savePath:" + savePath);

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

                    log.Info("extension:" + extension);


                    string serverSavePath = rootPath + "/" + savePath + "/" + fileName + extension;
                    string domainPathUrl = domain + "/" + savePath + "/" + fileName + extension;

                    log.Info("serverSavePath:" + serverSavePath);
                    log.Info("domainPathUrl:" + domainPathUrl);

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
                log.Error("WebApi上传图片异常", ex);
            }

            string json = r.ToString();
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(json, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }
    }
}
