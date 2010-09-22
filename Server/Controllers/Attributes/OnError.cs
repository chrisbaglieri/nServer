using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Server.Controllers.Attributes
{
    /// <summary>
    /// Provides lightweight error handling capabilities without the
    /// need to explictly codify handlers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class OnErrorAttribute : CustomAttribute
    {
        #region Enumerations

        /// <summary>
        /// The end point of the error message.
        /// </summary>
        public enum ExceptionEndPoint
        {
            /// <summary>
            /// An email sent to a set of recipients.
            /// </summary>
            Email,

            /// <summary>
            /// A log message.
            /// </summary>
            Log
        }

        /// <summary>
        /// How the system behaves as a result of the error.
        /// </summary>
        public enum ServerBehavior
        {
            /// <summary>
            /// Remains available for clients.
            /// </summary>
            RemainActive,

            /// <summary>
            /// Blocks all clients from making subsequent calls.
            /// </summary>
            BlockClients
        }

        #endregion

        #region Members
        public readonly ExceptionEndPoint EndPoint;
        public readonly ServerBehavior Behavior;
        private string recipients = null;
        #endregion

        #region Constructors

        /// <summary>
        /// Empty default constructor.  Assumes logging the exception
        /// and keeping the server active.
        /// </summary>
        public OnErrorAttribute() 
        {
            EndPoint = ExceptionEndPoint.Log;
            Behavior = ServerBehavior.RemainActive;
        }

        /// <summary>
        /// Constructs the attribute with an end point and behavior.
        /// </summary>
        /// <param name="endPoint">end point</param>
        /// <param name="behavior">server behavior</param>
        public OnErrorAttribute(ExceptionEndPoint endPoint, ServerBehavior behavior)
        {
            this.EndPoint = endPoint;
            this.Behavior = behavior;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Comma delimited list of recipients used when the end point is email.
        /// </summary>
        public string Recipients
        {
            get { return this.recipients; }
            set { this.recipients = value; }
        }

        #endregion
    }
}