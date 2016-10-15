using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataSt;

namespace MSDNSoket
{
  class Program
  {
  static  Data _data = new Data()
    {
      direction = 124, 
      typeTrain = 59, 
      currentSpeed = 10, 
      nameRailRoad = "Северокавказская Жд", 
      modelTime = 95, 
      coordinate = 158.3
    };


    static void Main(string[] args)
    {

   
      Socket  _s = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
      IPEndPoint ipe = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);
      _s.Connect(ipe);
      var msg = Data.Serialize(_data);
      
      foreach (byte b in msg)
        Console.Write(b.ToString() + " ");


      while (true)
      {
        Thread.Sleep(1000);

        try
        {
          _s.Send(msg);
        }
        catch (Exception)
        {
         break;
        }
         

      }

      _s.Shutdown(SocketShutdown.Both);
      _s.Close();
      Console.ReadLine();
    }
  }
}
