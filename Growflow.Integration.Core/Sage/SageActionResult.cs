using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Growflo.Integration.Core.Sage
{
    public class SageActionResult<T> : ISageActionResult
    {
        public SageActionResult()
        {

        }

        public SageActionResult(T data, SageActionResultType result, string message)
        {
            Data = data;
            Result = result;
            Message = message;
        }

        public string Action { get; set; }

        public string Id { get; set; }

        public T Data { get; set; }

        public SageActionResultType Result { get; set; }

        public string Message { get; set; }

        public override string ToString()
        {
            return $"{Result}: {Message}";
        }
    }

    public enum SageActionResultType
    {
        NoAction,
        Error,
        Failure,
        Success
    }
}
