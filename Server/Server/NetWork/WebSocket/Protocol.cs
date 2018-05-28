using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Wophi {

  public class WebsockCloseException : Exception {
    public ushort Code;
    public string Reason;
    public WebsockCloseException(byte[] data) {
      Code  = (ushort)(((data[0] & 0xff) << 8) | ((data[1] & 0xff) << 0));
      byte[] reason = new byte[data.Length - 2];
      for (int i = 2; i < data.Length; i++)
        reason[i - 2] = data[i];
      Reason = Encoding.UTF8.GetString(reason);
    }
  }

  public class WebsockProtocol : IDisposable {
    private MemoryStream dataBuilder;
    private Queue<Func<byte[], Task>> callbacks;
    private readonly ConnectionStream stream;

    public WebsockProtocol(ref ConnectionStream stream) {
      this.stream = stream;
      dataBuilder = new MemoryStream();
      callbacks = new Queue<Func<byte[], Task>>();
    }

    public void Dispose() {
      stream.Dispose();
    }

    public void Close(ushort code, string reason) {
      CloseAsync(code, reason).GetAwaiter().GetResult();
    }

    public void Send(byte[] data, WebsockOpCode opcode = WebsockOpCode.Text) {
      SendAsync(data, opcode).GetAwaiter().GetResult();
    }

    public void Ping(string data, Func<byte[], Task> callback) {
      PingAsync(data, callback).GetAwaiter().GetResult();
    }

    public async Task SendAsync(string data, WebsockOpCode opcode = WebsockOpCode.Text) {
      byte[] byteData = Encoding.UTF8.GetBytes(data);
      await SendAsync(byteData, WebsockOpCode.Text);
    }

    public async Task PingAsync(string data, Func<byte[], Task> callback) {
      callbacks.Enqueue(callback);
      await SendAsync(data, WebsockOpCode.Ping);
    }

    public async Task CloseAsync(ushort code, string reason) {
      byte[] closeCode = BitConverter.GetBytes(code);
      Array.Reverse(closeCode);
      byte[] closeFrame = new byte[closeCode.Length + reason.Length];
      closeCode.CopyTo(closeFrame, 0);
      Encoding.UTF8.GetBytes(reason).CopyTo(closeFrame, closeCode.Length);
      await SendAsync(closeFrame, WebsockOpCode.Close);
    }

    public async Task SendAsync(byte[] data, WebsockOpCode opcode = WebsockOpCode.Text) {
      WebsockFrame frame = new WebsockFrame();
      frame.Fin = true;
      frame.Data = data;
      frame.Masked = true;
      frame.Opcode = opcode;
      byte[] frameData = frame.Dump();
      await stream.WriteAsync(frameData, frameData.Length);
    }

    public async Task<byte[]> GetMessage() {
      int size = 0;
      byte[] buffer = new byte[2];
      WebsockFrame frame = new WebsockFrame();

      // read first byte of data
      buffer = await stream.ReadAsync(buffer.Length);
      frame.Fin  = (buffer[0] & 0x80) != 0;             // first bit
      frame.Rsv1 = (buffer[0] & 0x40) != 0;             // second bit
      frame.Rsv2 = (buffer[0] & 0x20) != 0;             // thrid bit
      frame.Rsv3 = (buffer[0] & 0x10) != 0;             // fourth bit
      frame.Opcode = (WebsockOpCode)(buffer[0] & 0x0f); // fifth to eigth bits

      // read second byte of data
      frame.Masked = (buffer[1] & 0x80) != 0; // set second byte first bit
      size = (int)(buffer[1] & (~0x80));      // get deciding size

      // payload size is 64 bits long
      if (size == 127) {
        size = 0;
        buffer = new byte[8];
        buffer = await stream.ReadAsync(buffer.Length);
        size |= (buffer[0] & 0xff) << 56;
        size |= (buffer[1] & 0xff) << 48;
        size |= (buffer[2] & 0xff) << 40;
        size |= (buffer[3] & 0xff) << 32;
        size |= (buffer[4] & 0xff) << 24;
        size |= (buffer[5] & 0xff) << 16;
        size |= (buffer[6] & 0xff) << 8;
        size |= (buffer[7] & 0xff) << 0;

      // payload size is 16 bits long
      } else if (size == 126) {
        size = 0;
        buffer = new byte[2];
        buffer = await stream.ReadAsync(buffer.Length);
        size |= (buffer[0] & 0xff) << 8;
        size |= (buffer[1] & 0xff) << 0;          
      } 
      
      // read mask data
      if (frame.Masked) {
        frame.MaskingKey = new byte[4];
        frame.MaskingKey = await stream.ReadAsync(frame.MaskingKey.Length);
      }

      // read payload data
      frame.Data = new byte[size];
      frame.Data = await stream.ReadAsync(size);
      if (frame.Masked)
        for (int i = 0; i < size; i++)
          frame.Data[i] ^= frame.MaskingKey[i % 4];

      // combine with continued data
      if (dataBuilder.Length > 0) {
        byte[] continued = dataBuilder.ToArray();
        dataBuilder.SetLength(0);
        byte[] combined = new byte[continued.Length + frame.Data.Length];
        Buffer.BlockCopy(continued, 0, combined, 0, continued.Length);
        Buffer.BlockCopy(frame.Data, 0, continued, continued.Length, frame.Data.Length);
        frame.Data = combined;
      }  

      // build up fragmented frame & try again
      if (frame.Opcode == WebsockOpCode.Continue) {
        dataBuilder.Write(frame.Data, 0, frame.Data.Length);
        return await GetMessage();

      // echo back close data and close connection
      } else if (frame.Opcode == WebsockOpCode.Close) {
        await SendAsync(frame.Data, WebsockOpCode.Close);
        throw new WebsockCloseException(frame.Data);

      // perform ping callback that was queued up & try again
      } else if (frame.Opcode == WebsockOpCode.Pong) {
        await callbacks.Dequeue()(frame.Data);
        return await GetMessage();

      // frame is either Text or Binary, return data
      } else {
        return frame.Data;
      }
    }
  }

}