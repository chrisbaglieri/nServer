using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Server.Controllers
{
    public class Response
    {
        /// <summary>
        /// Response constructor
        /// </summary>
        /// <param name="statusCode">http status code</param>
        /// <param name="statusDescription">http status description</param>
        /// <param name="message">response</param>
        public Response(HttpStatusCode statusCode, string statusDescription, string message)
        {
            StatusCode = statusCode;
            StatusDescription = statusDescription;
            Message = message;
        }

        /// <summary>
        /// Generic response constructor.  Success equates to an HttpStatus of 200 (e.g. OK).
        /// Failure equates to an HttpStatus of 400 (e.g. Internal Server Error).
        /// </summary>
        /// <param name="requestCompleted">whether or not the request was successfully carried out</param>
        /// <param name="statusDescription">http status description</param>
        /// <param name="message">response</param>
        public Response(bool requestCompleted, string statusDescription, string message)
        {
            if (requestCompleted) {
                StatusCode = HttpStatusCode.OK;
            } else {
                StatusCode = HttpStatusCode.InternalServerError;
            }
            StatusDescription = statusDescription;
            Message = message;
        }

        /// <summary>
        /// Http Status Code
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        
        /// <summary>
        /// Status Descritpion
        /// </summary>
        public string StatusDescription { get; set; }

        /// <summary>
        /// Overall response message
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// Message byte stream (read only)
        /// </summary>
        internal byte[] MessageStream 
        {
            get
            {
                byte[] stream = new byte[0];
                if (Message != null) {
                    stream = Encoding.UTF8.GetBytes(Message);
                }
                return stream;
            }
        }
    }
}