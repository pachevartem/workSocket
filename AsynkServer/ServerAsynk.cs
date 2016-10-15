using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using DataSt;

// State object for reading client data asynchronously
public class StateObject
{
  public StateObject(Socket _s)
  {
    workSocket = _s;
  }
  public Socket workSocket = null;
  public const int BufferSize = 282;
  public byte[] buffer = new byte[BufferSize];
 // public MemoryStream ms = new MemoryStream(new byte[BufferSize]);
}

public class AsynchronousSocketListener
{
  public static ManualResetEvent allDone = new ManualResetEvent(false);
  
  public static void StartListening()
  {
    IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"),11000);
    Socket listener = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);

    try
    {
      listener.Bind(localEndPoint);
      listener.Listen(100);

      while(true)
      {
        // Set the event to nonsignaled state.
        allDone.Reset();

        // Start an asynchronous socket to listen for connections.
        Console.WriteLine("Waiting for a connection...");
        listener.BeginAccept( new AsyncCallback(AcceptCallback), listener);
        // Wait until a connection is made before continuing.
        allDone.WaitOne();
      }

    }
    catch(Exception e)
    {
      Console.WriteLine(e.ToString());
    }

    Console.WriteLine("\nPress ENTER to continue...");
    Console.Read();

  }

  public static void AcceptCallback(IAsyncResult ar)
  {
    // Signal the main thread to continue.
    allDone.Set();

    // Get the socket that handles the client request.
    Socket listener = (Socket)ar.AsyncState;
    Socket handler = listener.EndAccept(ar);

    // Create the state object.
    StateObject state = new StateObject(handler);
    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
        new AsyncCallback(ReadCallback), state);
     
  }

  public static void ReadCallback(IAsyncResult ar)
  {
    Data _data;

    // Retrieve the state object and the handler socket
    // from the asynchronous state object.
    StateObject state = (StateObject)ar.AsyncState;
    Socket handler = state.workSocket;

    // Read data from the client socket. 
    int bytesRead = handler.EndReceive(ar);
    

    if(bytesRead > 0)
    {

     _data = Data.Deserialize(state.buffer);
      Console.WriteLine(_data);
    }
    // Not all data received. Get more.
    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
    new AsyncCallback(ReadCallback), state);
  }

  private static void Send(Socket handler, String data)
  {
    // Convert the string data to byte data using ASCII encoding.
    byte[] byteData = Encoding.ASCII.GetBytes(data);

    // Begin sending the data to the remote device.
    handler.BeginSend(byteData, 0, byteData.Length, 0,
        new AsyncCallback(SendCallback), handler);
  }

  private static void SendCallback(IAsyncResult ar)
  {
    try
    {
      // Retrieve the socket from the state object.
      Socket handler = (Socket)ar.AsyncState;

      // Complete sending the data to the remote device.
      int bytesSent = handler.EndSend(ar);
      Console.WriteLine("Sent {0} bytes to client.", bytesSent);

      handler.Shutdown(SocketShutdown.Both);
      handler.Close();

    }
    catch(Exception e)
    {
      Console.WriteLine(e.ToString());
    }
  }


  public static int Main(String[] args)
  {
    StartListening();
    return 0;
  }
}