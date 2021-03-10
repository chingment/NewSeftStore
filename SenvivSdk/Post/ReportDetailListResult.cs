using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenvivSdk
{
    public class ReportDetailListResult
    {
        public int count { get; set; }

        public List<DataModel> data { get; set; }

        public class DataModel
        {
            public string reportId { get; set; }
            public string sn { get; set; }
            public string userid { get; set; }
            public int read { get; set; }
            public string is_catch { get; set; }
            public string FinishTime { get; set; }
            public string createtime { get; set; }
            public D_Report Report { get; set; }
            public D_ReportOfBreath ReportOfBreath { get; set; }
            public D_ReportOfHeartBeat ReportOfHeartBeat { get; set; }
            public D_ReportOfSleep ReportOfSleep { get; set; }
            public D_ReportOfHRV ReportOfHRV { get; set; }
            public D_ReportOfOffbed ReportOfOffbed { get; set; }
            public D_UserBaseInfo UserBaseInfo { get; set; }
            public D_ReportChart ReportCharts { get; set; }
        }


        public class D_Report
        {
            public string ReportId { get; set; }
            public string BoxMac { get; set; }
            public long StartTime { get; set; }
            public long FinishTime { get; set; }
            public long CreateTime { get; set; }
            public long OffbedTime { get; set; }
            public long OnbedTime { get; set; }
            public string PenaltyInfo { get; set; }
            public decimal TotalScore { get; set; }
            public List<D_ReportIndex> indexs { get; set; }
            public List<D_ReportLabel> labels { get; set; }
            public List<D_ReportAdvice> advices { get; set; }
            //public List<D_ReportTodayprediction> todaypredictions { get; set; }
            public int hrvreport { get; set; }
            public string userId { get; set; }
            public List<D_ReportTarget> target { get; set; }
            public string boxSn { get; set; }
        }

        public class D_Data
        {
            public string MY { get; set; }
            public string GR { get; set; }
            public string GxueY { get; set; }
            public string Gxing { get; set; }
            public string TanNiao { get; set; }
            public string QingXu { get; set; }
            public string KanYa { get; set; }
        }

        public class D_ReportIndex
        {
            public decimal score { get; set; }
            public string type { get; set; }
            public string explain { get; set; }
            public List<string> suggest { get; set; }
        }

        public class D_ReportLabel
        {
            public string Explain { get; set; }
            public string TagName { get; set; }
            public int level { get; set; }
            public string color { get; set; }
            public List<string> suggest { get; set; }
        }

        public class D_ReportAdvice
        {
            public string suggestcode { get; set; }
            public string suggestion { get; set; }
            public string summarystr { get; set; }
            public string suggestdirection { get; set; }
        }

        public class D_ReportTodayprediction
        {

        }
        public class D_ReportTarget
        {
            public string Title { get; set; }
            public string measuredValue { get; set; }
            public string referenceValue { get; set; }
            public int judge { get; set; }
        }

        public class D_ReportOfBreath
        {
            public List<D_ReportOfBreathPause> ReportOfBreathPause { get; set; }
            public List<D_BreathMaxCount> BreathMaxCount { get; set; }
            public long Benchmark { get; set; }
            public long BreathMax { get; set; }
            public long AvgPause { get; set; }
            public long BreathMin { get; set; }
            public long HigherCounts { get; set; }
            public long Longest { get; set; }
            public long Longterm { get; set; }
            public long LowerCounts { get; set; }
            public long Pause10Counts { get; set; }
            public long Pause30Counts { get; set; }
            public long PauseSum { get; set; }
            public long Average { get; set; }
            public long Shortest { get; set; }
            public decimal AHI { get; set; }
        }

        public class D_ReportOfBreathPause
        {
            public long StartTime { get; set; }
            public long EndTime { get; set; }
            public int longerval { get; set; }
        }

        public class D_BreathMaxCount
        {
            public int b { get; set; }
            public int c { get; set; }
        }

        public class D_ReportOfHeartBeat
        {
            public List<D_HeartbeatMaxCount> HeartbeatMaxCount { get; set; }
            public long Benchmark { get; set; }
            public long DayBenchmark { get; set; }
            public long DayCurBenchmark { get; set; }
            public long DayLongterm { get; set; }
            public long ExceedBenchmark125 { get; set; }
            public long ExceedBenchmark115 { get; set; }
            public long ExceedBenchmark83 { get; set; }
            public long ExceedBenchmark43 { get; set; }
            public long HeartbeatMax { get; set; }
            public long Average { get; set; }
            public long HeartbeatMin { get; set; }
            public long Higher { get; set; }
            public long HigherCounts { get; set; }
            public long HrvAverage { get; set; }
            public long HrvHigher { get; set; }
            public long HrvLower { get; set; }
            public long Longterm { get; set; }
            public long Lower { get; set; }
            public long LowerCounts { get; set; }
            public long NightCurBenchmark { get; set; }
            public long NightLongterm { get; set; }
        }

        public class D_HeartbeatMaxCount
        {
            public int b { get; set; }
            public int c { get; set; }
        }

        public class D_ReportOfSleep
        {
            public long ShallowRatio { get; set; }
            public long DeepRatio { get; set; }
            public long RemRatio { get; set; }
            public long Rem { get; set; }
            public long MoveCounts { get; set; }
            public long Shallow { get; set; }
            public long Deep { get; set; }
            public long SleepCounts { get; set; }
            public long SleepTime { get; set; }
            public long TotalTime { get; set; }
            public List<D_SLeepDetail> SLeepDetails { get; set; }

            public List<D_Move> Moves { get; set; }

            public long SoberRatio { get; set; }
            public long OffbedRatio { get; set; }
            public long Sober { get; set; }
            public long Offbed { get; set; }
            public long MovingAverageLength { get; set; }
        }

        public class D_SLeepDetail
        {
            public long Score { get; set; }
            public string Title { get; set; }
            public string Value { get; set; }

        }

        public class D_Move
        {
            public long starttime { get; set; }
            public long endtime { get; set; }
            public decimal score { get; set; }

        }

        public class D_ReportOfHRV
        {
            public long HeartIndex { get; set; }
            public long BaseTP { get; set; }
            public long BaseHF { get; set; }
            public long AvgHeart { get; set; }
            public long LF { get; set; }
            public long BaseLF { get; set; }
            public long HF { get; set; }
            public long LFHF { get; set; }
            public long BaseLFHF { get; set; }
            public long DcValue { get; set; }
            public long BaseDC { get; set; }
            public long DRsStr { get; set; }
            public long SDNN { get; set; }
            public long BaseSDNN { get; set; }
            public long endocrine { get; set; }
            public long temperature { get; set; }
            public long BaseULF { get; set; }
            public long BaseVLF { get; set; }
            public long Today { get; set; }
            public long SleepValue { get; set; }
            public long ReportDate { get; set; }
        }

        public class D_ReportOfOffbed
        {
            public long OffbedCounts { get; set; }
        }

        public class D_UserBaseInfo
        {
            public long Today { get; set; }
            public long HeartIndex { get; set; }
            public decimal AvgHeart { get; set; }
            public decimal LF { get; set; }
            public decimal HF { get; set; }
            public decimal LFHF { get; set; }
            public decimal DcValue { get; set; }
            public long DRsStr { get; set; }
            public decimal SDNN { get; set; }
            public long deepBili { get; set; }
            public List<object> BreathCounter { get; set; }
            public List<object> HeartbeatCounter { get; set; }
            public string VLFColor { get; set; }
            public string ULFColor { get; set; }
            public string TpColor { get; set; }
            public string SDNNColor { get; set; }
            public string LFHFColor { get; set; }
            public string LFColor { get; set; }
            public string HFColor { get; set; }
            public string DCColor { get; set; }
            public decimal BaseVLF { get; set; }
            public decimal BaseULF { get; set; }
            public decimal BaseDC { get; set; }
            public decimal BaseSDNN { get; set; }
            public decimal BaseLFHF { get; set; }
            public decimal BaseLF { get; set; }
            public decimal BaseTP { get; set; }
            public decimal BaseHF { get; set; }
            public decimal SleepValue { get; set; }
        }

        public class D_ReportChart
        {
            public List<D_ReportBarChart> ReportBarChart { get; set; }
            public List<D_ReportTrendChart> ReportTrendChart { get; set; }
            public List<D_ReportDaysTrentChart> ReportDaysTrentChart { get; set; }
        }

        public class D_ReportBarChart
        {
            public string ChartTypeId { get; set; }
            public string Title { get; set; }
            public string Comments { get; set; }
            public List<D_ReportBarChart_Item> Items { get; set; }
            public string ExtColumn1 { get; set; }
            public string ExtColumn2 { get; set; }
            public string ExtColumn3 { get; set; }

        }

        public class D_ReportBarChart_Item
        {
            public long endtime { get; set; }
            public long starttime { get; set; }

            public List<D_ReportBarChart_SubItem> subitems { get; set; }
        }

        public class D_ReportBarChart_SubItem
        {
            public long endtime { get; set; }
            public long starttime { get; set; }

            public int type { get; set; }
        }

        public class D_ReportTrendChart
        {
            public long type { get; set; }
            public long ChartTypeId { get; set; }
            public string Title { get; set; }
            public string Comments { get; set; }
            public long MaxAlert { get; set; }
            public long MinAlert { get; set; }
            public long NormalStart { get; set; }
            public long NormalEnd { get; set; }
            public string ExtColumn1 { get; set; }
            public string ExtColumn2 { get; set; }
            public string ExtColumn3 { get; set; }
            public List<int> XDataValue { get; set; }
            public List<long> XDataTime { get; set; }
        }

        public class D_ReportDaysTrentChart
        {
            public long ChartTypeId { get; set; }
            public string Title { get; set; }
            public List<D_ReportDaysTrentChart_Data> data { get; set; }
        }

        public class D_ReportDaysTrentChart_Data
        {
            public long time { get; set; }
            public string value { get; set; }
            public string color { get; set; }
            public decimal basevalue { get; set; }
        }
    }
}
