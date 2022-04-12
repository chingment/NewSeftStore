using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using LocalS.BLL;
using LocalS.BLL.Biz;
using log4net;
using SenvivSdk;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Test
{

    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", "LTAI4GHVbVRpJJ4h2kSAmVc6", "yipmZ8XZ0Bw4p2p2CvEZrirPre46b3");

    //        DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", "Dysmsapi", "dysmsapi.aliyuncs.com");

    //        DefaultAcsClient client = new DefaultAcsClient(profile);

    //        CommonRequest request = new CommonRequest("Dysmsapi", "2017-05-25", "SendSms");
    //        request.Method = MethodType.POST;
    //        request.RegionId = "cn-hangzhou";
    //        request.Add("PhoneNumbers", "15989287032"); //接收短信的手机号码
    //        request.Add("SignName", "贩聚社团"); //短信签名名称
    //        request.Add("TemplateCode", "SMS_88990017"); //短信模板ID
    //        request.Add("TemplateParam", "{\"code\":\"1111\"}"); //短信模板变量对应的实际值，JSON格式
    //        try
    //        {
    //            CommonResponse response = client.GetAcsResponse(request);
    //            Console.WriteLine(System.Text.Encoding.Default.GetString(response.HttpResponse.Content));
    //        }
    //        catch (ServerException e)
    //        {
    //            Console.WriteLine(e);
    //        }
    //        catch (ClientException e)
    //        {
    //            Console.WriteLine(e);
    //        }
    //    }
    //}


    class Program
    {
        public static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);



        static void Main(string[] args)
        {
            SvUtil.D46Long(1000 * 1m / 6);

            decimal cccc = SvUtil.D46Decimal("3533.56653");

            //var week = Lumos.CommonUtil.GetDiffWeekDay(DateTime.Parse(DateTime.Now.ToString("2021-09-24")), DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")));
            //var birthLastDays = Convert.ToInt32((DateTime.Parse(DateTime.Now.ToString("2022-07-01")) - DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"))).TotalDays);
            //var pregnancy = new { birthLastDays = birthLastDays, gesWeek = week.Week, gesDay = week.Day };

            //int ss = 80;
            //decimal c = ss;
            //var s = Lumos.CommonUtil.GetDiffWeekDay(DateTime.Parse("2022-02-03"), DateTime.Now);


            //var c3 = Lumos.CommonUtil.GetPregnancyTime(20, 6);
            //int days = System.Threading.Thread.CurrentThread.CurrentUICulture.Calendar.GetDaysInMonth(2022, 2);


            //int cdddd = SvUtil.D46Int(1842.33);

            //long a = 18112;
            //long b = 29401;
            //// decimal c = (decimal)a / b;
            //decimal c = Math.Round((decimal)a / b, 2);

            //decimal num1 = (decimal)1.0;
            //string strNum = num1.ToString("0.#####");//0.5

            //String product = "Dysmsapi";//短信API产品名称
            //String domain = "dysmsapi.aliyuncs.com";//短信API产品域名

            //IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", "LTAI4GHVbVRpJJ4h2kSAmVc6", "yipmZ8XZ0Bw4p2p2CvEZrirPre46b3");

            //DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);

            //IAcsClient acsClient = new DefaultAcsClient(profile);
            //SendSmsRequest request = new SendSmsRequest();


            //string validcode = "524643";
            //string phoneNumber = "15989287032";
            //string templateCode = "SMS_88990017";
            //string templateParam = "{\"code\":\"" + validcode + "\"}";
            //request.SignName = "贩聚社团";//"管理控制台中配置的短信签名（状态必须是验证通过）"
            //request.PhoneNumbers = phoneNumber;//"接收号码，多个号码可以逗号分隔"
            //request.TemplateCode = templateCode;//管理控制台中配置的审核通过的短信模板的模板CODE（状态必须是验证通过）"
            //request.TemplateParam = templateParam;//短信模板中的变量；数字需要转换为字符串；个人用户每个变量长度必须小于15个字符。"

            //SendSmsResponse sendSmsResponse = acsClient.GetAcsResponse(request);




            //  string aaaaaaa = "{\"answers\":{\"birthday\":\"1949-10-01\",\"chronicdisease\":[\"5\",\"6\"],\"fullName\":\"邱小文\",\"height\":\"165\",\"isPregnant\":\"\",\"medicalhis\":[\"3\",\"2\"],\"medicine\":[\"3\",\"2\"],\"perplexs\":[\"2\",\"5\",\"4\"],\"sex\":\"1\",\"subhealth\":[\"8\",\"4\"],\"weight\":\"65\"},\"deviceId\":\"1004E747A049\"}";

            //  var rop = aaaaaaa.ToJsonObject<RopQuestFill>();

            //  //   var acc = new string[] { "1", "3" };
            //  var ss = rop.Answers["chronicdisease"];

            //  string ccc = ss.ToJsonString();

            //  string[] bbb = ccc.ToJsonObject<List<string>>().ToArray();
            // string cccc= string.Join(",", bbb);
            ////  string css = string.Join(",", acc);

            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

            //  var post = new
            //  {
            //      userid = "461x847d01142F",
            //      code = "",
            //      mobile = "13800138000",
            //      wechatid = "",
            //      nick = "test",
            //      headimgurl = "test",
            //      sex = "1",
            //      birthday = DateTime.Now.ToUnifiedFormatDate(),
            //      height = 100,
            //      weight = 100,
            //      createtime = "2020-06-22T10:23:58.784Z", //创建时间
            //      updateTime = "2020-06-22T10:23:58.784Z", //最后一次更新时间
            //      SAS = "1",
            //      BreathingMachine = "1",
            //      Perplex = "1", //目前困扰 （查看字典表）
            //      OtherPerplex = "1", //目前困扰输入其它 ,
            //      Medicalhistory = "1", //既往史 （查看字典表）
            //      OtherFamilyhistory = "1", //既往史其它 ,
            //      Medicine = "1", //用药情况 （查看字典表）
            //      OtherMedicine = "1", //用药情况其它 ,
            //      deptid = "46"
            //  };

            //  //LoginRequest b = new LoginRequest("", new { Name = "全线通月子会所", Pwd = "qxt123456" });
            //  //var restb = api.DoPost(b);
            //"uSHRH8B+8DwNDkACgL/F+pqakM7xJ+AHP2/k/36d96/ttvzZg6QTc2WSahsp6GIXkgPD5w99Q8WjoB9KWzVaKhilmaMYx18U+VAHSmP/me4="
            string token = "\"uSHRH8B+8DwNDkACgL/F+pqakM7xJ+AHP2/k/36d96/ttvzZg6QTc2WSahsp6GIXGqaIyOVTP9lF/fBNVG5Xel2WvOa+tj9CUbqIFayhmvc=\"";

            ReportParDetailRequest c1 = new ReportParDetailRequest(token, new { deptid = "46", sn = "1004E747A205", size = 1, page = 1 });
            var restb = api.DoPost(c1);

            //var config_Senviv = BizFactory.Senviv.GetConfig("46");

            //var i_SenvivUsers = SdkFactory.Senviv.GetUserList(config_Senviv);

            //  string sn = "1004E747A049";
            //  //"1"  //461x847d0113A4
            //  string deptid = "46";
            //  UserCreateRequest a = new UserCreateRequest(token, post);
            //  var rest = api.DoPost(a);
            //  //if (rest.Data != null)
            //  //{
            //  //    if (rest.Data.Data != null)
            //  //    {
            //  //        if (!string.IsNullOrEmpty(rest.Data.Data.userid))
            //  //        {
            //  //            string userid = rest.Data.Data.userid;
            //  //            BoxBindRequest b = new BoxBindRequest(token, new { sn = sn, userid = userid, deptid = deptid });
            //  //            var restb = api.DoPost(b);

            //  //            BoxUnBindRequest c = new BoxUnBindRequest(token, new { sn = sn, userid = userid, deptid = deptid });
            //  //            var restc = api.DoPost(c);
            //  //        }

            //  //    }
            //  //}

            //  //BoxBindRequest b = new BoxBindRequest(token, new { sn = sn, userid = "461x847d0113A4", deptid = deptid });
            //  //var restb = api.DoPost(b);

            //  BoxUnBindRequest c = new BoxUnBindRequest(token, new { sn = sn, userid = "461x847d0113A4", deptid = deptid });
            //  var restc = api.DoPost(c);

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
