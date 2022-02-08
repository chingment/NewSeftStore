using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenvivSdk
{
    public class ReportParDetailResult
    {
        public int count { get; set; }

        public List<DataModel> data { get; set; }

        public class DataModel
        {
            public ReportParModel reportpar { get; set; }
            public List<TrendChartModel> trendchart { get; set; }
            public List<MvModel> mv { get; set; }
            public BarchartModel barchart { get; set; }
            public List<PModel> p { get; set; }
        }

        public class ReportParModel
        {
            public string userid { get; set; }
            public string ReportId { get; set; }
            public string sn { get; set; }
            public long FinishTime { get; set; }
            public long StartTime { get; set; }
            public long CreateTime { get; set; }
            public long OffbedTime { get; set; }
            public long OnbedTime { get; set; }
            public string hr { get; set; }
            public string lhr { get; set; }
            public string br { get; set; }
            public string lbr { get; set; }
            public string brz { get; set; }
            public string AHI { get; set; }
            public string TP { get; set; }
            public string LF { get; set; }
            public string HF { get; set; }
            public string LFHF { get; set; }
            public string ulf { get; set; }
            public string vlf { get; set; }
            public string dc { get; set; }
            public string sdnn { get; set; }
            public string im { get; set; }
            public string gr { get; set; }
            public string hc { get; set; }
            public string mc { get; set; }
            public string tc { get; set; }
            public string press { get; set; }
            public string gmmg { get; set; }
            public string emotion { get; set; }
            public string brfast { get; set; }
            public string brslow { get; set; }
            public string BaseTP { get; set; }
            public string BaseLF { get; set; }
            public string BaseHF { get; set; }
            public decimal BaseLFHF { get; set; }
            public string hrfast { get; set; }
            public string hrslow { get; set; }
            public string hr125 { get; set; }
            public string hr115 { get; set; }
            public string Sc_tst { get; set; }
            public string oft { get; set; }
            public string ot { get; set; }
            public string Sc_an { get; set; }
            public string dp { get; set; }
            public string rem { get; set; }
            public string sl { get; set; }
            public string rft_rst { get; set; }
            public string sffcy2 { get; set; }
            public string upHF { get; set; }
            public string upTP { get; set; }
            public string dpr { get; set; }
            public string slr { get; set; }
            public string inh { get; set; }
            public string ms { get; set; }
            public string st { get; set; }
            public string BaseSDNN { get; set; }
            public string BaseDC { get; set; }
            public string sct { get; set; }
            public string lr { get; set; }
            public string AwakeTimes { get; set; }
            public string WakingTime { get; set; }
            public string mct { get; set; }
            public string ofbdc { get; set; }
            public string ofbdc2 { get; set; }
            public string remr { get; set; }
            public string SleepContinuity { get; set; }
            public string upsdnn { get; set; }
            public string updc { get; set; }
            public string uplf { get; set; }
            public string uped { get; set; }
            public string uptpe { get; set; }
            public string ldpr { get; set; }
            public string reportCount { get; set; }
            public string qxxl { get; set; }
            public string gxb { get; set; }
            public string tnb { get; set; }
            public string gxy { get; set; }
            public string jbCount { get; set; }
            public string sleepValue { get; set; }
            public string mbs { get; set; }
            public List<string> AbnormalLabel { get; set; }
            public string hv { get; set; }
            public string avg { get; set; }
            public string max { get; set; }
            public string min { get; set; }
            public string bavg { get; set; }
            public string bmax { get; set; }
            public string bmin { get; set; }
            public string mvavg { get; set; }
            public string sr { get; set; }
            public string srr { get; set; }
            public string of { get; set; }
            public string hr85 { get; set; }
            public string hr75 { get; set; }
            public string Baseulf { get; set; }
            public string Basevlf { get; set; }
            public string hrv { get; set; }
            public string upTime { get; set; }
            public string mobile { get; set; }
            public string nick { get; set; }
            public string headimgurl { get; set; }
            public string hcNot { get; set; }
            public string mcNot { get; set; }
            public string tcNot { get; set; }
            public string age { get; set; }
            public string sex { get; set; }
            public string v { get; set; }
            public string ht { get; set; }
            public string gmml { get; set; }
            public string gmyq { get; set; }
            public string gmsr { get; set; }
            public string gmyp { get; set; }
            public List<string> Path { get; set; }
            public string remark { get; set; }
            public string remarkTime { get; set; }
            public string remarkUser { get; set; }
            public string _id { get; set; }
            public string deptid { get; set; }
        }

        public class TrendChartModel
        {
            public List< decimal> xdatavalue { get; set; }
            public List<decimal> xdatatime { get; set; }
            public decimal type { get; set; }
        }

        public class MvModel
        {
            public decimal e { get; set; }
            public decimal s { get; set; }
        }

        public class BarchartModel
        {
            public List<BarchartItemModel> items { get; set; }

            public decimal type { get; set; }
        }

        public class BarchartItemModel
        {
            public List<BarchartItemSubModel> sub { get; set; }
            public string st { get; set; }
            public string et { get; set; }
        }

        public class BarchartItemSubModel
        {
            public string st { get; set; }
            public string et { get; set; }
            public decimal type { get; set; }
        }

        public class PModel
        {
            public string e { get; set; }
            public string s { get; set; }
            public string i { get; set; }
        }
    }
}
