using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Wophi {

  public class HttpPacket {
    private enum ParseState {
      Method, Headers, End
    }

    private static readonly string[] HeaderSep = new string[]{ ": " };

    public string Path;
    public string Method;
    public string QueryString;
    private ParseState parseState;
    public Dictionary<string, string> Headers;
    public Dictionary<string, string> QueryParams;

    public HttpPacket(byte[] data) {
      parseState = ParseState.Method;
      Headers = new Dictionary<string, string>();
      QueryParams = new Dictionary<string, string>();
      Parse(data);
    }

    public void Parse(byte[] data) {
      using (StringReader reader = new StringReader(Encoding.UTF8.GetString(data))) {
        string line;
        while (true) {
          line = reader.ReadLine();
          if (line == null) break;
          if (parseState == ParseState.Method) {
            string[] parts = line.Split(' ');
            Method = parts[0];
            Path = parts[1].Split('?')[0];
            QueryString = Path.Contains("?") ? Path.Substring(Path.IndexOf("?") + 1) : "";
            if (QueryString.Length > 1) {
              foreach (string param in QueryString.Substring(1).Split('&')) {
                parts = param.Split('=');
                QueryParams.Add(parts[0], parts[1]);
              }
            }
            parseState = ParseState.Headers;
          } else if (parseState == ParseState.Headers) {
            if (line.Length > 1) {
              string[] parts = line.Split(HeaderSep, StringSplitOptions.None);
              Headers.Add(parts[0], parts[1]);
            } else {
              parseState = ParseState.End;
            }
          } else if (parseState == ParseState.End) {
            return;
          }
        }
      }
    }
  }
}