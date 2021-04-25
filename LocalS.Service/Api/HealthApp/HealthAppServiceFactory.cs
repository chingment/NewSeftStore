using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.HealthApp
{
    public class HealthAppServiceFactory
    {
        public static MonthReportService MonthReport
        {
            get
            {
                return new MonthReportService();
            }
        }
    }
}
