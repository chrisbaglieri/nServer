using Server.Controllers;
using Server.Controllers.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Server.Exceptions
{
    internal class ExceptionHandler {

        /// <summary>
        /// Handles exceptions thrown from the server. Note, we rely on
        /// the OnError attribute to determine how to handle the exception.
        /// </summary>
        /// <param name="source">class that the exception was thrown</param>
        /// <param name="exception">throw exception</param>
        internal static void HandleException(MethodInfo actionImplementation, Exception exception)
        {
            // grab the custom attributes so we know what to do
            List<OnErrorAttribute> onErrorAttributes = AttributeHelper.
                FetchAttributes<OnErrorAttribute>(actionImplementation);

            // deal with any and all endpoints
            DealWithEndPoints(onErrorAttributes, exception);

            // deal with any and all behaviors
            DealWithBehaviors(onErrorAttributes);
        }

        /// <summary>
        /// Deals with the various end points specified in the attribute
        /// </summary>
        /// <param name="onErrorAttributes">collection of related attributes</param>
        /// <param name="exception">root exception</param>
        /// <param name="sourceController">controller where the exception was thrown</param>
        /// <param name="sourceAction">action where the exception was thrown</param>
        private static void DealWithEndPoints(List<OnErrorAttribute> onErrorAttributes, Exception exception) {            
            foreach (OnErrorAttribute onErrorAttribute in onErrorAttributes) {
                switch (onErrorAttribute.EndPoint) {
                    case OnErrorAttribute.ExceptionEndPoint.Email:
                        if (!String.IsNullOrEmpty(onErrorAttribute.Recipients)) {
                            Messaging.EmailHelper.SendMail(
                                onErrorAttribute.Recipients,
                                "server@noreply.com",
                                String.Format("Server Exception Thrown: {0}", exception.Message),
                                exception.StackTrace);
                        }
                        break;
                    case OnErrorAttribute.ExceptionEndPoint.Log:
                        Logging.LogHelper.LogMessage(
                            String.Format("Server Exception Thrown"),
                            Server.Logging.LogLevel.Error,
                            exception);
                        break;
                }
            }
        }

        /// <summary>
        /// Detals with the behaviors of the on error attribute.
        /// </summary>
        /// <param name="onErrorAttributes">collection of related attributes</param>
        private static void DealWithBehaviors(List<OnErrorAttribute> onErrorAttributes) {
            foreach (OnErrorAttribute onErrorAttribute in onErrorAttributes) {
                switch (onErrorAttribute.Behavior) {
                    case OnErrorAttribute.ServerBehavior.RemainActive:
                        // do nothing
                        break;
                    case OnErrorAttribute.ServerBehavior.BlockClients:
                        nServer.CurrentInstance.BlockRequests = true;
                        return;
                }
            }
        }
    }
}