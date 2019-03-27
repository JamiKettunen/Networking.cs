# Networking.cs
🌐 A C# TCP networking API meant to make things simple

## Usage
1. Add references to `Networking.dll` (& `System.Windows.Forms`) in the project
2. Add `using Networking;` atop the required files
3. Enjoy a simpler network programming experience :)

## Documentation
[Coming soon!](https://github.com/JamiKettunen/Networking.cs/wiki)

Currently only `PlainClient` & `PlainServer` classes are available for use and all of their available methods, events etc should give a good enough understanding for now via summaries.

## Examples

### Client

```C#
...
PlainClient client = new PlainClient();
client.OnConnected += () =>
{
    // Established connection with the server!
};
client.OnMessageReceived += (msg) =>
{
    // Received a message from the server!
};
client.OnDisconnected += () =>
{
    // Connection to the server has dropped!
};
client.Connect("127.0.0.1", 8080); // Connect to host @ 127.0.0.1:8080
...
```

### Server

```C#
...
PlainServer server = new PlainServer();
server.OnStarted += () =>
{
    // The server has been started!
};
server.OnConnected += (client) =>
{
    // A client has connected to the server!
};
server.OnMessageReceived += (client, msg, isAccepted) =>
{
    // A message was received from a client!
};
server.OnDisconnected += (client) =>
{
    // A client has disconnected from the server!
};
server.OnStopped += () =>
{
    // The server has been stopped!
};
server.Start(8080); // Start server on port 8080
...
```

## Contribution
Critical feedback and feature requests are welcome in the form of [issues](https://github.com/JamiKettunen/Networking.cs/issues).  
Code contributions as [pull requests](https://github.com/JamiKettunen/Networking.cs/pulls) please! :)
