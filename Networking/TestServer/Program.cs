using System;
using System.IO;
using Networking;
using System.Net.Sockets;
using static System.Environment;
using TestServer.Properties;

namespace TestServer
{
    class Program
    {
        private const ushort port = 5000;
        private const bool localOnly = false;
        private const uint maxClients = 20;

        private PlainServer server;

        private string help, motd;

        static void Main()
        {
            Console.Title = "Server";

            new Program().StartServer();
        }

        private void StartServer()
        {
            if (File.Exists("help.txt")) { help = FileIO.ReadFromFile("help.txt"); }
            else { help = Resources.help; }
            if (File.Exists("motd.txt")) { motd = FileIO.ReadFromFile("motd.txt"); }
            else { motd = Resources.motd; }

            server?.Stop();
            server = new PlainServer
            {
                EnableLogging = true
            };
            server.OnStarted += Server_OnListeningChanged;
            server.OnStopped += Server_OnListeningChanged;
            server.OnConnected += Server_OnConnected;
            server.OnDisconnected += Server_OnDisconnected;
            server.OnConnecting += Server_OnConnecting;
            server.OnMessageReceived += Server_OnMessageReceived;

            string host = $"{(localOnly ? "127.0.0.1" : "0.0.0.0")}:{port}";
            Console.WriteLine($"Server >> Starting server @ {host}...");
            SetStatus("Server | Starting server @ " + host + "...");
            server.Start(port, localOnly);

            Console.Read(); // Hang here (Console application)
        }

        private void Server_OnListeningChanged()
        {
            Console.WriteLine($"Server >> Server has been st{(server.IsListening ? "art" : "opp")}ed!");
            SetStatus(server.IsListening ? "Listening on port " + port : ""); // Not listening
        }

        private bool Server_OnConnecting(TcpClient client)
        {
            bool canConnect = (server.Clients.Count < maxClients); // Simply allow clients if Server is not full

            if (!canConnect)
            {
                Console.WriteLine($"Server ({GetClientsCount()}) >> " + server.GetClientIdentity(client) + " tried to connect, but there's no space! Rejecting...");
                //server.SendMessage("!full", client);
            }
            /* else TODO Password / ban check ... */

            return canConnect;
        }

        private void Server_OnConnected(TcpClient client)
        {
            Console.WriteLine($"Server ({GetClientsCount()}) >> {server.GetClientIdentity(client, true, true)} has connected! Sending motd...");
            server.SendMessage("!motd " + GetDynamicMotd(), client); // Send motd as welcome message
        }

        private void Server_OnDisconnected(TcpClient client)
        {
            Console.WriteLine($"Server ({GetClientsCount()}) >> {server.GetClientIdentity(client, true, true)} has disconnected! ");
        }

        private void Server_OnMessageReceived(TcpClient client, string msg, bool isAccepted)
        {
            if (!msg.StartsWith("!")) // Normal message => Broadcast
            {
                Console.WriteLine($"Server ({GetClientsCount()}) >> Received '{msg}' from {server.GetClientIdentity(client, true, true)}; broadcasting to everyone...");
                server.BroadcastMessage(msg);
            }
            else // Server command => Handle accordingly
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

                Console.WriteLine($"Server ({GetClientsCount()}) >> Received command '" + msg + "' from " + server.GetClientIdentity(client, true, true) + "; handling now...");

                if (cmd == "ip") { server.SendMessage("!ip " + server.GetClientIdentity(client, false, true), client); }
                else if (cmd == "ping") { server.SendMessage("!pong", client); }
                else if (cmd == "h" || cmd == "help") { server.SendMessage("!h " + help, client); }
                else if (cmd == "motd") { server.SendMessage("!motd " + GetDynamicMotd(), client); }
                else { server.SendMessage("!unknown " + cmd, client); }
            }
        }

        #region Helper methods

        private string GetClientsCount()
        {
            return server.Clients.Count.ToString();//.PadLeft(MaxClients.ToString().Length, '0');
        }

        private string GetDynamicMotd()
        {
            return motd.Replace("{OTHER_CLIENTS_COUNT}", (server.Clients.Count - 1).ToString());
        }

        private void SetStatus(string status)
        {
            Console.Title = "Server" + (!String.IsNullOrEmpty(status) ? $" | {status}" : "");
        }

        #endregion
    }
}
