using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("SenvivHealthDay")]
    public class SenvivHealthDay
    {
        public string Id { get; set; }
        public string SvUserId { get; set; }
        public DateTime HealthDate { get; set; }
        public decimal TotalScore { get; set; }
        //实际睡眠时长
        public decimal SmXjsmsc { get; set; }
        //浅度睡眠
        public decimal SmQdmc { get; set; }
        //深度睡眠
        public decimal SmSdmc { get; set; }
        //REM睡眠
        public decimal SmSemmc { get; set; }
        //睡眠周期
        public decimal SmSmzq { get; set; }
        //体动次数
        public decimal SmTdcs { get; set; }
        //当次基准心率
        public decimal XlDcjzxl { get; set; }
        //长期基准心率
        public decimal XlCqjzxl { get; set; }
        //当次平均心率
        public decimal XlDcpjxl { get; set; }
        //当次基准呼吸
        public decimal HxDcjzhx { get; set; }
        //长期基准呼吸
        public decimal HxCqjzhx { get; set; }
        //平均呼吸
        public decimal HxDcPj { get; set; }
        //呼吸暂停次数
        public decimal HxZtcs { get; set; }
        //AHI指数
        public decimal HxAhizs { get; set; }
        //心律失常风险
        public decimal HrvXlscfx { get; set; }
        //心率减速力
        public decimal HrvXljsl { get; set; }
        //心脏总能量
        public decimal HrvXzznl { get; set; }
        //交感神经张力指数
        public decimal HrvJgsjzlzs { get; set; }
        //迷走神经张力指数
        public decimal HrvMzsjzlzs { get; set; }
        //自主神经平衡指数
        public decimal HrvZzsjzlzs { get; set; }
        //荷尔蒙指数
        public decimal HrvHermzs { get; set; }
        //体温及血管舒缩指数
        public decimal HrvTwjxgsszh { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
