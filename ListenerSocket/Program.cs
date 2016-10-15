using DataSt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ListenerSocket
{
  class Program
  {
    static void Main()
    {
      int countBytes = 282;
      //byte[] _input = new byte[countBytes];

      var memStream = new MemoryStream(new byte[countBytes]);

      Socket _s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      IPEndPoint ipe = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);
      _s.Bind(ipe);
      _s.Listen(100);
     


        Console.WriteLine("Waiting connection");
        Socket handler = _s.Accept();
      while(true)
      {
        Console.WriteLine("Connected");
        while (memStream.Position < countBytes)
        {
          byte[] bufBytes = new byte[285];
          int reciv = handler.Receive(bufBytes);
          memStream.Write(bufBytes, 0, reciv);
        }
        var a = Data.Deserialize(memStream.ToArray());
        Console.WriteLine(a);
        memStream.Position = 0;
        
      }
      Console.ReadLine();
    }
  }
}
