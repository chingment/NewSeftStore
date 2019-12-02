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
        public static string GetMain(string jsonStr)
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

            return imgUrl;
        }

        public static string Convert_Main_S(string imgUrl)
        {
            return imgUrl;
        }
    }
}
