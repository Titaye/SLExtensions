namespace SLExtensions.SocketServer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Base class for creation of a Silverlight-aware Socket Server
    /// </summary>
    public class SocketServerBase : Component
    {
        #region Fields

        Thread _acceptLoopThread;
        AutoResetEvent _clientAcceptedAre;
        private IPAddress _ipAddress = IPAddress.Any;
        TcpListener _listener;
        private int _port;
        private bool _running;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Address on which the server is listening
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IPAddress IPAddress
        {
            get { return _ipAddress; } set { if (_running)throw new InvalidOperationException("Listener is already running"); _ipAddress = value; }
        }

        /// <summary>
        /// Port on which the server is listening
        /// </summary>
        public int Port
        {
            get { return _port; } set { if (_running)throw new InvalidOperationException("Listener is already running"); _port = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Initialize the SocketServer and launch the Listener thread
        /// </summary>
        public virtual void Initialize()
        {
            _clientAcceptedAre = new AutoResetEvent(false);
            _running = true;
            _listener = new TcpListener(_ipAddress, _port);
            _listener.Start();
            _acceptLoopThread = new Thread(AcceptLoop);
            _acceptLoopThread.Start();
        }

        /// <summary>
        /// Clean unmanaged resource
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_running && _acceptLoopThread != null)
                {
                    _running = false;
                    _clientAcceptedAre.Set();
                    _acceptLoopThread.Join();
                }
                if (_clientAcceptedAre != null)
                {
                    _clientAcceptedAre.Close();
                    _clientAcceptedAre = null;
                }

            }
        }

        /// <summary>
        /// Implemented in inherited component to handle a client socket
        /// </summary>
        /// <param name="clientSocket">client-specific socket</param>
        protected virtual void OnSocketAccepted(Socket clientSocket)
        {
        }

        /// <summary>
        /// Client socket accept callback
        /// </summary>
        /// <param name="asyncres">Used for the EndAcceptSocket method call</param>
        private void AcceptAsyncCallback(IAsyncResult asyncres)
        {
            if (_listener != null)
            {

                var socket = _listener.EndAcceptSocket(asyncres);
                if (socket.Connected)
                {
                    OnSocketAccepted(socket);
                }
            }
            if (_clientAcceptedAre != null)
                _clientAcceptedAre.Set();
        }

        /// <summary>
        /// Listener loop
        /// </summary>
        private void AcceptLoop()
        {
            while (_running)
            {
                _clientAcceptedAre.Reset();
                _listener.BeginAcceptSocket(new AsyncCallback(AcceptAsyncCallback), null);
                _clientAcceptedAre.WaitOne();
            }
            _listener.Stop();
            _listener = null;
        }

        #endregion Methods
    }
}