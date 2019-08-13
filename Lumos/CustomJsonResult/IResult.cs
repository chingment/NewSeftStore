using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos
{
    public enum ResultType
    {
        Unknown = 0,
        Success = 1,
        Failure = 2,
        Exception = 3
    }

    public static class ResultCode
    {
        public readonly static string Success = "1000";
        public readonly static string Failure = "2000";
        public readonly static string Failure2NoRight = "2401";
        public readonly static string Exception = "3000";
        public readonly static string NeedLogin = "5000";
    }


    public interface IResult<T>
    {
        ResultType Result { get; set; }
        string Code { get; set; }
        string Message { get; set; }

        T Data { get; set; }
    }

    public interface IResult : IResult<object>
    {

    }
}
