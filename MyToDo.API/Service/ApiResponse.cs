using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyToDo.API.Service
{
    public class ApiResponse
    {
        public ApiResponse(bool status, object result)
        {
            this.Status = status;
            this.Result = result;
        }
        public ApiResponse(string message, bool status = false)
        {
            this.Message = message;
            this.Status = status;
        }

        

        public string Message { get; set; }

        public bool Status { get; set; }

        public object Result { get; set; }
    }
}
