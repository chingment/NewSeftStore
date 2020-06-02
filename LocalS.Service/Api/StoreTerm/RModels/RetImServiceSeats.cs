using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RetImServiceSeats
    {

        public RetImServiceSeats()
        {
            this.Seats = new List<ImSeatModel>();
        }

        public List<ImSeatModel> Seats { get; set; }
    }
}
