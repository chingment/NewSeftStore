using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LocalS.Entity
{

    public enum E_SenvivHealthMonthStatus
    {
        Unknow = 0,
        WaitBuild = 1,
        Building = 2,
        BuildSuccess = 3,
        BuildFailure = 4,
        WaitSend = 5,
        Sending = 6,
        SendSuccess = 7,
        SendFailure = 8
    }

    [Table("SenvivHealthMonthReport")]
    public class SenvivHealthMonthReport
    {
        public string Id { get; set; }
        public string SvUserId { get; set; }
        public string HealthDate { get; set; }
        public int DayCount { get; set; }
        public decimal TotalScore { get; set; }

        public string SmTags { get; set; }
        //在床时长
        public decimal SmZcsc { get; set; }
        //睡眠时长
        public decimal SmSmsc { get; set; }
        //入睡需时
        public decimal SmRsxs { get; set; }
        public decimal SmSdsmsc { get; set; }
        //深度睡眠比例
        public decimal SmSdsmbl { get; set; }
        //浅度睡眠时长
        public decimal SmQdsmsc { get; set; }
        //浅度睡眠比例
        public decimal SmQdsmbl { get; set; }
        //REM睡眠时长
        public decimal SmRemsmsc { get; set; }
        //REM睡眠比例
        public decimal SmRemsmbl { get; set; }
        //清醒时刻时长
        public decimal SmQxsksc { get; set; }
        //清醒时刻比例
        public decimal SmQxskbl { get; set; }
        //离真次数
        public decimal SmLzcs { get; set; }
        //离真时长
        public decimal SmLzsc { get; set; }

        public decimal SmLzscbl { get; set; }

        //睡眠周期
        public decimal SmSmzq { get; set; }
        //体动次数
        public decimal SmTdcs { get; set; }
        //平均体动时长
        public decimal SmPjtdsc { get; set; }

        //当次基准心率
        public decimal XlDcjzxl { get; set; }
        //长期基准心率
        public decimal XlCqjzxl { get; set; }
        //当次平均心率
        public decimal XlDcpjxl { get; set; }
        //最高心率
        public decimal XlZg { get; set; }
        //最低心率
        public decimal XlZd { get; set; }
        //心动过快时长
        public decimal XlXdgksc { get; set; }
        //心动过慢时长
        public decimal XlXdgmsc { get; set; }
        //心率超过1.25时长
        public decimal Xlcg125 { get; set; }
        //心率超过1.15时长
        public decimal Xlcg115 { get; set; }
        //心率超过0.85时长
        public decimal Xlcg085 { get; set; }
        //心率超过075时长
        public decimal Xlcg075 { get; set; }
        //呼吸当次基准呼吸
        public decimal HxDcjzhx { get; set; }
        //呼吸长期基准呼吸
        public decimal HxCqjzhx { get; set; }
        //呼吸平均呼吸
        public decimal HxDcpjhx { get; set; }
        //呼吸最高呼吸
        public decimal HxZghx { get; set; }
        //呼吸最低呼吸
        public decimal HxZdhx { get; set; }
        //呼吸过快时长
        public decimal HxGksc { get; set; }
        //呼吸过慢时长
        public decimal HxGmsc { get; set; }
        //呼吸暂停次数
        public decimal HxZtcs { get; set; }
        //呼吸暂停AHI指数
        public decimal HxZtahizs { get; set; }
        //呼吸暂停平均时长
        public decimal HxZtpjsc { get; set; }
        //心律失常风险
        public decimal JbfxXlscfx { get; set; }
        //心率减速力
        public decimal JbfxXljsl { get; set; }
        //心脏总能量
        public decimal HrvXzznl { get; set; }
        //心脏总能量基准值
        public decimal HrvXzznljzz { get; set; }
        //交感神经张力指数
        public decimal HrvJgsjzlzs { get; set; }
        //交感神经张力指数基准值
        public decimal HrvJgsjzlzsjzz { get; set; }
        //迷走神经张力指数
        public decimal HrvMzsjzlzs { get; set; }
        //迷走神经张力指数基准值
        public decimal HrvMzsjzlzsjzz { get; set; }
        //自主神经平衡指数
        public decimal HrvZzsjzlzs { get; set; }
        //自主神经平衡指数基准值
        public decimal HrvZzsjzlzsjzz { get; set; }
        //荷尔蒙指数
        public decimal HrvHermzs { get; set; }
        //荷尔蒙指数基准值
        public decimal HrvHermzsjzz { get; set; }
        //体温及血管舒缩指数
        public decimal HrvTwjxgsszs { get; set; }
        //体温及血管舒缩指数基准值
        public decimal HrvTwjxgsszhjzz { get; set; }
        //免疫力指数
        public decimal MylMylzs { get; set; }
        //感染风险
        public decimal MylGrfx { get; set; }
        //高血压管控
        public decimal MbGxygk { get; set; }
        //冠心病管控
        public decimal MbGxbgk { get; set; }
        //糖尿病管控
        public decimal MbTlbgk { get; set; }
        //焦虑情绪
        public string QxxlJlqx { get; set; }
        //抗压能力
        public decimal QxxlKynl { get; set; }
        //情绪应激
        public decimal QxxlQxyj { get; set; }

        public string DatePt { get; set; }
        public string SmSmscPt { get; set; }
        public string SmDtqcsPt { get; set; }
        public string XlDcjzxlPt { get; set; }
        public string XlCqjzxlPt { get; set; }
        public string HrvXzznlPt { get; set; }
        public string HxZtcsPt { get; set; }
        public string HxDcjzhxPt { get; set; }
        public string HxCqjzhxPt { get; set; }
        public string HxZtahizsPt { get; set; }
        public string HrvJgsjzlzsPt { get; set; }
        public string HrvMzsjzlzsPt { get; set; }
        public string HrvZzsjzlzsPt { get; set; }
        public string HrvHermzsPt { get; set; }
        public string HrvTwjxgsszsPt { get; set; }
        public string JbfxXlscfxPt { get; set; }
        public string JbfxXljslPt { get; set; }
        public bool IsBuild { get; set; }
        public bool IsSend { get; set; }
        public int VisitCount { get; set; }
        public E_SenvivHealthMonthStatus Status { get; set; }
        public string SugByYd { get; set; }
        public string SugByYy { get; set; }
        public string SugBySm { get; set; }
        public string SugByQxyl { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
