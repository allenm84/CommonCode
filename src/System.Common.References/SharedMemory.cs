using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Common.References
{
  /// <summary>
  /// Shared memory can be accessed by any process on the local machine 
  /// by using the same mapping name when creating the SharedMemory object.
  /// 
  /// As long as the SharedMemory instance is referenced, the SharedMemory will
  /// be available to other processes.
  /// </summary>
  public class SharedMemory : IDisposable
  {
    #region P/Invoke Defines

    private const int STANDARD_RIGHTS_REQUIRED = 0x000F0000;
    private const int SECTION_QUERY = 0x0001;
    private const int SECTION_MAP_WRITE = 0x0002;
    private const int SECTION_MAP_READ = 0x0004;
    private const int SECTION_MAP_EXECUTE = 0x0008;
    private const int SECTION_EXTEND_SIZE = 0x0010;

    private const int SECTION_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | SECTION_QUERY |
      SECTION_MAP_WRITE |
      SECTION_MAP_READ |
      SECTION_MAP_EXECUTE |
      SECTION_EXTEND_SIZE);

    private const int FILE_MAP_READ = SECTION_MAP_READ;
    private const int FILE_MAP_WRITE = SECTION_MAP_WRITE;
    private const int FILE_MAP_ALL_ACCESS = SECTION_ALL_ACCESS;

    private const int PAGE_READONLY = 0x02;
    private const int PAGE_READWRITE = 0x04;

    [DllImport("kernel32.dll", EntryPoint = "CreateFileMapping", SetLastError = true)]
    private static extern IntPtr CreateFileMapping(
      int hFile,
      int lpAttributes,
      int fsProtect,
      int maxSizeHigh,
      int maxSizeLow,
      string name);

    [DllImport("kernel32.dll", EntryPoint = "MapViewOfFile")]
    private static extern IntPtr MapViewOfFile(
      IntPtr hFileMappingObject,
      int dwDesiredAccess,
      int dwFileOffsetHigh,
      int dwFileOffsetLow,
      int dwNumberOfBytesToMap);

    [DllImport("kernel32.dll", EntryPoint = "UnmapViewOfFile")]
    private static extern bool UnmapViewOfFile(IntPtr pBuf);

    [DllImport("kernel32.dll", EntryPoint = "CloseHandle")]
    private static extern bool CloseHandle(IntPtr hObject);

    #endregion

    /// <summary>The handle to the shared memory file mapping</summary>
    private readonly IntPtr mMapPtr;

    /// <summary>The size of the shared memory buffer</summary>
    private readonly int mBufferLen;

    /// <summary>
    /// Public constructor for the SharedMemory class
    /// </summary>
    /// <param name="name">
    /// The name of the shared memory location to create.  Names starting with
    /// "Global\" should not be used as additional permissions are required to
    /// created such names.  Also, backslashes are generally not allowed, with 
    /// the exception that the name can start with Local\ to indicate that it
    /// cannot be connected to remotely.
    /// </param>
    /// <param name="len"></param>
    /// <param name="writeable"></param>
    /// <exception cref="NotSupportedException">Thrown if unable to 
    /// create the file mapping</exception>
    public SharedMemory(string name, int len, bool writeable)
    {
      mBufferLen = len;

      mMapPtr = CreateFileMapping(
        -1,
        // -1 indicates a page file backed mapping
        0,
        // security
        writeable ? PAGE_READWRITE : PAGE_READONLY,
        0,
        len,
        name);

      if (mMapPtr == IntPtr.Zero)
      {
        throw new NotSupportedException("Unable to create file mapping");
      }
    }

    /// <summary>
    /// Finalizer for the SharedMemory object
    /// </summary>
    ~SharedMemory()
    {
      Dispose();
    }

    /// <summary>
    /// Dispose of the shared memory.  If this is the last reference to the 
    /// shared memory mapping specified by the name parameter in the constructor,
    /// the shared memory will be removed from the system.
    /// </summary>
    public void Dispose()
    {
      CloseHandle(mMapPtr);
      System.GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Reads data from the shared memory map
    /// </summary>
    /// <param name="data"></param>
    /// <param name="len"></param>
    public void Read(byte[] data)
    {
      if (data.Length > mBufferLen)
        throw new IndexOutOfRangeException("Cannot read outside of the memory buffer");

      IntPtr mapView = CreateMapView(false);

      try
      {
        Marshal.Copy(mapView, data, 0, data.Length);
      }
      finally
      {
        UnmapViewOfFile(mapView);
      }
    }

    /// <summary>
    /// Write data to the shared memory buffer
    /// </summary>
    /// <param name="data"></param>
    /// <param name="len"></param>
    public void Write(byte[] data)
    {
      if (data.Length > mBufferLen)
        throw new IndexOutOfRangeException("Cannot read outside of the memory buffer");

      IntPtr mapView = CreateMapView(true);

      try
      {
        Marshal.Copy(data, 0, mapView, data.Length);
      }
      finally
      {
        UnmapViewOfFile(mapView);
      }
    }

    /// <summary>
    /// Allocates a map view of the file so that the data can be read from
    /// or written to
    /// </summary>
    /// <returns></returns>
    private IntPtr CreateMapView(bool writeable)
    {
      IntPtr buffer = MapViewOfFile(mMapPtr,
        // handle to map object
        writeable ? FILE_MAP_WRITE : FILE_MAP_READ,
        // read/write permission
        0,
        0,
        mBufferLen);

      if (buffer == IntPtr.Zero)
      {
        throw new NotSupportedException("Unable to create file map view");
      }

      return buffer;
    }
  }
}
