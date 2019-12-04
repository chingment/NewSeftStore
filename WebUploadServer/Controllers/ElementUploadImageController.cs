﻿using log4net;
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
using System.Web.Mvc;


namespace WebUploadServer.Controllers
{
    public class ElementUploadImageController : ApiController
    {
        public string GetUploadPath(string path)
        {
            string rootPath = "/Upload/";
            rootPath += path;
            return rootPath;
        }

        private bool GreateMiniImageModel(string originalpath, string savepath, int tWidth, int tHeight)
        {
            System.Drawing.Image SImage = System.Drawing.Image.FromFile(originalpath);
            try
            {
                SImage = System.Drawing.Image.FromFile(originalpath);
                int imgWidth = SImage.Width;
                int imgHeight = SImage.Height;
                int newWidth = 0;
                int newHeight = 0;
                if (imgWidth > tWidth)
                {
                    newWidth = tWidth;
                    newHeight = tWidth * imgHeight / imgWidth;
                    if (newHeight > tHeight)
                    {
                        newWidth = tHeight * newWidth / newHeight;
                        newHeight = tHeight;
                    }
                }
                else if (imgHeight > tHeight)
                {
                    newHeight = tHeight;
                    newWidth = tHeight * imgWidth / imgHeight;
                    if (newWidth > tWidth)
                    {
                        newHeight = tWidth * newHeight / newWidth;
                        newWidth = tWidth;
                    }
                }
                else
                {
                    newWidth = imgWidth;
                    newHeight = imgHeight;
                }
                Bitmap b = new Bitmap(SImage, newWidth, newHeight);

                b.Save(savepath);
                b.Dispose();
                SImage.Dispose();

                return true;
            }
            catch
            {
                if (SImage != null)
                {
                    SImage.Dispose();
                }
                return false;
            }

        }

        private static ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public HttpResponseMessage Post()
        {
            log.Info("调用ElementUploadImage");

            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
            HttpRequestBase request = context.Request;//定义传统request对象 
            CustomJsonResult r = new CustomJsonResult();
            try
            {
                log.Info("文件Count:" + request.Files.Count);
                for (var i = 0; i < request.Files.Count; i++)
                {
                    log.Info("文件名称:" + request.Files[i].FileName);
                }
                byte[] fileData = null;
                using (var binaryReader = new BinaryReader(request.Files[0].InputStream))
                {
                    fileData = binaryReader.ReadBytes(request.Files[0].ContentLength);
                }
                if (fileData != null && fileData.Length > 0)
                {
                    ImageUpload image = new ImageUpload();
                    int[] bigImgSize = new int[2] { 500, 500 };
                    int[] smallImgSize = new int[2] { 100, 100 };
                    string imageSign = "";

                    string fileName = request.Files[0].FileName;
                    string uploadFolder = "common";
                    string savefolder = GetUploadPath(uploadFolder);
                    string extension = Path.GetExtension(fileName).ToLower();
                    string yyyyMMddhhmmssfff = Guid.NewGuid().ToString().Replace("-", "");
                    string originalNewfilename = imageSign + yyyyMMddhhmmssfff + "_O" + extension;//原图片文件名称
                    string bigNewfilename = imageSign + yyyyMMddhhmmssfff + "_B" + extension;//大图片文件名称
                    string smallNewfilename = imageSign + yyyyMMddhhmmssfff + "_S" + extension;//小图片文件名称

                    string originalSavePath = string.Format("{0}/{1}", savefolder, originalNewfilename);
                    string bigSavePath = string.Format("{0}/{1}", savefolder, bigNewfilename);
                    string smallSavePath = string.Format("{0}/{1}", savefolder, smallNewfilename);


                    string path = System.Configuration.ConfigurationManager.AppSettings["custom:FileServerUploadPath"];

                    string serverOriginalSavePath = path + "/" + originalSavePath;
                    string serverBigSavePath = path + "/" + bigSavePath;
                    string serverSmallSavePath = path + "/" + smallSavePath;

                    fileName = fileName.ToLower().Replace("\\", "/");

                    ImageUpload s = new ImageUpload();
                    string domain = System.Configuration.ConfigurationManager.AppSettings["custom:FilesServerUrl"];




                    DirectoryInfo Drr = new DirectoryInfo(path + "/" + savefolder);
                    if (!Drr.Exists)
                    {
                        Drr.Create();
                    }

                    FileStream fs = new FileStream(serverOriginalSavePath, FileMode.Create, FileAccess.Write);
                    fs.Write(fileData, 0, fileData.Length);
                    fs.Flush();
                    fs.Close();

                    System.Drawing.Image originalImage = System.Drawing.Image.FromFile(serverOriginalSavePath);
                    image.OriginalPath = domain + originalSavePath;
                    image.OriginalWidth = originalImage.Width;
                    image.OriginalHeight = originalImage.Height;

                    if (GreateMiniImageModel(serverOriginalSavePath, serverBigSavePath, bigImgSize[0], bigImgSize[1]))
                    {
                        image.BigPath = domain + bigSavePath;
                        image.BigWidth = bigImgSize[0];
                        image.BigHeight = bigImgSize[1];
                    }
                    if (GreateMiniImageModel(serverOriginalSavePath, serverSmallSavePath, smallImgSize[0], smallImgSize[1]))
                    {
                        image.SmallPath = domain + smallSavePath;
                        image.SmallWidth = smallImgSize[0];
                        image.SmallHeight = smallImgSize[1];
                    }


                    originalImage.Dispose();
                    r.Data = new { name = fileName, url = image.OriginalPath };
                    r.Message = "上传成功";
                    r.Result = ResultType.Success;
                }
            }
            catch (Exception ex)
            {
                r.Result = ResultType.Exception;
                r.Message = "上传失败";
                LogUtil.Error("WebApi上传图片异常", ex);

            }

            string json = r.ToString();
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(json, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }
    }
}
