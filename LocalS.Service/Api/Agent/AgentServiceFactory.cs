using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Agent
{
    public class AgentServiceFactory
    {
        public static HomeService Home
        {
            get
            {
                return new HomeService();
            }
        }

        public static UserService User
        {
            get
            {
                return new UserService();
            }
        }
    }
}
