using System;
using System.Text;
using System.Net.Sockets;
using System.Net.Security;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Wophi {
  
  public enum WebsockState {
    Open, Connecting, Closed
  };

  public class WebsockClient {
    private bool isConnected;
    private System.Net.Sockets.TcpClient client;
    private WebsockState state;
    private WebsockProtocol protocol;

    public event Func<byte[], Task> OnMessage;
    public event Func<ushort, string, Task> OnClose;

    private static readonly string HttpHandshakeData =
      "GET {Path} HTTP/1.1\r\n" + 
      "Host: {Host}:{Port}\r\n" +
      "Upgrade: websocket\r\n" + 
      "Connection: upgrade\r\n" +
      "Origin: {Scheme}://{Host}\r\n" + 
      "Sec-WebSocket-Version: 13\r\n" +
      "Sec-WebSocket-Key: {Key}\r\n\r\n"; 

    public WebsockClient() {
      isConnected = false;
      client = new System.Net.Sockets.TcpClient();
      state = WebsockState.Closed;
    }

    public WebsockState GetState() {
      return state;
    }

    public void Ping(string data, Func<byte[], Task> callback) {
      PingAsync(data, callback).GetAwaiter().GetResult();
    }

    public async Task PingAsync(string data, Func<byte[], Task> callback) {
      await protocol.PingAsync(data, callback);
    }

    private string GenerateKey() {
      byte[] key = new byte[16];
      WebsockFrame.Rand.NextBytes(key);
      return System.Convert.ToBase64String(key);
    }

    private async Task<bool> Handshake(ConnectionStream stream, Uri uri, int port) {
      state = WebsockState.Connecting;
      string handshakeData = HttpHandshakeData
        .Replace("{Path}", uri.PathAndQuery)
        .Replace("{Host}", uri.Host)
        .Replace("{Scheme}", uri.Scheme)
        .Replace("{Port}", port.ToString())
        .Replace("{Key}", GenerateKey());
      await stream.WriteAsync(handshakeData);
      string response = Encoding.UTF8.GetString(await stream.ReadAsync());
      return response.Split('\n')[0].Split(' ')[1] == "101";
    }

    public async Task ConnectAsync(string url) {
      Uri uri = new Uri(url);
      string query = uri.PathAndQuery;
      bool ssl = uri.Scheme.ToLower().StartsWith("wss");
      int port = uri.Port > 0 ? uri.Port : (ssl ? 443 : 80);

      await client.ConnectAsync(uri.Host, port);
      ConnectionStream stream = new ConnectionStream(ref client, ssl);
      if (ssl) await stream.Authenticate(ConnectionStream.AuthMode.Client, uri.Host);

      if (!(await Handshake(stream, uri, port))) {
        state = WebsockState.Closed;
        throw new Exception("Failed connection at handshake");
      }

      protocol = new WebsockProtocol(ref stream);
      state = WebsockState.Open;
      try {
        while (true)
          await OnMessage(await protocol.GetMessage());
      } catch (WebsockCloseException ex) {
        await OnClose(ex.Code, ex.Reason);
      } finally {
        protocol.Dispose();
      }
    }
  }

}