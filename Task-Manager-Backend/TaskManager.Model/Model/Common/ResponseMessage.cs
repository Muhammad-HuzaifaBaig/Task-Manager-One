using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Model.Model.Common
{
    public class ResponseMessage : ResponseMessage<object>
    {
        public ResponseMessage(bool success, object data, string message, int statusCode = 200) 
            : base(success, data, message, statusCode)
        {
        }

        public static ResponseMessage Ok(object data = null!, string message = "Success")
        {
            return new ResponseMessage(true, data, message, 200);
        }

        public static ResponseMessage Created(object data = null!, string message = "Created")
        {
            return new ResponseMessage(true, data, message, 201);
        }

        public static ResponseMessage BadRequest(string message = "Bad Request", object data = null!)
        {
            return new ResponseMessage(false, data, message, 400);
        }

        public static ResponseMessage Unauthorized(string message = "Unauthorized", object data = null!)
        {
            return new ResponseMessage(false, data, message, 401);
        }

        public static ResponseMessage NotFound(string message = "Not Found", object data = null!)
        {
            return new ResponseMessage(false, data, message, 404);
        }

        public static ResponseMessage Error(string message = "Internal Server Error", object data = null!)
        {
            return new ResponseMessage(false, data, message, 500);
        }
    }

    public class ResponseMessage<T> where T : class
    {
        public bool success { get; set; }
        public T data { get; set; }
        public string message { get; set; }
        public int statusCode { get; set; }

        public ResponseMessage(bool success, T data = null!, string message = "", int statusCode = 200)
        {
            this.success = success;
            this.data = data;
            this.message = message;
            this.statusCode = statusCode;
        }
    }
}
