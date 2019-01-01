using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SysGuiApi.Control
{
    public class ServiceResponse
    {
        public int statusCode;
        public object message;

        public void Ok (object message)
        {
            this.statusCode = 200;
            this.message = message;
        }

        public void BadRequest(object message)
        {
            this.statusCode = 400;
            this.message = message;
        }
    }
}
