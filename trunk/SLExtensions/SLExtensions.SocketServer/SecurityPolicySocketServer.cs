namespace SLExtensions.SocketServer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    using SLExtensions.SocketServer.Properties;

    /// <summary>
    /// Security policy socket server usable to authorize Silverlight clients to connect to the server
    /// </summary>
    public class SecurityPolicySocketServer : SocketServerBase
    {
        #region Fields

        const string _PolicyRequestString = "<policy-file-request/>";

        string[] _acceptedDomains = new string[] { };
        string[] _portRanges = new string[] { };

        #endregion Fields

        #region Constructors

        public SecurityPolicySocketServer()
        {
            base.Port = 943;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Accepted Silverlight application domains
        /// </summary>
        [Bindable(BindableSupport.Yes, BindingDirection.OneWay)]
        public string[] AcceptedDomains
        {
            get { return _acceptedDomains; }
            set { _acceptedDomains = value; }
        }

        /// <summary>
        /// Port du server de socket de police de sécurité
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new int Port
        {
            get { return base.Port; }
        }

        /// <summary>
        /// Ports du serveur de socket acceptant des connexions
        /// </summary>
        [Bindable(BindableSupport.Yes, BindingDirection.OneWay)]
        public string[] PortRanges
        {
            get { return _portRanges; }
            set { _portRanges = value; }
        }

        #endregion Properties

        #region Methods

        protected override void OnSocketAccepted(System.Net.Sockets.Socket clientSocket)
        {
            var mgr = new SecurityPolicySocketManager(clientSocket, this);
            mgr.Start();
        }

        #endregion Methods

        #region Nested Types

        /// <summary>
        /// Handle a policy file request
        /// </summary>
        private class SecurityPolicySocketManager
        {
            #region Fields

            Socket _clientSocket;
            private SecurityPolicySocketServer _server;

            #endregion Fields

            #region Constructors

            public SecurityPolicySocketManager(Socket clientSocket, SecurityPolicySocketServer server)
            {
                _clientSocket = clientSocket;
                _server = server;
            }

            #endregion Constructors

            #region Methods

            public void Start()
            {
                Thread t = new Thread(WaitForRequest);
                t.Start();
            }

            private void WaitForRequest()
            {
                byte[] buffer = new byte[_PolicyRequestString.Length];
                string message = string.Empty;
                try
                {
                    _clientSocket.Receive(buffer);
                    message = Encoding.UTF8.GetString(buffer);
                }
                catch (Exception ex)
                {

                }
                if (message == _PolicyRequestString)
                {
                    StringBuilder toSend = new StringBuilder(
                        Resources.SecurityPolicyTemplate);
                    toSend.Replace("$domains$",
                        string.Join("\r\n",
                            (from d in _server.AcceptedDomains
                             select string.Format("<domain uri=\"{0}\" />", d)).ToArray()));

                    toSend.Replace("$resources$",
                        string.Join("\r\n",
                            (from p in _server.PortRanges
                             select string.Format("<socket-resource port=\"{0}\" protocol=\"tcp\" />", p)).ToArray()));

                    _clientSocket.Send(Encoding.UTF8.GetBytes(toSend.ToString()));
                }

                _clientSocket.Close();
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}