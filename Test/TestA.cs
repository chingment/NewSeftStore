using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public enum StoreStatus
    {
        [Remark("未知")]
        Unknow = 0,
        [Remark("设置中")]
        Setting = 1,
        [Remark("维护中")]
        Maintain = 2,
        [Remark("正常")]
        Opened = 3,
        [Remark("关闭")]
        Closed = 4
    }

    public class TestA
    {
        [Remark("关闭")]
        public const string Login = "Login";

        public StoreStatus Ma { get; set; }

        [Remark("关闭")]
        public string MaA { get; set; }

        public void hello()
        {

        }
    }
}
