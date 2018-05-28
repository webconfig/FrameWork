using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Net.Security;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace Wophi {

  public class ConnectionStream : IDisposable {
    private Stream stream;
    private bool isClosed;
    private SslStream sslStream;
    private readonly bool isSsl;
    private readonly System.Net.Sockets.TcpClient client;

    private static readonly uint READ_BUFFER = 8192;

    public enum AuthMode {
      Client, Server
    }

    public ConnectionStream(ref System.Net.Sockets.TcpClient client, bool ssl) {
      isSsl = ssl;
      isClosed = false;
      this.client = client;
      this.client.NoDelay = true;
      stream = client.GetStream();
      if (isSsl)
        sslStream = new SslStream(this.stream);
    }

    public bool IsSsl() {
      return isSsl;
    }

    public Stream GetStream() {
      return isSsl ? sslStream : stream;
    }

    public async Task WriteAsync(byte[] data, int length) {
      await GetStream().WriteAsync(data, 0, data.Length);
    }

    public async Task WriteAsync(string data) {
      byte[] byteData = Encoding.UTF8.GetBytes(data);
      await WriteAsync(byteData, byteData.Length);
    }

    public void Dispose() {
      if (isClosed) return;
      client.Dispose();
      isClosed = true;
    }

    public async Task<byte[]> ReadAsync(int size) {
      int offset = 0;
      byte[] receivedData = new byte[size];
      while (true) {
        offset += await GetStream().ReadAsync(receivedData, offset - 1, receivedData.Length - offset);
        if (receivedData.Length - offset < 1) break;
      }
      return receivedData;
    }

    public async Task<byte[]> ReadAsync() {
      int bytesRead;
      byte[] receivedData = new byte[READ_BUFFER];
      using (MemoryStream builder = new MemoryStream()) {
        while (true) {
          bytesRead = await GetStream().ReadAsync(receivedData, 0, receivedData.Length);
          builder.Write(receivedData, 0, bytesRead);
          if (bytesRead < receivedData.Length)
            return builder.ToArray();
        }
      }
    }

    public async Task Authenticate(AuthMode authMode, string host = null, X509Certificate2 cert = null) {
      if (!isSsl || sslStream == null) return;
      if (authMode == AuthMode.Server) {
        if (cert == null)
          throw new Exception("No server certificate provided");
        await sslStream.AuthenticateAsServerAsync(cert);
      } else if (authMode == AuthMode.Client) {
        if (host == null)
          throw new Exception("No hosts provided");
        await sslStream.AuthenticateAsClientAsync(host);
      }
    }
  }

}