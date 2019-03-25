using System;
using Networking;
using System.Drawing;
using System.Windows.Forms;
using TestClient.Properties;

namespace TestClient
{
    public partial class ClientForm : Form
    {
        private string host = "";
        private PlainClient client;

        private DateTime lastMsgTime;
        private DateTime lastPingTime;

        #region ClientForm

        public ClientForm()
        {
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

            InitializeComponent();
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {
            txtNick.Text = Settings.Default.LastNick;
            txtHost.Text = Settings.Default.LastHost;

            AddMessage("[CLIENT] Settings loaded");
            AddMessage("[CLIENT] Ready to connect");
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.LastNick = txtNick.Text;
            Settings.Default.LastHost = txtHost.Text;
            Settings.Default.Save();
            AddMessage("[CLIENT] Settings saved");

            client?.Disconnect();
        }

        #endregion

        #region PlainClient

        private void Connect()
        {
            if (client == null || !client.IsConnected)
            {
                string[] parts = txtHost.Text.Split(':');
                string ip = parts[0];
                ushort port = 5000;
                if (parts.Length == 2 && !string.IsNullOrEmpty(parts[1]) && ushort.TryParse(parts[1], out ushort tmpPort)) { port = tmpPort; }

                client?.Disconnect();
                client = new PlainClient(this)
                {
                    EnableLogging = true
                };
                client.OnConnected += Client_OnConnectedChanged;
                client.OnDisconnected += Client_OnConnectedChanged;
                client.OnMessageReceived += Client_OnMessageReceived;
                txtHost.Enabled = false;
                host = $"{ip}:{port}";
                AddMessage("[CLIENT] Connecting to the Server @ " + host + "...");
                rtbMessages.Show();
                ChangeStatus("Connecting to " + host + "...");
                client.Connect(ip, port);
            }
        }

        private void Client_OnConnectedChanged()
        {
            try
            {
                bool isConnected = client.IsConnected;

                rtbMessages.Text = "";

                /*if (!isConnected) { rtbMessages.Text = ""; } // AddMessage("[CLIENT] The messages have been cleared as the connection to the Server was lost / could not be made");
                else { AddMessage("[CLIENT] Connection to the Server has been established!"); } // (isConnected ? "established" : "lost")*/

                txtHost.Enabled = !isConnected;
                txtChat.Enabled = isConnected;
                rtbMessages.Visible = isConnected;

                ChangeStatus(isConnected ? "Connected to " + host : "Not connected");

                this.ActiveControl = txtChat;
                txtChat.Select(txtChat.Text.Length, 0);
            }
            catch { }
        }

        private void SendMessage(string msg)
        {
            txtChat.Text = ""; // Reset chat input field
            msg = msg.Trim();
            if (!string.IsNullOrEmpty(msg) && DateTime.Now.Subtract(lastMsgTime).TotalMilliseconds >= 200) // TODO Make check server-side
            {
                if (!msg.StartsWith("!"))
                {
                    string user = txtNick.Text.Trim().Replace(":", ""); // No ':' in Nickname
                    client.SendMessage((!string.IsNullOrEmpty(user) ? user + ": " : "") + msg);
                }
                else
                {
                    bool clientSide = false;
                    lastMsgTime = DateTime.Now;

                    if (msg == "!ping" || msg.StartsWith("!ping ")) { lastPingTime = DateTime.Now; }
                    else if (msg == "!dc" || msg.StartsWith("!dc ")) { client.Disconnect(); rtbMessages.Text = ""; clientSide = true; }
                    else if (msg == "!clear" || msg.StartsWith("!clear ")) { rtbMessages.Text = ""; AddMessage("[CLIENT] The client-side messages have been cleared!"); clientSide = false; }

                    if (!clientSide) client.SendMessage(msg);
                }
            }
        }

        private void Client_OnMessageReceived(string msg)
        {
            if (!msg.StartsWith("!")) { AddMessage(msg); }
            else
            {
                #region Seperate 'msg' to 'cmd' & 'args'

                int msgSepIdx = msg.IndexOf(" ");
                string cmd = msg.Substring(1).ToLower();
                string args = "";

                if (msgSepIdx > 0)
                {
                    cmd = msg.Substring(1, msgSepIdx - 1).ToLower();
                    args = msg.Substring(msgSepIdx + 1);
                }

                #endregion

                if (cmd == "pong") { AddMessage("[CLIENT] Pong! Your ping to the Server is ~" + (int)(DateTime.Now.Subtract(lastPingTime).TotalMilliseconds / 2) + " ms"); }
                else if (cmd == "ip") { AddMessage("[CLIENT] Your public IP as reported by the Server is " + args); }
                else if (cmd == "h" || cmd == "motd") { AddMessage(args, false); }
                else if (cmd == "clear") { rtbMessages.Text = ""; }
                else if (cmd == "unknown") { AddMessage("[CLIENT] The issued command '" + args + "' is not available on the Client nor the Server!"); }
                /*else if(cmd == "full")
                {
                    client.Disconnect(true);
                    MessageBox.Show("Server is full(((");
                }*/
            }
        }

        #endregion

        #region User Interface

        private void AddMessage(string msg, bool showTime = true)
        {
            rtbMessages.Text = (showTime ? DateTime.Now.ToString("t") + " - " : "") + msg + (String.IsNullOrEmpty(rtbMessages.Text) ? "" : Environment.NewLine + rtbMessages.Text);
        }

        private void ChangeStatus(string status)
        {
            this.Text = "Client - " + status;
        }

        private void txtNick_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.ActiveControl = txtHost;
                e.Handled = true;
            }
        }

        private void txtHost_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Connect();
                e.Handled = true;
            }
        }

        private void txtChat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && client.IsConnected)
            {
                e.SuppressKeyPress = true;
                SendMessage(txtChat.Text);
                e.Handled = true;
            }
        }

        private Color ulColor;

        private void txtHost_EnabledChanged(object sender, EventArgs e)
        {
            ulColor = (txtHost.Enabled) ? Color.White : Color.FromArgb(109, 109, 109);
            picHostUL.BackColor = ulColor;
        }

        private void txtNick_EnabledChanged(object sender, EventArgs e)
        {
            ulColor = (txtNick.Enabled) ? Color.White : Color.FromArgb(109, 109, 109);
            picNickUL.BackColor = ulColor;
        }

        private void txtChat_EnabledChanged(object sender, EventArgs e)
        {
            ulColor = (txtChat.Enabled) ? Color.White : Color.FromArgb(109, 109, 109);
            picTxtChatUL.BackColor = ulColor;
        }

        #endregion
    }
}
