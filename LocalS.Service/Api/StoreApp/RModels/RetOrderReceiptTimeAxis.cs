using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RetOrderReceiptTimeAxis
    {
        public RetOrderReceiptTimeAxis()
        {
            this.Top = new TopModel();
            this.RecordTop = new RecordTopModel();
            this.Records = new List<RecordModel>();
        }

        public TopModel Top { get; set; }
        public RecordTopModel RecordTop { get; set; }
        public List<RecordModel> Records { get; set; }
        public class TopModel
        {
            public string CircleText { get; set; }
            public string Field1 { get; set; }
            public string Field2 { get; set; }
            public string Field3 { get; set; }
        }
        public class RecordModel
        {
            public string Time1 { get; set; }
            public string Time2 { get; set; }
            public string Status { get; set; }
            public string Description { get; set; }
            public bool IsLastest { get; set; }
        }
        public class RecordTopModel
        {
            public string CircleText { get; set; }
            public string Description { get; set; }
        }
    }
}
