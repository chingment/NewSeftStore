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
            public int Benchmark { get; set; }
            public int BreathMax { get; set; }
            public int AvgPause { get; set; }
            public int BreathMin { get; set; }
            public int HigherCounts { get; set; }
            public int Longest { get; set; }
            public int Longterm { get; set; }
            public int LowerCounts { get; set; }
            public int Pause10Counts { get; set; }
            public int Pause30Counts { get; set; }
            public int PauseSum { get; set; }
            public int Average { get; set; }
            public int Shortest { get; set; }
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
            public int DayCurBenchmark { get; set; }
            public int DayLongterm { get; set; }
            public int ExceedBenchmark125 { get; set; }
            public int ExceedBenchmark115 { get; set; }
            public int ExceedBenchmark83 { get; set; }
            public int ExceedBenchmark43 { get; set; }
            public int HeartbeatMax { get; set; }
            public int Average { get; set; }
            public int HeartbeatMin { get; set; }
            public int Higher { get; set; }
            public int HigherCounts { get; set; }
            public int HrvAverage { get; set; }
            public int HrvHigher { get; set; }
            public int HrvLower { get; set; }
            public int Longterm { get; set; }
            public int Lower { get; set; }
            public int LowerCounts { get; set; }
            public int NightCurBenchmark { get; set; }
            public int NightLongterm { get; set; }
        }

        public class D_HeartbeatMaxCount
        {
            public int b { get; set; }
            public int c { get; set; }
        }

        public class D_ReportOfSleep
        {
            public decimal ShallowRatio { get; set; }
            public decimal DeepRatio { get; set; }
            public decimal RemRatio { get; set; }
            public int Rem { get; set; }
            public int MoveCounts { get; set; }
            public int Shallow { get; set; }
            public int Deep { get; set; }
            public int SleepCounts { get; set; }
            public long SleepTime { get; set; }
            public long TotalTime { get; set; }
            public List<D_SLeepDetail> SLeepDetails { get; set; }

            public List<D_Move> Moves { get; set; }

            public decimal SoberRatio { get; set; }
            public decimal OffbedRatio { get; set; }
            public int Sober { get; set; }
            public int Offbed { get; set; }
            public int MovingAverageLength { get; set; }
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
            public int HeartIndex { get; set; }
            public int BaseTP { get; set; }
            public int BaseHF { get; set; }
            public int AvgHeart { get; set; }
            public int LF { get; set; }
            public int BaseLF { get; set; }
            public int HF { get; set; }
            public int LFHF { get; set; }
            public int BaseLFHF { get; set; }
            public int DcValue { get; set; }
            public int BaseDC { get; set; }
            public int DRsStr { get; set; }
            public int SDNN { get; set; }
            public int BaseSDNN { get; set; }
            public decimal endocrine { get; set; }
            public decimal temperature { get; set; }
            public int BaseULF { get; set; }
            public int BaseVLF { get; set; }
            public int Today { get; set; }
            public int SleepValue { get; set; }
            public int ReportDate { get; set; }
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
