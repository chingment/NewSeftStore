using EasemobSdk;
using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.BLL.Mq.MqByRedis;
using LocalS.BLL.Push;
using LocalS.BLL.Task;
using LocalS.Entity;
using LocalS.Service;
using LocalS.Service.Api.HealthApp;
using LocalS.Service.Api.Merch;
using LocalS.Service.Api.StoreApp;
using LocalS.Service.Api.StoreTerm;
using log4net;
using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using MyWeiXinSdk;
using Newtonsoft.Json.Linq;
using NPinyin;
using SenvivSdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TgPaySdk;
using XrtPaySdk;

namespace Test
{

    class Program
    {
        public static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {

            string aaaaaaa = "{\"answers\":{\"birthday\":\"1949-10-01\",\"chronicdisease\":[\"5\",\"6\"],\"fullName\":\"邱小文\",\"height\":\"165\",\"isPregnant\":\"\",\"medicalhis\":[\"3\",\"2\"],\"medicine\":[\"3\",\"2\"],\"perplexs\":[\"2\",\"5\",\"4\"],\"sex\":\"1\",\"subhealth\":[\"8\",\"4\"],\"weight\":\"65\"},\"deviceId\":\"1004E747A049\"}";

            var rop = aaaaaaa.ToJsonObject<RopQuestFill>();

            //   var acc = new string[] { "1", "3" };
            var ss = rop.Answers["chronicdisease"];

            string ccc = ss.ToJsonString();

            string[] bbb = ccc.ToJsonObject<List<string>>().ToArray();
           string cccc= string.Join(",", bbb);
          //  string css = string.Join(",", acc);

            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

            var post = new
            {
                userid = "461x847d01142F",
                code = "",
                mobile = "13800138000",
                wechatid = "",
                nick = "test",
                headimgurl = "test",
                sex = "1",
                birthday = DateTime.Now.ToUnifiedFormatDate(),
                height = 100,
                weight = 100,
                createtime = "2020-06-22T10:23:58.784Z", //创建时间
                updateTime = "2020-06-22T10:23:58.784Z", //最后一次更新时间
                SAS = "1",
                BreathingMachine = "1",
                Perplex = "1", //目前困扰 （查看字典表）
                OtherPerplex = "1", //目前困扰输入其它 ,
                Medicalhistory = "1", //既往史 （查看字典表）
                OtherFamilyhistory = "1", //既往史其它 ,
                Medicine = "1", //用药情况 （查看字典表）
                OtherMedicine = "1", //用药情况其它 ,
                deptid = "46"
            };

            //LoginRequest b = new LoginRequest("", new { Name = "全线通月子会所", Pwd = "qxt123456" });
            //var restb = api.DoPost(b);

            string token = "\"uSHRH8B+8DwNDkACgL/F+pqakM7xJ+AHP2/k/36d96/ttvzZg6QTc2WSahsp6GIXzK4ogIzu99ch2EaMLa6EKIKVXzvn6vi4wRi1No7AhC0=\"";
            string sn = "1004E747A049";
            //"1"  //461x847d0113A4
            string deptid = "46";
            UserCreateRequest a = new UserCreateRequest(token, post);
            var rest = api.DoPost(a);
            //if (rest.Data != null)
            //{
            //    if (rest.Data.Data != null)
            //    {
            //        if (!string.IsNullOrEmpty(rest.Data.Data.userid))
            //        {
            //            string userid = rest.Data.Data.userid;
            //            BoxBindRequest b = new BoxBindRequest(token, new { sn = sn, userid = userid, deptid = deptid });
            //            var restb = api.DoPost(b);

            //            BoxUnBindRequest c = new BoxUnBindRequest(token, new { sn = sn, userid = userid, deptid = deptid });
            //            var restc = api.DoPost(c);
            //        }

            //    }
            //}

            //BoxBindRequest b = new BoxBindRequest(token, new { sn = sn, userid = "461x847d0113A4", deptid = deptid });
            //var restb = api.DoPost(b);

            BoxUnBindRequest c = new BoxUnBindRequest(token, new { sn = sn, userid = "461x847d0113A4", deptid = deptid });
            var restc = api.DoPost(c);

            Console.ReadLine();
        }



        private static sbyte[] SendCmd(sbyte[] frameCmd, sbyte[] frameCmdParms)
        {
            sbyte[] frameHead = new sbyte[] { 0x24 };
            sbyte[] frameEnd = new sbyte[] { 0x0D, 0x0A };
            if (frameCmd == null)
            {
                frameCmd = new sbyte[] { };
            }

            if (frameCmdParms == null)
            {
                frameCmdParms = new sbyte[] { };
            }

            //长度码=命令码字节数+命令参数字节数+校验码字节数
            sbyte framLengthCode = Convert.ToSByte(frameCmd.Length + frameCmdParms.Length + 1);

            //int xorAndLength = 1 + i_framLenth;

            sbyte[] xorAnd = new sbyte[1 + frameCmd.Length + frameCmdParms.Length];

            xorAnd[0] = framLengthCode;

            for (int i = 0; i < frameCmd.Length; i++)
            {
                xorAnd[i + 1] = frameCmd[i];
            }

            for (int i = 0; i < frameCmdParms.Length; i++)
            {
                xorAnd[frameCmd.Length + 1] = frameCmdParms[i];
            }


            sbyte framXorCode = 0;
            for (int i = 0; i < xorAnd.Length; i++)
            {
                framXorCode ^= xorAnd[i];
            }

            List<sbyte> frame = new List<sbyte>();

            frame.AddRange(frameHead);
            frame.Add(framLengthCode);
            frame.AddRange(frameCmd);
            frame.AddRange(frameCmdParms);
            frame.Add(framXorCode);
            frame.AddRange(frameEnd);

            return frame.ToArray();
        }


    }
}
