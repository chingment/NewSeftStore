using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RupSenvivGetUsers : RupBaseGetList
    {
        public string Name { get; set; }

        public string Sas { get; set; }

        public string Chronic { get; set; }

        public string Perplex { get; set; }

        public E_SvUserCareLevel CareLevel { get; set; }
    }
}
