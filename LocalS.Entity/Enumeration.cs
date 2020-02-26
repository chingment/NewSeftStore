using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Entity
{
    public enum E_CartOperateType
    {

        Unknow = 0,
        Selected = 1,
        Increase = 2,
        Decrease = 3,
        Delete = 4
    }

    public enum E_AppCaller
    {
        Unknow = 0,
        Wxmp = 1,
        Term = 2
    }
}
