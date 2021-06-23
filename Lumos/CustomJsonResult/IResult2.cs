using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos
{
    public interface IResult2<T>
    {
        string Code { get; set; }
        string Msg { get; set; }
        T Data { get; set; }
    }

    public interface IResult2 : IResult2<object>
    {

    }
}
