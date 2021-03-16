﻿using System;
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

        //在床时长
        public decimal SmZcsc { get; set; }
        //睡眠时长
        public decimal SmSmsc { get; set; }
        //上床时刻
        public decimal SmScsk{ get; set; }
        //入睡需时
        public decimal SmRsxs { get; set; }
        //入睡时刻
        public decimal SmRssk { get; set; }
        //清醒时刻
        public decimal SmQxsk { get; set; }
        //起床时刻
        public decimal SmQcsk { get; set; }
        //深度睡眠时长
        public decimal SmSdsmsc { get; set; }
        //深度睡眠比例
        public decimal SmSdsmbl { get; set; }
        //浅度睡眠时长
        public decimal SmQdsmsc { get; set; }
        //浅度睡眠比例
        public decimal SmQdsmbl { get; set; }
        //REM睡眠时长
        public decimal SmSemqsc { get; set; }
        //REM睡眠比例
        public decimal SmSemqbl { get; set; }
        //清醒时刻时长
        public decimal SmQxsksc { get; set; }
        //清醒时刻比例
        public decimal SmQxskbl{ get; set; }
        //离真次数
        public decimal SmLzcs{ get; set; }
        //离真时长
        public decimal SmLzsc { get; set; }
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
        //呼吸当次基准呼吸
        public decimal HxDcjzhx { get; set; }
        //呼吸长期基准呼吸
        public decimal HxCqjzhx { get; set; }
        //呼吸平均呼吸
        public decimal HxDcPj { get; set; }
        //呼吸最高呼吸
        public decimal HxZgHx { get; set; }
        //呼吸最低呼吸
        public decimal HxZdHx { get; set; }
        //呼吸过快时长
        public decimal HxGksc{ get; set; }
        //呼吸过慢时长
        public decimal HxGmsc { get; set; }
        //呼吸暂停次数
        public decimal HxZtHxztcs { get; set; }
        //呼吸暂停AHI指数
        public decimal HxZtAhizs { get; set; }
        //呼吸暂停平均时长
        public decimal HxZtPjsc { get; set; }
        //心律失常风险
        public decimal JbfxXlscfx { get; set; }
        //心率减速力
        public decimal JbfxXljsl { get; set; }
        //心脏总能量
        public decimal HrvXzznl { get; set; }
        //心脏总能量基准值
        public decimal HrvXzznlJzz { get; set; }
        //交感神经张力指数
        public decimal HrvJgsjzlzs { get; set; }
        //交感神经张力指数基准值
        public decimal HrvJgsjzlzsJzz { get; set; }
        //迷走神经张力指数
        public decimal HrvMzsjzlzs { get; set; }
        //迷走神经张力指数基准值
        public decimal HrvMzsjzlzsJzz { get; set; }
        //自主神经平衡指数
        public decimal HrvZzsjzlzs { get; set; }
        //自主神经平衡指数基准值
        public decimal HrvZzsjzlzsJzz { get; set; }
        //荷尔蒙指数
        public decimal HrvHermzs { get; set; }
        //荷尔蒙指数基准值
        public decimal HrvHermzsJzz { get; set; }
        //体温及血管舒缩指数
        public decimal HrvTwjxgsszh { get; set; }
        //体温及血管舒缩指数基准值
        public decimal HrvTwjxgsszhJzz { get; set; }

        //免疫力指数
        public decimal MylMylZs { get; set; }
        //感染风险
        public decimal MylGrfx { get; set; }

        //高血压管控
        public decimal MbGxygk{ get; set; }
        //冠心病管控
        public decimal MbGxbgk { get; set; }
        //糖尿病管控
        public decimal MbTlbgk { get; set; }
        //焦虑情绪
        public decimal QxxlJlqx { get; set; }
        //抗压能力
        public decimal QxxlKynl { get; set; }
        //情绪应激
        public decimal QxxlQxyj { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
