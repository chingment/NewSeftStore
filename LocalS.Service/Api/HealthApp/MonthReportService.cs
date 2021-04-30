using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.HealthApp
{
    public class MonthReportService : BaseService
    {
        public CustomJsonResult GetMonitor(string operater, string rptId)
        {

            var result = new CustomJsonResult();

            var rpt = (from u in CurrentDb.SenvivHealthMonthReport

                       join s in CurrentDb.SenvivUser on u.SvUserId equals s.Id into temp
                       from tt in temp.DefaultIfEmpty()
                       where u.Id == rptId
                       select new
                       {
                           u.Id,
                           tt.Nick,
                           tt.Sex,
                           tt.Account,
                           tt.Birthday,
                           tt.HeadImgurl,
                           u.SmTags,
                           u.TotalScore,
                           u.HealthDate,
                           u.MylGrfx,
                           u.MylMylzs,
                           u.MbGxbgk,
                           u.MbGxygk,
                           u.MbTlbgk,
                           u.QxxlJlqx,
                           u.QxxlKynl,
                           u.QxxlQxyj,
                           u.JbfxXljsl,
                           u.JbfxXlscfx,
                           u.HrvXzznl,
                           u.HrvJgsjzlzs,
                           u.HrvMzsjzlzs,
                           u.HrvZzsjzlzs,
                           u.HrvHermzs,
                           u.HrvTwjxgsszs,
                           u.XlDcjzxl,
                           u.XlCqjzxl,
                           u.XlDcpjxl,
                           u.HxDcjzhx,
                           u.HxCqjzhx,
                           u.HxDcpjhx,
                           u.HxZtcs,
                           u.HxZtahizs,
                           u.HxZtpjsc,
                           u.SmSmsc,
                           u.SmSdsmsc,
                           u.SmQdsmsc,
                           u.SmRemsmsc,
                           u.SmTdcs,
                           u.SmSmzq,
                           u.IsBuild,
                           u.IsSend,
                           u.VisitCount,
                           u.Status,
                           u.SvUserId,
                           u.CreateTime
                       }).FirstOrDefault();


            var sum_TotalScores = CurrentDb.SenvivHealthMonthReport.Where(m => m.HealthDate == rpt.HealthDate).Select(m => m.TotalScore).ToList();
            int scoreRatio = 80;
            if (sum_TotalScores.Count > 0)
            {
                int a = sum_TotalScores.Where(m => m < rpt.TotalScore).Count();
                int b = sum_TotalScores.Count();
                double r = Math.Round((Convert.ToDouble(a) / Convert.ToDouble(b)), 2) * 100;
                scoreRatio = Convert.ToInt32(r);
            }

            var ret = new
            {
                Id = rpt.Id,
                UserInfo = new
                {
                    SignName = SvUtil.GetSignName(rpt.Nick, rpt.Account),
                    HeadImgurl = rpt.HeadImgurl,
                    Sex = SvUtil.GetSexName(rpt.Sex),
                    Age = SvUtil.GetAge(rpt.Birthday)
                },
                ReportData = new
                {
                    scoreRatio,
                    rpt.TotalScore,
                    rpt.HealthDate,
                    SmTags = rpt.SmTags.ToJsonObject<List<object>>(),
                    MylGrfx = SvDataJdUtil.GetMylGrfx(rpt.MylGrfx),
                    MylMylzs = SvDataJdUtil.GetMylzs(rpt.MylMylzs),
                    MbGxbgk = SvDataJdUtil.GetMbGxbgk(rpt.MbGxbgk),
                    MbGxygk = SvDataJdUtil.GetMbGxygk(rpt.MbGxygk),
                    MbTlbgk = SvDataJdUtil.GetMbTlbgk(rpt.MbTlbgk),
                    rpt.QxxlJlqx,
                    QxxlKynl = SvDataJdUtil.GetQxxlKynl(rpt.QxxlKynl),
                    rpt.QxxlQxyj,
                    JbfxXlscfx = SvDataJdUtil.GetJbfxXlscfx(rpt.JbfxXlscfx),
                    JbfxXljsl = SvDataJdUtil.GetJbfxXljsl(rpt.JbfxXljsl),
                    //心脏总能量
                    HrvXzznl = SvDataJdUtil.GetHrvXzznl(rpt.HrvXzznl),
                    //交感神经张力指数
                    HrvJgsjzlzs = SvDataJdUtil.GetHrvJgsjzlzs(rpt.HrvJgsjzlzs),
                    //迷走神经张力指数
                    HrvMzsjzlzs = SvDataJdUtil.GetHrvMzsjzlzs(rpt.HrvMzsjzlzs),
                    //自主神经平衡指数
                    HrvZzsjzlzs = SvDataJdUtil.GetHrvZzsjzlzs(rpt.HrvZzsjzlzs),
                    //荷尔蒙指数
                    HrvHermzs = SvDataJdUtil.GetHrvHermzs(rpt.HrvHermzs),
                    //体温及血管舒缩指数
                    HrvTwjxgsszs = SvDataJdUtil.GetHrvTwjxgsszh(rpt.HrvTwjxgsszs),
                    //当次基准心率
                    XlDcjzxl = SvDataJdUtil.GetXlDcjzxl(rpt.XlDcjzxl),
                    //长期基准心率
                    XlCqjzxl = SvDataJdUtil.GetXlCqjzxl(rpt.XlCqjzxl),
                    //当次平均心率
                    XlDcpjxl = SvDataJdUtil.GetXlDcpjxl(rpt.XlDcpjxl),
                    //呼吸当次基准呼吸
                    HxDcjzhx = SvDataJdUtil.GetHxDcjzhx(rpt.HxDcjzhx),
                    //呼吸长期基准呼吸
                    HxCqjzhx = SvDataJdUtil.GetHxCqjzhx(rpt.HxCqjzhx),
                    //呼吸平均呼吸
                    HxDcpjhx = SvDataJdUtil.GetHxDcpjhx(rpt.HxDcpjhx),
                    //呼吸暂停次数
                    HxZtcs = SvDataJdUtil.GetHxZtcs(rpt.HxZtcs),
                    //呼吸暂停AHI指数
                    HxZtahizs = SvDataJdUtil.GetHxZtahizs(rpt.HxZtahizs),
                    //睡眠时长
                    SmSmsc = SvDataJdUtil.GetSmSmsc(rpt.SmSmsc),
                    //深度睡眠时长
                    SmSdsmsc = SvDataJdUtil.GetSmSdsmsc(rpt.SmSdsmsc),
                    //浅度睡眠时长
                    SmQdsmsc = SvDataJdUtil.GetSmQdsmsc(rpt.SmQdsmsc),
                    //REM睡眠时长
                    SmRemsmsc = SvDataJdUtil.GetSmRemsmsc(rpt.SmRemsmsc),
                    //睡眠周期=
                    SmSmzq = SvDataJdUtil.GetSmSmzq(rpt.SmSmzq),
                    //体动次数
                    SmTdcs = SvDataJdUtil.GetSmTdcs(rpt.SmTdcs)
                }
            };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult GetEnergy(string operater, string rptId)
        {
            var result = new CustomJsonResult();

            var rpt = (from u in CurrentDb.SenvivHealthMonthReport
                       where u.Id == rptId
                       select new
                       {
                           u.Id,
                           u.SmSmsc,
                           u.HrvXzznl,
                           u.HxZtcs,
                           u.HxZtahizs,
                           u.DatePt,
                           u.SmSmscPt,
                           u.HrvXzznlPt,
                           u.HxZtcsPt,
                           u.HxZtahizsPt,
                           u.CreateTime
                       }).FirstOrDefault();



            var ret = new
            {
                HrvXzznl = SvDataJdUtil.GetHrvXzznl(rpt.HrvXzznl),
                HxZtcs = SvDataJdUtil.GetHxZtcs(rpt.HxZtcs),
                HxZtahizs = SvDataJdUtil.GetHxZtahizs(rpt.HxZtahizs),
                SmSmsc = SvDataJdUtil.GetSmSmsc(rpt.SmSmsc),
                DatePt = rpt.DatePt.ToJsonObject<List<object>>(),
                SmSmscPt = rpt.SmSmscPt.ToJsonObject<List<object>>(),
                HrvXzznlPt = rpt.HrvXzznlPt.ToJsonObject<List<object>>(),
                HxZtcsPt = rpt.HxZtcsPt.ToJsonObject<List<object>>(),
                HxZtahizsPt = rpt.HxZtahizsPt.ToJsonObject<List<object>>(),
            };


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

        public CustomJsonResult GetAdvise(string operater, string rptId)
        {

            var result = new CustomJsonResult();

            var rpt = (from u in CurrentDb.SenvivHealthMonthReport
                       where u.Id == rptId
                       select new
                       {
                           u.Id,
                           u.SugByYy,
                           u.SugByYd,
                           u.SugBySm,
                           u.SugByQxyl,
                           u.IsSend,
                           u.Status,
                           u.CreateTime
                       }).FirstOrDefault();

            var ret = new
            {
                SugByYy = rpt.SugByYy.NullToEmpty(),
                SugByYd = rpt.SugByYd.NullToEmpty(),
                SugBySm = rpt.SugBySm.NullToEmpty(),
                SugByQxyl = rpt.SugByQxyl.NullToEmpty(),
                rpt.IsSend
            };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }

    }
}
