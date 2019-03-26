using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Networking
{
    public class PlainServer : IDisposable
    {
        /// <summary>
        /// The TcpListener object
        /// </summary>
        public TcpListener Listener { get; private set; }
        /// <summary>
        /// Get the Server's listening status
        /// </summary>
        public bool IsListening
        {
            get => _isListening;
            private set
            {
                if (_form != null && _form.InvokeRequired)
                {
                    _form.Invoke(new MethodInvoker(delegate { this.IsListening = value; }));
                    return; // Stop executing the non-invoked method call
                }

                _isListening = value;

                if (_isListening) OnStarted?.Invoke();
                else OnStopped?.Invoke();
            }
        }
        /// <summary>
        /// A list of all the connected clients
        /// </summary>
        public List<TcpClient> Clients { get; private set; }
        /// <summary>
        /// The receive buffer size
        /// </summary>
        public uint ReceiveBuffer = 2048;
        /// <summary>
        /// The message encoding format
        /// </summary>
        public Encoding Encoding = Encoding.UTF8;
        /// <summary>
        /// Enable messages support? (enable the events & encode / decode data)
        /// </summary>
        public bool EnableMessages = true;
        /// <summary>
        /// Should the client join accept rate be limited?
        /// </summary>
        public bool LimitJoinRate = true;
        /// <summary>
        /// Amount of milliseconds to wait before accepting more connections (LimitJoinRate == true)
        /// </summary>
        public ushort JoinRate = 200;
        /// <summary>
        /// Should runtime debug logs be enabled?
        /// </summary>
        public bool EnableLogging = false;

        private bool _isListening = false; // Current Server listening status
        private readonly Form _form;       // An optional Form object, used to make event usage seamless & integrate easily to WinForms

        public PlainServer(Form form = null)
        {
            _form = form;

            this.Clients?.Clear();
            this.Clients = new List<TcpClient>();
        }

        #region Events

        public delegate void OnStartedDelegate();
        /// <summary>
        /// Occurs when the Server gets started succesfully.
        /// </summary>
        public event OnStartedDelegate OnStarted;

        public delegate void OnStoppedDelegate();
        /// <summary>
        /// Occurs when the Server gets stopped.
        /// </summary>
        public event OnStoppedDelegate OnStopped;

        public delegate bool OnConnectingDelegate(TcpClient client);
        /// <summary>
        /// Occurs when a Client is connecting & gets accepted.
        /// Add possible authentication check etc here.
        /// Return boolean representing client acception state.
        /// </summary>
        public event OnConnectingDelegate OnConnecting;

        public delegate void OnConnectedDelegate(TcpClient client);
        /// <summary>
        /// Occurs when a Client's connection to the Server gets established.
        /// </summary>
        public event OnConnectedDelegate OnConnected;

        public delegate void OnDisconnectedDelegate(TcpClient client);
        /// <summary>
        /// Occurs when a Client's connection to the Server drops.
        /// </summary>
        public event OnDisconnectedDelegate OnDisconnected;

        public delegate void OnDataReceivedDelegate(TcpClient client, byte[] buffer, int bytes, bool isAccepted);
        /// <summary>
        /// Occurs when data is received from a Client.
        /// Gets Invoked BEFORE OnMessageReceived.
        /// </summary>
        public event OnDataReceivedDelegate OnDataReceived;

        public delegate void OnMessageReceivedDelegate(TcpClient client, string msg, bool isAccepted);
        /// <summary>
        /// Occurs when a new message is received from a Client.
        /// Gets Invoked AFTER OnDataReceived & when EnableMessages == true.
        /// </summary>
        public event OnMessageReceivedDelegate OnMessageReceived;

        #endregion

        #region Public methods

        /// <summary>
        /// Tries to start the Server on a specified port
        /// </summary>
        /// <param name="port">The port</param>
        /// <param name="useLoopback">Should this Server only be local?</param>
        /// <param name="useThread">Should the Server be run in a separate thread?</param>
        public void Start(ushort port, bool useLoopback = false, bool useThread = true)
        {
            Log($"PlainServer >> INFO: Starting server{(useThread ? " on a new thread" : "")}...");

            if (useThread)
            {
                Thread connThread = new Thread(new ThreadStart(() => ListenBlocking(port, useLoopback)))
                {
                    IsBackground = true
                };
                connThread.Start();
            }
            else { ListenBlocking(port, useLoopback); }
        }

        /// <summary>
        /// Stops the server
        /// </summary>
        public void Stop()
        {
            if (this.IsListening)
            {
                Log("PlainServer >> Stop()");
                this.IsListening = false;
            }
        }

        /// <summary>
        /// Broadcasts a message to all connected Clients
        /// </summary>
        /// <param name="msg">The message to be broadcasted</param>
        public void BroadcastMessage(string msg)
        {
            if (!this.EnableMessages)
            {
                Log("PlainServer >> Not broadcasting message as they are not enabled");
                return;
            }

            BroadcastData(Encoding.GetBytes(msg));
        }

        /// <summary>
        /// Broadcasts raw data bytes to all connected TCP Clients
        /// </summary>
        /// <param name="buffer">The data buffer</param>
        public void BroadcastData(byte[] buffer)
        {
            if (this.Clients.Count == 0) return; // No client's to broadcast the message to; stop executing

            Log($"PlainServer >> Broadcasting {buffer.Length} bytes to all connected clients...");

            try
            {
                // Broadcast the data to all connected Clients
                foreach (TcpClient client in this.Clients)
                    SendData(buffer, client);
            }
            catch { }
        }

        /// <summary>
        /// Send a message to the specified Client
        /// </summary>
        /// <param name="msg">The message to be sent</param>
        /// <param name="receiver">The receiving TcpClient</param>
        /// <returns>Send success (boolean)</returns>
        public bool SendMessage(string msg, TcpClient receiver)
        {
            if (!EnableMessages)
            {
                Log("PlainServer >> Not sending message as they are not enabled");
                return false;
            }

            return SendData(Encoding.GetBytes(msg), receiver);
        }

        /// <summary>
        /// Write some bytes to a Client's NetworkStream & flush it
        /// </summary>
        /// <param name="buffer">The data buffer</param>
        /// <param name="receiver">The receiving TcpClient</param>
        /// <returns>Send success (boolean)</returns>
        public bool SendData(byte[] buffer, TcpClient receiver)
        {
            try
            {
                NetworkStream stream = receiver.GetStream();
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Releases all resources used by this object
        /// </summary>
        public void Dispose()
        {
            Log("PlainServer >> Dispose()");
            _isListening = false;
            if (this.Clients != null)
            {
                foreach (TcpClient client in this.Clients)
                {
                    client?.Close();
                    client?.Dispose();
                }
                this.Clients.Clear();
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Thread blocking connection method for listening to new clients
        /// </summary>
        /// <param name="port">Listening port e.g. 420</param>
        /// <param name="useLoopback">Should the server be only available locally?</param>
        private void ListenBlocking(ushort port, bool useLoopback)
        {
            Log("PlainServer >> ListenBlocking()");
            try
            {
                // listener thread (connect) > message handler thread
                this.Listener = new TcpListener((useLoopback ? IPAddress.Loopback : IPAddress.Any), port);
                this.Listener.Start();
                this.IsListening = true;

                Log("PlainServer >> INFO: Now accepting connections " + (useLoopback ? "privately" : "publicly") + " on port '" + ((IPEndPoint)this.Listener.LocalEndpoint).Port + "'!");

                // Server now listening! Start accepting connecting clients... (this Thread)
                AcceptClientsLoop();
            }
            catch { Log("PlainServer >> ERROR: Server was not started on port '" + port + "'!"); this.Stop(); }
            finally { this.Listener?.Stop(); }

            Log("PlainServer >> INFO: The server has been stopped!");
        }

        /// <summary>
        /// Thread blocking loop that accepts incoming connections
        /// </summary>
        private void AcceptClientsLoop()
        {
            try
            {
                Log("PlainServer >> Listening for new client connections...");
                while (_isListening)
                {
                    if (!LimitJoinRate || this.Listener.Pending())
                    {
                        Thread connHandler = new Thread(new ThreadStart(() => HandleClient(this.Listener.AcceptTcpClient())));
                        Log("PlainServer >> A new client has connected!");
                        connHandler.IsBackground = true;
                        connHandler.Start();
                    }

                    if (LimitJoinRate)
                        Thread.Sleep(JoinRate);
                }
            }
            catch { this.Stop(); }
        }

        /// <summary>
        /// Threaded method to handle a client after it has connected to not block the listening thread
        /// </summary>
        /// <param name="client">The TcpClient</param>
        private void HandleClient(TcpClient client)
        {
            Log($"PlainServer >> HandleClient({GetClientIdentity(client, false, true)})");
            bool isAccepted = true;

            // Start listening for messages from the Client in a new Thread
            Thread lisMsgThread = new Thread(new ThreadStart(() => ReceiveMessagesLoop(client)))
            {
                IsBackground = true
            };
            lisMsgThread.Start();

            if (OnConnecting != null)  // Invoke client connecting event (if set)
            {
                InvokeAction(new Action(() => { isAccepted = OnConnecting.Invoke(client); })); // Thread blocking

                if (!isAccepted) lisMsgThread.Abort();
            }

            if (isAccepted)
            {
                this.Clients.Add(client);

                InvokeAction(new Action(() => { OnConnected?.Invoke(client); })); // Thread blocking
            }
            else { client.Close(); } // Client was rejected => Close connection & release resources

            Log($"PlainServer >> INFO: Client was {(isAccepted ? "accepted" : "rejected")}");
            // Log("PlainServer >> " + GetClientIdentity(connectingClient, true, true) + " was " + (isAccepted ? "accepted" : "rejected"));
        }

        /// <summary>
        /// Thread blocking loop that listens for incoming data from a Client
        /// </summary>
        /// <param name="client">The TcpClient</param>
        private void ReceiveMessagesLoop(TcpClient client)
        {
            NetworkStream dataStream = null;
            bool accepted = false;

            try
            {
                dataStream = client.GetStream();

                while (_isListening)
                {
                    Log($"PlainServer >> ReceiveMessagesLoop(\"{GetClientIdentity(client, true, true)}\"): waiting for some data to arrive...");
                    if (ReceiveData(dataStream, out byte[] buff, out int bytes)) // Thread blocking
                    {
                        Log($"PlainServer >> ReceiveMessagesLoop(\"{GetClientIdentity(client, true, true)}\"): received {bytes} bytes of data");

                        accepted = this.Clients.Contains(client);
                        InvokeAction(new Action(() => { OnDataReceived?.Invoke(client, buff, bytes, accepted); })); // Thread blocking

                        if (this.EnableMessages)
                        {
                            try
                            {
                                string msg = GetString(buff, bytes);
                                if (!String.IsNullOrEmpty(msg)) { InvokeAction(new Action(() => { OnMessageReceived?.Invoke(client, msg, accepted); })); } // Thread blocking
                            }
                            catch { }
                        }
                    }
                    else { break; } // Disconnected
                }
            }
            catch { }

            Log($"PlainServer >> ReceiveMessagesLoop(\"{GetClientIdentity(client, true, true)}\"): receive loop done, cleaning up...");

            try { this.Clients.Remove(client); } catch { }
            dataStream?.Close();
            client?.Close();

            InvokeAction(new Action(() => { OnDisconnected?.Invoke(client); })); // Thread blocking
        }

        /// <summary>
        /// Wait till some data arrives from a Client's NetworkStream & set the return values
        /// </summary>
        /// <param name="buffer">The data buffer</param>
        /// <param name="bytes">The amount of bytes received</param>
        /// <returns>Receive success (boolean)</returns>
        private bool ReceiveData(NetworkStream stream, out byte[] buffer, out int bytes)
        {
            try
            {
                buffer = new byte[ReceiveBuffer];
                bytes = stream.Read(buffer, 0, buffer.Length);
                if (bytes == 0) throw new WebException(); // Received 0 bytes => assume closed connection => return approperiate values
                return true;
            }
            catch { buffer = null; bytes = -1; return false; }
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Logs out a message w/ a timestamp to the Debug.Listeners collection when EnableLogging == true
        /// </summary>
        /// <param name="message">Message to log</param>
        private void Log(string message)
        {
            if (EnableLogging)
                Debug.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss.fff")}] {message}");
        }

        /// <summary>
        /// Returns a string decoded using the specified Encoding in the Client.
        /// Returns null if the message couldn't be decoded.
        /// </summary>
        /// <param name="buffer">The message buffer</param>
        /// <param name="bytes">The message bytes count</param>
        /// <returns>Decoded message string / null</returns>
        public string GetString(byte[] buffer, int bytes)
        {
            if (buffer == null && bytes < 1) return null;

            try { return Encoding.GetString(buffer, 0, bytes); }
            catch { return null; }
        }

        /// <summary>
        /// A helper method to make WinForms Action invoking seamless
        /// </summary>
        /// <param name="action">The action delegate void</param>
        private void InvokeAction(Action action)
        {
            if (_form != null && _form.InvokeRequired)
                _form.Invoke(new MethodInvoker(delegate { action(); }));
            else
                action();
        }

        /// <summary>
        /// Gets a Client's identity, e.g. connection number, IP address, ...
        /// </summary>
        /// <param name="client">The TcpClient</param>
        /// <param name="showClient">Include client connection number?</param>
        /// <param name="showIP">Include client IPAddress?</param>
        /// <returns></returns>
        public string GetClientIdentity(TcpClient client, bool showClient = true, bool showIP = false)
        {
            if (client == null || (!showClient && !showIP)) { return ""; }

            try
            {
                int num = (this.Clients.IndexOf(client) + 1);
                IPEndPoint ep = (IPEndPoint)client.Client.RemoteEndPoint;
                string ip = (ep.Address.ToString() + ":" + ep.Port);

                return (showClient ? (num > 0 ? "Client " + num : "A client") : "") +
                       (showIP ? (showClient ? " @ " + ip : ip) : "");
            }
            catch { return "A client"; }
        }

        #endregion
    }
}
