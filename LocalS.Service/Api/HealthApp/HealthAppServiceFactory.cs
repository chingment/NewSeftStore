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

        public static OwnService Own
        {
            get
            {
                return new OwnService();
            }
        }

        public static DayReportService DayReport
        {
            get
            {
                return new DayReportService();
            }
        }

        public static DeviceService Device
        {
            get
            {
                return new DeviceService();
            }
        }

        public static ArticleService Article
        {
            get
            {
                return new ArticleService();
            }
        }

        public static ImitateService Imitate
        {
            get
            {
                return new ImitateService();
            }
        }

    }
}
