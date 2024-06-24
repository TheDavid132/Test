using SanlamTest.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanlamTest.Common.DTO
{
    public class Response<T>
    {
        public Response(T t,Status responseStatus)
        {
            Value = t;
            ResponseStatus = responseStatus;
        }

        public Response(Status responseStatus)
        {            
            ResponseStatus = responseStatus;
        }

        public Response(T? t)
        {
            Value = t;
            ResponseStatus = Status.Success;
        }

        public T? Value { get; set; }
        public Status ResponseStatus { get; set; }
    }
}
