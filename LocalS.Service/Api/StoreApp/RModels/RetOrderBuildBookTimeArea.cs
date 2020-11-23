using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RetOrderBuildBookTimeArea
    {
        public RetOrderBuildBookTimeArea()
        {
            this.DateArea = new List<BookTimeDateAreaModel>();
        }

        public List<BookTimeDateAreaModel> DateArea { get; set; }
    }
}
