﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreSvcChat
{
    public class StoreSvcChatServiceFactory
    {
        public static UserService User
        {
            get
            {
                return new UserService();
            }
        }

        public static OwnService Own
        {
            get
            {
                return new OwnService();
            }
        }
    }
}