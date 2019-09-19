using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetStoreInitManageMachine
    {
        public RetStoreInitManageMachine()
        {
            this.FormSelectMachines = new List<object>();
        }
        public string StoreName { get; set; }

        public List<object> FormSelectMachines { get; set; }
    }
}
