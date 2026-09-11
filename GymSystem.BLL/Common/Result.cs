using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Common
{
    public record Result(bool Success,string? Error = null,ResultKind kind = ResultKind.Ok)
    {
        public static Result Ok() => new Result(true);
        public static Result Fail(string ErrorMessage,ResultKind kind = ResultKind.Conflict) => new Result(false,ErrorMessage,kind);
        public static Result NotFound(string ErrorMessage = "Not Found") => new Result(false,ErrorMessage,ResultKind.NotFound);
        public static Result Vaildation(string ErrorMessage) => new Result(false, ErrorMessage, ResultKind.ValidationFailed);
    }
    public record Result<T>(bool Success, T? Value, string? Error = null, ResultKind kind = ResultKind.Ok)
    {
        public static Result<T> Ok(T Value) => new(true, Value);
        public static Result<T> Fail(string ErrorMessage, ResultKind kind = ResultKind.Conflict) => new (false,default, ErrorMessage, kind);
        public static Result<T> NotFound(string ErrorMessage = "Not Found") => new (false, default,ErrorMessage, ResultKind.NotFound);
    }

    public enum ResultKind
    {
        Ok,
        NotFound,
        Conflict,
        ValidationFailed,
        Forbidden
    }
}
