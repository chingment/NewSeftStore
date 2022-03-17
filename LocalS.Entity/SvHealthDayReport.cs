using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("SvHealthDayReport")]
    public class SvHealthDayReport
    {
        public string Id { get; set; }
        public string SvUserId { get; set; }
        public DateTime HealthDate { get; set; }
        public decimal HealthScore { get; set; }
        public decimal SmScore { get; set; }
        public string SmTags { get; set; }
        public DateTime SmScsj { get; set; }
        public DateTime SmLcsj { get; set; }
        //在床时长
        public long SmZcsc { get; set; }
        //睡眠时长
        public long SmSmsc { get; set; }
        //入睡需时
        public long SmRsxs { get; set; }
        //入睡时刻
        public DateTime SmRssj { get; set; }
        //清醒时刻
        public DateTime SmQxsj { get; set; }
        //深度睡眠时长
        public long SmSdsmsc { get; set; }
        //深度睡眠比例
        public decimal SmSdsmbl { get; set; }
        //浅度睡眠时长
        public long SmQdsmsc { get; set; }
        //浅度睡眠比例
        public decimal SmQdsmbl { get; set; }
        //REM睡眠时长
        public long SmRemsmsc { get; set; }
        //REM睡眠比例
        public decimal SmRemsmbl { get; set; }
        //清醒时刻时长
        public long SmQxsksc { get; set; }
        //清醒时刻比例
        public decimal SmQxskbl { get; set; }
        //离真次数
        public int SmLzcs { get; set; }
        //离真时长
        public long SmLzsc { get; set; }

        public decimal SmLzscbl { get; set; }

        //睡眠周期
        public int SmSmzq { get; set; }
        //体动次数
        public int SmTdcs { get; set; }
        //体动次数结构体
        public string SmTdcsPoint { get; set; }

        //平均体动时长
        public int SmPjtdsc { get; set; }

        //睡眠效率
        public decimal SmSmxl { get; set; }

        //睡眠连续性
        public decimal SmSmlxx { get; set; }

        //孕气指数
        public decimal ZsGmYq { get; set; }
        //易胖指数
        public decimal ZsGmYp { get; set; }
        //水润指数
        public decimal ZsGmSr { get; set; }
        public decimal ZsGmMl { get; set; }
        //当次基准心率
        public int XlDcjzxl { get; set; }
        //长期基准心率
        public int XlCqjzxl { get; set; }
        //当次平均心率
        public int XlDcpjxl { get; set; }
        //最高心率
        public int XlZg { get; set; }
        //最低心率
        public int XlZd { get; set; }
        //心动过快时长
        public long XlXdgksc { get; set; }
        //心动过慢时长
        public long XlXdgmsc { get; set; }
        //心率超过1.25时长
        public long Xlcg125 { get; set; }
        //心率超过1.15时长
        public long Xlcg115 { get; set; }
        //心率超过0.85时长
        public long Xlcg085 { get; set; }
        //心率超过075时长
        public long Xlcg075 { get; set; }
        //呼吸当次基准呼吸
        public int HxDcjzhx { get; set; }
        //呼吸长期基准呼吸
        public int HxCqjzhx { get; set; }
        //呼吸平均呼吸
        public int HxDcpjhx { get; set; }
        //呼吸最高呼吸
        public int HxZghx { get; set; }
        //呼吸最低呼吸
        public int HxZdhx { get; set; }
        //呼吸过快时长
        public long HxGksc { get; set; }
        //呼吸过慢时长
        public long HxGmsc { get; set; }
        //呼吸暂停次数
        public int HxZtcs { get; set; }
        //呼吸暂停次数结构体
        public string HxZtcsPoint { get; set; }

        //呼吸暂停AHI指数
        public decimal HxZtahizs { get; set; }
        //呼吸暂停平均时长
        public long HxZtpjsc { get; set; }
        //心律失常风险
        public int JbfxXlscfx { get; set; }
        //心率减速力
        public decimal JbfxXljsl { get; set; }
        //心脏总能量
        public int HrvXzznl { get; set; }
        //心脏总能量基准值
        public int HrvXzznljzz { get; set; }
        //交感神经张力指数
        public int HrvJgsjzlzs { get; set; }
        //交感神经张力指数基准值
        public int HrvJgsjzlzsjzz { get; set; }
        //迷走神经张力指数
        public int HrvMzsjzlzs { get; set; }
        //迷走神经张力指数基准值
        public int HrvMzsjzlzsjzz { get; set; }
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
        //情绪心理
        public decimal QxxlQxxl { get; set; }

        public string XlPoint { get; set; }
        public string HxPoint { get; set; }
        public string SmPoint { get; set; }
        public int VisitCount { get; set; }
        public bool IsValid { get; set; }
        public bool IsSend { get; set; }
        public string RptSummary { get; set; }
        public string RptSuggest { get; set; }
        public E_SvHealthReportStatus Status { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public decimal SmScoreRatio { get; set; }
        public decimal HealthScoreRatio { get; set; }
    }
}
