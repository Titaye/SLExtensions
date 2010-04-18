namespace SLExtensions.SocketClient
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    #region Enumerations

    public enum PersistantSocketState
    {
        Disconnected,
        Connecting,
        Connected
    }

    #endregion Enumerations

    /// <summary>
    /// EventArg containing a message from the server
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        #region Properties

        public string Message
        {
            get; set;
        }

        #endregion Properties
    }

    /// <summary>
    /// Control the Socket-based connectivity to a server
    /// </summary>
    public class PersistantSocketController : INotifyPropertyChanged, IDisposable
    {
        #region Fields

        bool _disposed;

        /// <summary>
        /// Server Endpoint
        /// </summary>
        private EndPoint _endPoint;

        /// <summary>
        /// Time between 2 "Ping" messages
        /// </summary>
        private TimeSpan _pingRate;

        /// <summary>
        /// Timer used in order to test connectivity
        /// </summary>
        private Timer _pingTimer;

        /// <summary>
        /// Managed Socket
        /// </summary>
        private Socket _realSocket;

        /// <summary>
        /// inbut buffer size
        /// </summary>
        int _receiveBufferLength;

        /// <summary>
        /// Current state of the SocketManager
        /// </summary>
        private PersistantSocketState _State = PersistantSocketState.Disconnected;

        /// <summary>
        /// Dispatcher used to raise events to the UI Thread
        /// </summary>
        private Dispatcher _uiThreadDispatcher;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Build a PersistantSocketController instance
        /// </summary>
        /// <param name="dispatcher">UI Thread dispatcher</param>
        /// <param name="hostName">Server hostname</param>
        /// <param name="port">Server port</param>
        /// <param name="pingRate">Time between 2 ping requests</param>
        /// <param name="receiveBufferLength">Socket buffer size</param>
        public PersistantSocketController(Dispatcher dispatcher, string hostName, int port, TimeSpan pingRate, int receiveBufferLength)
        {
            _uiThreadDispatcher = dispatcher;
            _endPoint = new DnsEndPoint(hostName, port, AddressFamily.InterNetwork);
            _pingRate = pingRate;
            _receiveBufferLength = receiveBufferLength;
        }

        ~PersistantSocketController()
        {
            Dispose(false);
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Raised when a message is received from server
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Current state of the SocketManager
        /// </summary>
        public PersistantSocketState State
        {
            get { return _State; }
            set
            {
                if (value == _State) return;

                _State = value;
                Debug.WriteLine("State of the PersistantSocketController modified : {0}", value);
                _uiThreadDispatcher.BeginInvoke(() =>
                {

                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("State"));
                });
            }
        }

        #endregion Properties

        #region Methods

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        /// <summary>
        /// Send a message to the server synchronously
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="System.Net.Sockets.SocketException"></exception>
        public void SendMessage(string message)
        {
            if (State != PersistantSocketState.Connected)
            {
                throw new InvalidOperationException("The socket server is unavailable");
            }
            bool success = false;
            using (AutoResetEvent are = new AutoResetEvent(false))
            {

                EventHandler<SocketAsyncEventArgs> sendMessageCallback = delegate(object sender, SocketAsyncEventArgs asyncArgs)
                {
                    if (asyncArgs.SocketError == SocketError.Success)
                        success = true;

                    are.Set();
                };
                if (!message.EndsWith("\r\n"))
                    message += "\r\n";
                var buffer = Encoding.UTF8.GetBytes(message);
                var toSend = new SocketAsyncEventArgs();
                toSend.SetBuffer(buffer, 0, buffer.Length);
                toSend.RemoteEndPoint = _endPoint;
                toSend.Completed += sendMessageCallback;
                if (!_realSocket.SendAsync(toSend))
                    sendMessageCallback(_realSocket, toSend);
                else
                    are.WaitOne();

                if (!success)
                {
                    State = PersistantSocketState.Disconnected;
                    throw new System.Net.Sockets.SocketException((int)toSend.SocketError);
                }

            }
        }

        /// <summary>
        /// Connect the socket and initialize the Ping Timer
        /// </summary>
        public void Start()
        {
            _pingTimer = new Timer(new TimerCallback(BeginConnect), null, TimeSpan.Zero, _pingRate);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            _disposed = true;

            if (disposing)
            {
                if (_pingTimer != null)
                {
                    _pingTimer.Dispose();
                    _pingTimer = null;
                }
                if (_realSocket != null)
                {
                    _realSocket.Dispose();
                    _realSocket = null;
                }
                State = PersistantSocketState.Disconnected;
            }
        }

        /// <summary>
        /// Connect or Ping the server
        /// </summary>
        void BeginConnect(object notUsed)
        {
            Debug.WriteLine("PersistantSocketController.BeginConnect [State : {0}]", State);
            if (State == PersistantSocketState.Disconnected)
            {
                if(_realSocket != null)
                    _realSocket.Dispose();
                _realSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // Connection...
                var sockArgs = new SocketAsyncEventArgs
                {
                    RemoteEndPoint = _endPoint
                };

                State = PersistantSocketState.Connecting;
                sockArgs.Completed += new EventHandler<SocketAsyncEventArgs>(_onConnected);
                if (!_realSocket.ConnectAsync(sockArgs))
                    _onConnected(_realSocket, sockArgs);
            }
            else if (State == PersistantSocketState.Connected)
            {
                // ping?

                byte[] buffer = System.Text.Encoding.UTF8.GetBytes("Ping?\r\n");
                var sockArgs = new SocketAsyncEventArgs();
                sockArgs.SetBuffer(buffer, 0, buffer.Length);
                sockArgs.RemoteEndPoint = _endPoint;
                sockArgs.Completed += new EventHandler<SocketAsyncEventArgs>(_onPing);
                if (!_realSocket.SendAsync(sockArgs))
                    _onPing(_realSocket, sockArgs);
            }
        }

        /// <summary>
        /// Message reception loop
        /// </summary>
        void ReceiveLoop()
        {
            if (State != PersistantSocketState.Connected)
                return;

            var sockArgs = new SocketAsyncEventArgs();
            sockArgs.SetBuffer(new Byte[_receiveBufferLength], 0, _receiveBufferLength);
            sockArgs.RemoteEndPoint = _endPoint;

            sockArgs.Completed += new EventHandler<SocketAsyncEventArgs>(_onReceived);

            Debug.WriteLine("PersistantSocketController : Beginning message reception");
            if (!_realSocket.ReceiveAsync(sockArgs))
                _onReceived(_realSocket, sockArgs);
        }

        /// <summary>
        /// Exucuted on Connection end
        /// </summary>
        void _onConnected(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {

                Debug.WriteLine("PersistantSocketContoller : Connection successful");
                State = PersistantSocketState.Connected;
                ReceiveLoop();
            }
            else
            {

                Debug.WriteLine("PersistantSocketContoller : Connexion failed : {0}", e.SocketError);
                State = PersistantSocketState.Disconnected;
            }
        }

        /// <summary>
        /// Executed on Ping End
        /// </summary>
        void _onPing(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                Debug.WriteLine("PersistantSocketContoller : Ping SuccessFull");
                // pong !

            }
            else
            {

                Debug.WriteLine("PersistantSocketContoller : Ping failed ! : {0}", e.SocketError);
                //Error
                State = PersistantSocketState.Disconnected;
            }
        }

        /// <summary>
        /// Executed when a message is accepted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _onReceived(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                Debug.WriteLine("PersistantSocketController : Message reception successful");
                string wholeMessage = Encoding.UTF8.GetString(e.Buffer, 0, e.BytesTransferred);
                var messages = wholeMessage.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (MessageReceived != null)
                    _uiThreadDispatcher.BeginInvoke(() =>
                    {
                        foreach (var message in messages)
                            MessageReceived(this, new MessageReceivedEventArgs { Message = message });
                    });

                ReceiveLoop();
            }
            else
            {

                Debug.WriteLine("PersistantSocketController : Message reception failed : {0}", e.SocketError);
                State = PersistantSocketState.Disconnected;
            }
        }

        #endregion Methods
    }
}