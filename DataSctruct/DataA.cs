using System;
using System.Runtime.InteropServices;

namespace DataSt
{

  [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 282)]
  public struct Data
  {

    [MarshalAs(UnmanagedType.U1)]
    public byte typeTrain;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public string nameRailRoad;

    [MarshalAs(UnmanagedType.R8)]
    public double modelTime;

    [MarshalAs(UnmanagedType.R8)]
    public double coordinate;

    [MarshalAs(UnmanagedType.R8)]
    public double currentSpeed;

    [MarshalAs(UnmanagedType.I1)]
    public sbyte direction;


    public static byte[] Serialize(Data data)
    {
      int rawSize = Marshal.SizeOf(typeof(Data));
      IntPtr buffer = Marshal.AllocHGlobal(rawSize);
      Marshal.StructureToPtr(data, buffer, false);
      byte[] rawData = new byte[rawSize];
      Marshal.Copy(buffer, rawData, 0, rawSize);
      Marshal.FreeHGlobal(buffer); //Освобождает память, выделенную ранее из неуправляемой памяти процесса.
      return rawData;
    }

    public static Data Deserialize(byte[] rawData)
    {
      int rawsize = Marshal.SizeOf(typeof(Data));
      if(rawsize > rawData.Length)
        return default(Data);

      Data obj = default(Data);
      IntPtr buffer = Marshal.AllocHGlobal(rawsize);

      try
      {
        Marshal.Copy(rawData, 0, buffer, rawsize);
        obj = (Data)Marshal.PtrToStructure(buffer, typeof(Data));
      }
      catch(Exception ex)
      {
        var errstring = ex.ToString();
      }
      finally
      {
        Marshal.FreeHGlobal(buffer); //Освобождает память, выделенную ранее из неуправляемой памяти процесса.
      }
      return obj;
    }

    public override string ToString()
    {
      return string.Format("[Data: typeTrain={0}, nameRailRoad={1}, modelTime={2}, coordinate={3}, currentSpeed={4}, direction={5}]", typeTrain, nameRailRoad, modelTime, coordinate, currentSpeed, direction);
    }
  }

  class Program
  {
    static void Main(string[] args)
    {
    }
  }
}