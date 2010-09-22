using System;
using System.Net;
using System.Globalization;
using System.Threading;

namespace Server
{
    public class nServer : IDisposable
    {
        /// <summary>
        /// Handle to the current instance of the server.
        /// </summary>
        public static nServer CurrentInstance = null;

        #region Members

        /// <summary>
        /// Server states
        /// </summary>
        public enum State
        {
            Stopped,
            Stopping,
            Started,
            Starting
        }

        /// <summary>
        /// Server's incoming request event handler
        /// </summary>
        public event EventHandler<HttpRequestEventArgs> IncomingRequest = null;

        private Thread _connectionManagerThread = null;
        private bool _disposed = false;
        private HttpListener _listener = null;
        private long _runState = (long)State.Stopped;

        #endregion

        #region Properties

        /// <summary>
        /// Server's current state
        /// </summary>
        public State CurrentState { get { return (State)Interlocked.Read(ref _runState); } }

        /// <summary>
        /// Server's unique identifier
        /// </summary>
        public Guid UniqueId { get; private set; }

        /// <summary>
        /// Server's root URL
        /// </summary>
        public Uri Endpoint { get; private set; }

        /// <summary>
        /// Flag to block all inbound requests to the server
        /// </summary>
        public bool BlockRequests { get; set; }

        #endregion

        #region Constructors/Destructors/Disposes

        /// <summary>
        /// Constructs a new server instance.
        /// </summary>
        /// <param name="listenerPrefix">the prefix the server should listen on</param>
        public nServer(Uri listenerPrefix)
        {
            // check to ensure httplistener is supported on the underlying OS
            if (!HttpListener.IsSupported) {
                throw new NotSupportedException("Use of System.Net.HttpListener is not supported on this operating system.");
            }

            // confirm a listener prefix was provided
            if (listenerPrefix == null) {
                throw new ArgumentNullException("listenerPrefix");
            }

            // keep track of the server endpoint
            this.Endpoint = listenerPrefix;

            // uniquely identify this server
            this.UniqueId = Guid.NewGuid();

            // go go gadget httplistener!
            this._listener = new HttpListener();
            this._listener.Prefixes.Add(listenerPrefix.AbsoluteUri);
        }

        /// <summary>
        /// Disposes the server.
        /// </summary>
        ~nServer()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Disposes the server
        /// </summary>
        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the server
        /// </summary>
        /// <param name="disposing">flag to queue thread cleanup</param>
        private void Dispose(bool disposing)
        {
            if (this._disposed) {
                return;
            }
            if (disposing) {
                if (this.CurrentState != State.Stopped) {
                    this.Stop();
                }
                if (this._connectionManagerThread != null) {
                    this._connectionManagerThread.Abort();
                    this._connectionManagerThread = null;
                }
            }
            this._disposed = true;
        }

        #endregion

        #region Server Commands

        /// <summary>
        /// Starts the server
        /// </summary>
        public virtual void Start()
        {
            // create the connection manager thread if one does not already exist
            if (this._connectionManagerThread == null || 
                this._connectionManagerThread.ThreadState == ThreadState.Stopped) {
                this._connectionManagerThread = new Thread(new ThreadStart(this.ConnectionManagerThreadStart));
                this._connectionManagerThread.Name = String.Format("ConnectionManager_{0}", this.UniqueId);
            } else if (this._connectionManagerThread.ThreadState == ThreadState.Running) {
                throw new ThreadStateException("Request handling process already enabled and running.");
            }

            // confirm the thread is initialized properly
            if (this._connectionManagerThread.ThreadState != ThreadState.Unstarted) {
                throw new ThreadStateException("Request handling process not properly initialized.");
            }

            // ready set go thread!
            this._connectionManagerThread.Start();

            // confirm we are ready to begin accepting requests
            long waitTime = DateTime.Now.Ticks + TimeSpan.TicksPerSecond * 10;
            while (this.CurrentState != State.Started) {
                Thread.Sleep(100);
                if (DateTime.Now.Ticks > waitTime) {
                    throw new TimeoutException("Request handling process failed to start.");
                }
            }
        }

        /// <summary>
        /// Starts the connection manager thread
        /// </summary>
        private void ConnectionManagerThreadStart()
        {
            Interlocked.Exchange(ref this._runState, (long)State.Starting);
            try {
                // start the listener up in a thread safe manner
                if (!this._listener.IsListening) {
                    this._listener.Start();
                }
                if (this._listener.IsListening) {
                    Interlocked.Exchange(ref this._runState, (long)State.Started);
                }

                // accept and handle inbound requests
                try {
                    while (CurrentState == State.Started) {
                        HttpListenerContext context = this._listener.GetContext();
                        this.RaiseIncomingRequest(context);
                    }
                } catch (HttpListenerException) {
                    // thrown when the listener shuts down; swallow it (nom nom nom) and move on
                }

            } finally {
                Interlocked.Exchange(ref this._runState, (long)State.Stopped);
            }
        }

        /// <summary>
        /// Raises the request and invokes the handler.
        /// </summary>
        /// <param name="context">the listener's context</param>
        private void RaiseIncomingRequest(HttpListenerContext context)
        {
            HttpRequestEventArgs requestEventArguments = new HttpRequestEventArgs(context);
            try {
                if (this.IncomingRequest != null) {
                    this.IncomingRequest.BeginInvoke(this, requestEventArguments, null, null);
                }
            } catch {
                // swallow the exception (nom nom nom) and log it
                // you probably don't want to exit just because an request handler failed
            }
        }

        /// <summary>
        /// Stops the server
        /// </summary>
        public virtual void Stop()
        {
            // setting the runstate to something other than "started" and
            // stopping the listener allows the ConnectionManagerThreadStart 
            // sequence to end, which sets the runstate to "stopped".
            Interlocked.Exchange(ref this._runState, (long)State.Stopping);
            if (this._listener.IsListening) {
                this._listener.Stop();
            }
            long waitTime = DateTime.Now.Ticks + TimeSpan.TicksPerSecond * 10;
            while (this.CurrentState != State.Stopped) {
                Thread.Sleep(100);
                if (DateTime.Now.Ticks > waitTime) {
                    throw new TimeoutException("Unable to stop the web server process.");
                }
            }

            this._connectionManagerThread = null;
        }

        #endregion

        #region HttpRequestEventArgs

        /// <summary>
        /// Class containing the reqeust event arguments
        /// </summary>
        public class HttpRequestEventArgs : EventArgs
        {
            public HttpListenerContext RequestContext { get; private set; }
            public HttpRequestEventArgs(HttpListenerContext requestContext)
            {
                this.RequestContext = requestContext;
            }
        }

        #endregion
    }
}