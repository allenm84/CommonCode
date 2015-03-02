using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace System.Common.Extensions
{
  public static partial class IOExtensions
  {
    /// <summary>
    /// 
    /// </summary>
    public const string ImageFilter = "Image files |*.bmp;*.jpg;*.jpeg;*.png;|Bitmap files (*.bmp)|*.bmp;|JPEG files (*.jpg, *.jpeg)|*.jpg;*.jpeg;|PNG files (*.png)|*.png";

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetFileInformationByHandle(SafeFileHandle hFile, out BY_HANDLE_FILE_INFORMATION lpFileInformation);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="subdirectoryName"></param>
    /// <returns></returns>
    public static DirectoryInfo GetSubdirectory(this DirectoryInfo parent, string subdirectoryName)
    {
      return new DirectoryInfo(Path.Combine(parent.FullName, subdirectoryName));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool IsSameAs(this FileInfo value, FileInfo other)
    {
      var p1 = Path.GetFullPath(value.FullName);
      var p2 = Path.GetFullPath(other.FullName);
      if (string.Equals(p1, p2, StringComparison.InvariantCultureIgnoreCase))
      {
        return true;
      }

      var f1 = value.OpenRead();
      var f2 = other.OpenRead();

      BY_HANDLE_FILE_INFORMATION i1, i2;
      if (
        GetFileInformationByHandle(f1.SafeFileHandle, out i1) &&
          GetFileInformationByHandle(f2.SafeFileHandle, out i2))
      {
        // dwVolumeSerialNumber, nFileIndexHigh and nFileIndexLow
        f1.Close();
        f1.Dispose();
        f2.Close();
        f2.Dispose();

        return
          (i1.VolumeSerialNumber == i2.VolumeSerialNumber) &&
            (i1.FileIndexHigh == i2.FileIndexHigh) &&
            (i1.FileIndexLow == i2.FileIndexLow);
      }
      f1.Close();
      f1.Dispose();
      f2.Close();
      f2.Dispose();

      return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="info"></param>
    public static void SetToNormal(this FileSystemInfo info)
    {
      info.Attributes = FileAttributes.Normal;
      if (info is DirectoryInfo)
      {
        var dir = info as DirectoryInfo;
        var infos = dir.GetFileSystemInfos();
        for (var i = 0; i < infos.Length; ++i)
        {
          SetToNormal(infos[i]);
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static MemoryStream ToStream(this byte[] bytes)
    {
      return new MemoryStream(bytes);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="file"></param>
    /// <param name="name"></param>
    public static void Rename(this FileInfo file, string name)
    {
      var directory = file.Directory.FullName;
      var filepath = string.Concat(directory, "\\", name, file.Extension);
      file.MoveTo(filepath);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="directory"></param>
    public static void SetToNormal(this DirectoryInfo directory)
    {
      directory.Attributes = FileAttributes.Normal;
      foreach (var info in directory.EnumerateFileSystemInfos("*.*", SearchOption.AllDirectories))
      {
        info.Attributes = FileAttributes.Normal;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    public static void CopyTo(this DirectoryInfo source, DirectoryInfo target)
    {
      if (!target.Exists)
      {
        target.Create();
      }

      foreach (var file in source.EnumerateFiles())
      {
        var destination = Path.Combine(target.FullName, file.Name);
        file.CopyTo(destination, true);
      }

      foreach (var directory in source.EnumerateDirectories())
      {
        CopyTo(directory, target.CreateSubdirectory(directory.Name));
      }
    }

    #region Nested type: BY_HANDLE_FILE_INFORMATION

    [StructLayout(LayoutKind.Sequential)]
    private struct BY_HANDLE_FILE_INFORMATION
    {
      public uint FileAttributes;
      public FILETIME CreationTime;
      public FILETIME LastAccessTime;
      public FILETIME LastWriteTime;
      public uint VolumeSerialNumber;
      public uint FileSizeHigh;
      public uint FileSizeLow;
      public uint NumberOfLinks;
      public uint FileIndexHigh;
      public uint FileIndexLow;
    }

    #endregion
  }
}