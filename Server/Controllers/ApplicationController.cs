using Server.Exceptions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Server.Controllers
{
    public abstract class ApplicationController
    {
        /// <summary>
        /// Request parameters
        /// </summary>
        public System.Collections.Specialized.NameValueCollection Parameters { get; set; }

        /// <summary>
        /// Invokes the specified action on the controller.
        /// </summary>
        /// <param name="actionName">name of the action</param>
        /// <returns>action response</returns>
        public Response InvokeAction(MethodInfo actionImplementation)
        {
            Response actionResponse = null;

            // get the action exception handler if one exists
            MethodInfo actionExceptionImplementation = null;
            foreach (MethodInfo methodInfo in this.GetType().GetMethods()) {
                if (methodInfo.Name.ToLower().Equals(actionImplementation.Name.ToLower() + "_exceptionhandler")) {
                    actionExceptionImplementation = methodInfo;
                }
            }

            // invoke the action if it exists and is in a ready state.
            // if an exception is thrown, apply any exception decorators
            // as well as call the action's exception handler if one exists
            string message = null;
            if (actionImplementation != null) {
                try {
                    if (this.CanInvoke(actionImplementation, out message)) {
                        actionResponse = (Response)actionImplementation.Invoke(this, null);
                    } else {
                        actionResponse = new Response(System.Net.HttpStatusCode.Conflict,
                            "Server is unable to handle your request",
                            message == null ? "Server unable to handle your request" : message);
                    }
                } catch (Exception ex) {
                    ExceptionHandler.HandleException(actionImplementation, ex);
                    if (actionExceptionImplementation != null) {
                        actionResponse = (Response)actionExceptionImplementation.Invoke(this, new object[] { ex });
                    } else {
                        actionResponse = new Response(System.Net.HttpStatusCode.InternalServerError,
                            "Server encountered an error while handling your request", ExtractExceptionMessage(ex));
                    }
                }
            } else {
                actionResponse = new Response(System.Net.HttpStatusCode.Conflict,
                    "Server is unable to handle your request",
                    message == null ? "Server unable to handle your request" : message);
            }

            return actionResponse;

        }

        /// <summary>
        /// Iterates over an action's custom attributes, applying each one.
        /// </summary>
        /// <param name="actionImplementation">action in question</param>
        /// <param name="message">outbound message</param>
        /// <returns></returns>
        private bool CanInvoke(MethodInfo actionImplementation, out string message)
        {
            bool canInvoke = true;
            message = null;

            // fetch all the custom attributes
            List<Attributes.CustomAttribute> customAttributes = Attributes.AttributeHelper.
                FetchAttributes<Attributes.CustomAttribute>(actionImplementation);
            
            // iterate over all the custom attributes and apply them.
            // stop iterating as soon as any return false.
            foreach (Attributes.CustomAttribute customAttribute in customAttributes) {
                canInvoke = customAttribute.Apply(this, actionImplementation, out message);
                if (!canInvoke) { break; }
            }

            return canInvoke;
        }

        /// <summary>
        /// Recursively gets the lowest level exception message
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private string ExtractExceptionMessage(Exception ex)
        {
            string exceptionMessage = null;
            if (ex.InnerException != null) {
                exceptionMessage = ExtractExceptionMessage(ex.InnerException);
            } else {
                exceptionMessage = ex.Message;
            }
            return exceptionMessage;
        }

        /// <summary>
        /// General controller ping action
        /// </summary>
        /// <returns></returns>
        public Response Ping()
        {
            StringBuilder message = new StringBuilder();
            message.Append("Alive and well!");
            if (this.Parameters.Count > 0) {
                message.Append(" { ");
                foreach (string key in this.Parameters.Keys) {
                    message.AppendLine(key).Append(" | ").Append(this.Parameters[key]).Append(", ");
                }
                message.Remove(message.ToString().Length - 2, 2);
                message.Append(" }");
            }
            return new Response(System.Net.HttpStatusCode.OK,
                "Ping", message.ToString());
        }
    }
}