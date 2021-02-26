using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenvivSdk
{
    public class TokenResult
    {
        public string Application { get; set; }

        public string Access_token { get; set; }
        public string Expires_in { get; set; }
    }
}
