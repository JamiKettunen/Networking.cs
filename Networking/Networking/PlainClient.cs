using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Networking
{
    public class PlainClient : IDisposable
    {
        /// <summary>
        /// The TcpClient object
        /// </summary>
        public TcpClient Client { get; private set; }
        /// <summary>
        /// Get connected status to the Server
        /// </summary>
        public bool IsConnected
        {
            get => _isConnected;
            private set
            {
                _isConnected = value;

                // Thread blocking calls
                if (_isConnected) InvokeAction(new Action(() => { OnConnected?.Invoke(); }));
                else
                {
                    _dataStream?.Close();
                    this.Client?.Close();

                    InvokeAction(new Action(() => { OnDisconnected?.Invoke(); }));
                }
            }
        }
        /// <summary>
        /// The receive buffer size
        /// </summary>
        public uint ReceiveBuffer = 2048;
        /// <summary>
        /// The message encoding format
        /// </summary>
        public Encoding Encoding = Encoding.UTF8;
        /// <summary>
        /// Time to wait until declaring a pending connection to a Server as timed out
        /// </summary>
        public TimeSpan ConnectionTimeout = TimeSpan.FromSeconds(3);
        /// <summary>
        /// Enable messages support? (enable the events & encode / decode data)
        /// </summary>
        public bool EnableMessages = true;
        /// <summary>
        /// Should runtime debug logs be enabled?
        /// </summary>
        public bool EnableLogging = false;

        private NetworkStream _dataStream; // The data NetworkStream between the Server & the Client
        private bool _isConnected;         // Current connected status to the Server
        private readonly Form _form;       // An optional Form object, used to make event usage seamless & integrate easily to WinForms

        public PlainClient(Form clientForm = null)
        {
            _form = clientForm;
        }

        #region Events

        public delegate void OnConnectedDelegate();
        /// <summary>
        /// Occurs when the Client's connection to the Server gets established.
        /// </summary>
        public event OnConnectedDelegate OnConnected;

        public delegate void OnDisconnectedDelegate();
        /// <summary>
        /// Occurs when the Client's connection to the Server drops.
        /// </summary>
        public event OnDisconnectedDelegate OnDisconnected;

        public delegate void OnDataReceivedDelegate(byte[] buffer, int bytes);
        /// <summary>
        /// Occurs when data is received from the Server.
        /// Gets Invoked BEFORE OnMessageReceived.
        /// </summary>
        public event OnDataReceivedDelegate OnDataReceived;

        public delegate void OnMessageReceivedDelegate(string msg);
        /// <summary>
        /// Occurs when a new message is received from the Server.
        /// Gets Invoked AFTER OnDataReceived & when EnableMessages == true.
        /// </summary>
        public event OnMessageReceivedDelegate OnMessageReceived;

        #endregion

        #region Public methods

        /// <summary>
        /// Tries to connect this Client to a specific host
        /// </summary>
        /// <param name="host">The server host</param>
        /// <param name="port">The server port</param>
        /// <param name="useThread">Should the connection be run in a separate thread?</param>
        public void Connect(string host, ushort port, bool useThread = true)
        {
            if (useThread)
            {
                Thread connThread = new Thread(new ThreadStart(() => ConnectBlocking(host, port)))
                {
                    IsBackground = true
                };
                connThread.Start();
            }
            else { ConnectBlocking(host, port); }
        }

        /// <summary>
        /// Disconnects this Client from the connected Server
        /// </summary>
        public void Disconnect()
        {
            if (this.IsConnected)
            {
                Log("PlainClient >> Disconnect()");
                this.IsConnected = false;
            }
        }

        /// <summary>
        /// Sends a message to the connected Server
        /// </summary>
        /// <param name="msg">The message to be sent</param>
        /// <returns>Send success (boolean)</returns>
        public bool SendMessage(string msg)
        {
            if (!this.EnableMessages)
            {
                Log("PlainClient >> Not sending message as they are not enabled");
                return false;
            }

            return SendData(Encoding.GetBytes(msg));
        }

        /// <summary>
        /// Write some bytes to the NetworkStream & flush it
        /// </summary>
        /// <param name="buffer">The data buffer</param>
        /// <returns>Send success (boolean)</returns>
        public bool SendData(byte[] buffer)
        {
            Log($"PlainClient >> Sending {buffer.Length} bytes of data to the server...");
            try
            {
                _dataStream.Write(buffer, 0, buffer.Length);
                _dataStream.Flush();
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Releases all resources used by this object
        /// </summary>
        public void Dispose()
        {
            Log("PlainClient >> Dispose()");
            _isConnected = false;
            _dataStream?.Close();
            _dataStream?.Dispose();
            this.Client?.Close();
            this.Client?.Dispose();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Thread blocking connection method for connecting to a Server
        /// </summary>
        /// <param name="host">Server hostname e.g. "something.chat" or "127.0.0.1"</param>
        /// <param name="port">Server port e.g. 5000</param>
        private void ConnectBlocking(string host, ushort port)
        {
            Log("PlainClient >> ConnectBlocking()");
            try
            {
                // Couldn't parse IPAddress => Try to resolve host
                if (!IPAddress.TryParse(host, out IPAddress a))
                {
                    Log("PlainClient >> Resolving host '" + host + "'...");
                    host = Dns.GetHostEntry(host).AddressList[0].ToString();
                    Log("PlainClient >> Host resolved to '" + host + "'");
                }

                this.Client = new TcpClient();
                if (this.Client.ConnectAsync(host, port).Wait(this.ConnectionTimeout) && this.Client.Connected)
                {
                    this.IsConnected = true;
                    Log("PlainClient >> Connected to '" + host + ":" + port + "'!");

                    _dataStream = this.Client.GetStream();

                    // Connection has been established! Start receiving data in the current thread...
                    ReceiveDataLoop();
                }
            }
            catch { this.Disconnect(); }
            //finally { dataStream?.Close(); this.Client?.Close(); }
        }

        /// <summary>
        /// Wait till some data arrives from the Server & set the return values
        /// </summary>
        /// <param name="buffer">The data buffer</param>
        /// <param name="bytes">The amount of bytes received</param>
        /// <returns>Receive success (boolean)</returns>
        private bool ReceiveData(out byte[] buffer, out int bytes)
        {
            try
            {
                buffer = new byte[ReceiveBuffer];
                bytes = _dataStream.Read(buffer, 0, buffer.Length);
                if (bytes == 0) throw new WebException(); // Received 0 bytes => assume closed connection => throw exception & return approperiate values
                Log($"PlainClient >> Received {bytes} bytes of data from the server!");
                return true;
            }
            catch { buffer = null; bytes = -1; return false; }
        }

        /// <summary>
        /// Thread blocking loop that listens for incoming data from the Server
        /// </summary>
        private void ReceiveDataLoop()
        {
            try
            {
                while (_isConnected)
                {
                    if (ReceiveData(out byte[] buff, out int bytes)) // Thread blocking
                    {
                        InvokeAction(new Action(() => { OnDataReceived?.Invoke(buff, bytes); })); // Thread blocking

                        if (this.EnableMessages)
                        {
                            try
                            {
                                string msg = GetString(buff, bytes);
                                if (!String.IsNullOrEmpty(msg)) InvokeAction(new Action(() => { OnMessageReceived?.Invoke(msg); })); // Thread blocking
                            }
                            catch { }
                        }
                    }
                    else { break; } // Disconnected
                }
            }
            catch { }

            this.Disconnect();
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

        #endregion
    }
}
