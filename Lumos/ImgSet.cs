using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace Lumos
{
    public class ImgSet
    {
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public string Name { get; set; }
        public int Priority { set; get; }

        public static string GetMain_S(string jsonStr)
        {
            return GetMain(jsonStr, "_S");
        }

        public static string Convert_S(string imgUrl)
        {
            if (imgUrl.IndexOf("_O") > -1)
            {
                imgUrl = imgUrl.Replace("_O", "_S");
            }
            else if (imgUrl.IndexOf("_B") > -1)
            {
                imgUrl = imgUrl.Replace("_B", "_S");
            }

            return imgUrl;
        }


        public static string Convert_B(string imgUrl)
        {
            if (imgUrl.IndexOf("_O") > -1)
            {
                imgUrl = imgUrl.Replace("_O", "_B");
            }
            else if (imgUrl.IndexOf("_S") > -1)
            {
                imgUrl = imgUrl.Replace("_S", "_B");
            }

            return imgUrl;
        }
        public static string GetMain_B(string jsonStr)
        {
            return GetMain(jsonStr, "_B");
        }

        public static string GetMain_O(string jsonStr)
        {
            return GetMain(jsonStr, null);
        }

        private static string GetMain(string jsonStr, string format = null)
        {
            string imgUrl = "";
            try
            {
                List<ImgSet> d = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ImgSet>>(jsonStr);

                if (d != null)
                {
                    if (d.Count > 0)
                    {
                        var d1 = d.Where(m => m.IsMain == true).FirstOrDefault();
                        if (d1 == null)
                        {
                            imgUrl = d[0].Url;
                        }
                        else
                        {
                            imgUrl = d1.Url;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogUtil.Error("解释ImgSet Json 错误", ex);
            }

            if (format != null)
            {
                if (imgUrl != null)
                {
                    if (imgUrl.IndexOf("_O") > -1)
                    {
                        imgUrl = imgUrl.Replace("_O", format);
                    }
                }
            }

            return imgUrl;
        }
    }
}
