using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net.Security;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;


namespace Wophi {

  public class WebsockServerClient {
    private readonly WebsockServer server;
    private readonly WebsockProtocol protocol;

    public event Func<byte[], Task> OnMessage;
    public event Func<ushort, string, Task> OnClose;

    public WebsockServerClient(WebsockServer server, ref ConnectionStream stream) {
      this.server = server;
      this.protocol = new WebsockProtocol(ref stream);
    }

    public void Ping(string data, Func<byte[], Task> callback) {
      PingAsync(data, callback).GetAwaiter().GetResult();
    }

    public async Task PingAsync(string data, Func<byte[], Task> callback) {
      await protocol.PingAsync(data, callback);
    }

    public async Task StartAsync() {
      // handle incoming data
      try {
        while (true)
          await OnMessage(await protocol.GetMessage());

      // handle websock closing
      } catch (WebsockCloseException ex) {
        await OnClose(ex.Code, ex.Reason);

      // close client and destroy stream on exit
      } finally {
        if (server.GetClients().Contains(this))
          server.GetClients().Remove(this);
        protocol.Dispose();
      }
    }
  }

  public class WebsockServer : IDisposable {
    private bool isSsl;
    private bool running;
    private readonly TcpListener server;
    private List<WebsockServerClient> clients;
    private CancellationTokenSource tokenSource;

    // Special Websocket Server GUID for prepending client keys
    private static readonly string WebsockGUID = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

    // Server http handshake response template
    private static readonly string HttpHandshakeSucces =
      "HTTP/1.1 101 Switching Protocols\r\n" +
      "Upgrade: websocket\r\n" +
      "Connection: Upgrade\r\n" +
      "Sec-WebSocket-Accept: {WebsockKey}\r\n\r\n";

    // Server http error response template
    private static readonly string HttpErrorTemplate =
      "HTTP/1.1 400 Bad Request\r\n" +
      "Server: Wophi\r\n" +
      "Content-Type: {ContentType}\r\n" +
      "Content-Length: {ContentLength}\r\n" +
      "\r\n{Content}";

    // constructor
    public WebsockServer(IPAddress address, int port, bool ssl) {
      isSsl = ssl;
      server = new TcpListener(address, port);
      clients = new List<WebsockServerClient>();
      server.Server.SetSocketOption(
        SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
    }

    // disposable stop
    public void Dispose() {
      Stop();
    }

    // start server and block (synchronously)
    public void Start() {
      StartAsync().GetAwaiter().GetResult();
    }

    // stop running server if running
    public void Stop() {
      if (tokenSource != null)
        tokenSource.Cancel();
    }

    // get list of clients connected to server
    public List<WebsockServerClient> GetClients() {
      return clients;
    }

    // Create Sec-WebSocket-Accept key from Sec-Web
    private string GenerateKey(string clientKey) {
      byte[] hash = SHA1.Create().ComputeHash(
        Encoding.UTF8.GetBytes(clientKey + WebsockGUID));
      return System.Convert.ToBase64String(hash);
    }

    // handle incoming clients
    private async Task HandleClient(ConnectionStream stream) {
      if (!(await HandshakeClient(stream))) return;
      WebsockServerClient serverClient = new WebsockServerClient(this, ref stream);
      clients.Add(serverClient);
      await serverClient.StartAsync();
    }

    // Deny client with http error message
    private async Task<bool> RejectClient(ConnectionStream stream, string errorMessage) {
      byte[] errorData = Encoding.UTF8.GetBytes(HttpErrorTemplate
        .Replace("{Content}", errorMessage)
        .Replace("{ContentType}", "text/plain")
        .Replace("{ContentLength}", errorMessage.Length.ToString()));
      await stream.WriteAsync(errorData, errorData.Length);
      stream.Dispose();
      return false;
    }

    // Perform websocket handshake with client 
    private async Task<bool> HandshakeClient(ConnectionStream stream) {

      // read and parse http request data
      byte[] httpData = await stream.ReadAsync();
      HttpPacket packet = new HttpPacket(httpData);

      // client requests checks
      if (!packet.Method.ToLower().StartsWith("get"))
        return await RejectClient(stream, "Only allows GET Requests");
      if (!packet.Headers.ContainsKey("Sec-WebSocket-Key"))
        return await RejectClient(stream, "No WebSocket-Key found");
      if (!packet.Headers.ContainsKey("Upgrade"))
        return await RejectClient(stream, "Upgrade header not set");
      else if (!packet.Headers["Upgrade"].ToLower().StartsWith("websocket"))
        return await RejectClient(stream, "Upgrade header not websocket");
      if (!packet.Headers.ContainsKey("Connection"))
        return await RejectClient(stream, "Connection header not set");
      else if (!packet.Headers["Connection"].ToLower().StartsWith("upgrade"))
        return await RejectClient(stream, "Connection header not upgrade");
      
      // generate websocket key, send response to client and complete handshake
      httpData = Encoding.UTF8.GetBytes(HttpHandshakeSucces.Replace(
        "{WebsockKey}", GenerateKey(packet.Headers["Sec-WebSocket-Key"])));
      await stream.WriteAsync(httpData, httpData.Length);
      return true;
    }

    // start server asynchronously
    public async Task StartAsync(CancellationToken? _token = null) {

      // create task cancellation token and start tcp server
      tokenSource = CancellationTokenSource.CreateLinkedTokenSource(
        _token ?? new CancellationToken());
      CancellationToken token = tokenSource.Token;
      server.Start();
      Console.WriteLine("Started");

      // start server accept event loop
      try {
        while (!token.IsCancellationRequested) {
          await Task.Run(async () => {

            // accept an incoming client
            System.Net.Sockets.TcpClient client = await server.AcceptTcpClientAsync();
            client.NoDelay = true;

            // get stream from client even if server is SSL
            await HandleClient(new ConnectionStream(ref client, isSsl));
          }, token);
        }

      // close resources on exit
      } finally {
        server.Stop();
        running = false;
      }
    }
  }
}