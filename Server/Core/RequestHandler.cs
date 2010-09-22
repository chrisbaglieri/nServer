using Server.Controllers;
using Server.Controllers.Attributes;
using Server.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

namespace Server
{
    public static class RequestHandler
    {
        /// <summary>
        /// Handles the incoming request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void HandleIncomingRequest(object sender, nServer.HttpRequestEventArgs e)
        {
            ApplicationController controller = null;
            MethodInfo action = null;

            // if the server is accepting commands, invoke the action
            string message = null;
            if (IsActionAvailable(e, out controller, out action,  out message)) {
                if (CheckServer(out message)) {
                    HandleResponse(e, controller.InvokeAction(action));
                    Logging.LogHelper.LogMessage(
                        String.Format("Successfullly invoked {0}...",
                        e.RequestContext.Request.Url.ToString()), 
                        LogLevel.Debug);
                } else {
                    HandleResponse(e, new Response(HttpStatusCode.ServiceUnavailable, 
                        String.Format("Failed to invoke {0}...", 
                        e.RequestContext.Request.Url.ToString()),
                        message));
                    LogHelper.LogMessage(
                        String.Format("Failed to invoke {0} for the following reason: {1}...",
                        e.RequestContext.Request.Url.ToString(),
                        message), 
                        LogLevel.Warning);
                }
            } else {
                if (!e.RequestContext.Request.Url.ToString().ToLower().Contains("favicon")) {
                    HandleResponse(e, new Response(HttpStatusCode.ServiceUnavailable,
                        String.Format("Failed to invoke {0}...", 
                        e.RequestContext.Request.Url.ToString()), 
                        message));
                    LogHelper.LogMessage(String.Format("Failed to invoke {0} for the following reason: {1}...", 
                        e.RequestContext.Request.Url.ToString(), 
                        message), 
                        LogLevel.Warning);
                }
            }

        }

        /// <summary>
        /// Checks to see if the requeste action is available.
        /// </summary>
        /// <param name="args">the event arguments</param>
        /// <param name="controllerToInvoke">controller to invoke</param>
        /// <param name="actionToInvoke">action to invoke</param>
        /// <param name="message">outbound message</param>
        /// <returns></returns>
        private static bool IsActionAvailable(nServer.HttpRequestEventArgs args, 
            out ApplicationController controllerToInvoke, 
            out MethodInfo actionToInvoke, 
            out string message)
        {
            bool isActionAvailable = false;
            Uri requestedAction = args.RequestContext.Request.Url;
            message = null;
            controllerToInvoke = null;
            actionToInvoke = null;
            string[] requestParts = requestedAction.LocalPath.Split('/');
            if (requestParts.Length == 3) {
                controllerToInvoke = Controllers.ControllerManager.GetControllerInstance(requestParts[1] + "controller");
                if (controllerToInvoke != null) {
                    controllerToInvoke.Parameters = args.RequestContext.Request.QueryString;
                    actionToInvoke = Controllers.ControllerManager.GetControllerAction(controllerToInvoke, requestParts[2]);
                }
            }
            if (controllerToInvoke != null && actionToInvoke != null) {
                isActionAvailable = true;
            } else {
                if (controllerToInvoke == null) {
                    message = "Controller does not exist.";
                } else if (actionToInvoke == null) {
                    message = "Action does not exist.";
                }
            }
            return isActionAvailable;
        }

        /// <summary>
        /// Confirms the server is accepting requests
        /// </summary>
        /// <param name="message">outbound message</param>
        /// <returns>whether or not the server is accepting requests</returns>
        private static bool CheckServer(out string message)
        {
            message = null;
            bool serverCanAcceptRequest = true;
            if (nServer.CurrentInstance.BlockRequests) {
                message = "Server is currently blocking all inbound requests.";
                serverCanAcceptRequest = false;
            }
            return serverCanAcceptRequest;
        }

        /// <summary>
        /// Handles the actual response
        /// </summary>
        /// <param name="args">the event arguments</param>
        /// <param name="response">the response from the action</param>
        private static void HandleResponse(nServer.HttpRequestEventArgs args, Response response)
        {
            HttpListenerResponse httpResponse = args.RequestContext.Response;
            httpResponse.StatusCode = (int)response.StatusCode;
            httpResponse.StatusDescription = response.StatusDescription;
            httpResponse.ContentLength64 = response.MessageStream.Length;
            httpResponse.ContentEncoding = Encoding.UTF8;
            httpResponse.OutputStream.Write(response.MessageStream, 0, response.MessageStream.Length);
            httpResponse.OutputStream.Close();
            httpResponse.Close();
        }
    }
}