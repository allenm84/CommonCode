using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Common.References
{
  public class VLC : IDisposable
  {
    // http://www.videolan.org/developers/vlc/doc/doxygen/html/group__libvlc.html

    static class LibVlc
    {
      const string LibVlcPath = @"C:\Program Files (x86)\VideoLAN\VLC\libvlc.dll";

      #region core
      [DllImport(LibVlcPath, CallingConvention = CallingConvention.Cdecl)]
      public static extern IntPtr libvlc_new(int argc, string[] argv);

      [DllImport(LibVlcPath, CallingConvention = CallingConvention.Cdecl)]
      public static extern void libvlc_release(IntPtr p_instance);
      #endregion

      #region media
      [DllImport(LibVlcPath, CallingConvention = CallingConvention.Cdecl)]
      public static extern IntPtr libvlc_media_new_location(IntPtr p_instance, [MarshalAs(UnmanagedType.LPStr)] string psz_mrl);

      [DllImport(LibVlcPath, CallingConvention = CallingConvention.Cdecl)]
      public static extern IntPtr libvlc_media_new_path(IntPtr p_instance, [MarshalAs(UnmanagedType.LPStr)] string path);

      [DllImport(LibVlcPath, CallingConvention = CallingConvention.Cdecl)]
      public static extern void libvlc_media_release(IntPtr media);
      #endregion

      #region media player

      [DllImport(LibVlcPath, CallingConvention = CallingConvention.Cdecl)]
      public static extern void libvlc_media_player_set_media(IntPtr mediaPlayer, IntPtr media);

      [DllImport(LibVlcPath, CallingConvention = CallingConvention.Cdecl)]
      public static extern IntPtr libvlc_media_player_new(IntPtr p_instance);

      [DllImport(LibVlcPath, CallingConvention = CallingConvention.Cdecl)]
      public static extern IntPtr libvlc_media_player_new_from_media(IntPtr media);

      [DllImport(LibVlcPath, CallingConvention = CallingConvention.Cdecl)]
      public static extern void libvlc_media_player_release(IntPtr mediaPlayer);

      [DllImport(LibVlcPath, CallingConvention = CallingConvention.Cdecl)]
      public static extern void libvlc_media_player_set_hwnd(IntPtr mediaPlayer, IntPtr hWnd);

      [DllImport(LibVlcPath, CallingConvention = CallingConvention.Cdecl)]
      public static extern int libvlc_media_player_play(IntPtr mediaPlayer);

      [DllImport(LibVlcPath, CallingConvention = CallingConvention.Cdecl)]
      public static extern void libvlc_media_player_pause(IntPtr mediaPlayer);

      [DllImport(LibVlcPath, CallingConvention = CallingConvention.Cdecl)]
      public static extern void libvlc_media_player_stop(IntPtr mediaPlayer);
      #endregion
    }

    static Lazy<VLC> singletonInstance = new Lazy<VLC>(() => new VLC(), true);
    public static VLC Instance { get { return singletonInstance.Value; } }

    private IntPtr player;
    private IntPtr instance;

    private bool playing = false;
    private bool disposed = false;

    public bool Playing { get { return playing; } }

    private VLC()
    {
      instance = LibVlc.libvlc_new(0, null);
    }

    ~VLC()
    {
      // Do not re-create Dispose clean-up code here.
      // Calling Dispose(false) is optimal in terms of
      // readability and maintainability.
      Dispose(false);
    }

    public void Dispose()
    {
      Dispose(true);
      // Take yourself off the Finalization queue 
      // to prevent finalization code for this object
      // from executing a second time.
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      // Check to see if Dispose has already been called.
      if (!this.disposed)
      {
        // If disposing equals true, dispose all managed 
        // and unmanaged resources.
        if (disposing)
        {
          // Dispose managed resources.
        }

        // Release unmanaged resources. If disposing is false, 
        // only the following code is executed.
        if (playing)
          Stop();
        LibVlc.libvlc_release(instance);
        instance = IntPtr.Zero;

        // Note that this is not thread safe.
        // Another thread could start disposing the object
        // after the managed resources are disposed,
        // but before the disposed flag is set to true.
        // If thread safety is necessary, it must be
        // implemented by the client.

      }
      disposed = true;
    }

    public void Play(string mediaPath, IntPtr hWnd)
    {
      if (!playing)
      {
        IntPtr media = LibVlc.libvlc_media_new_path(instance, mediaPath);
        player = LibVlc.libvlc_media_player_new_from_media(media);

        LibVlc.libvlc_media_release(media);
        LibVlc.libvlc_media_player_set_hwnd(player, hWnd);
        LibVlc.libvlc_media_player_play(player);

        playing = true;
      }
      else
      {
        throw new InvalidOperationException("VLC is currently playing media");
      }
    }

    public void Stop()
    {
      if (playing)
      {
        LibVlc.libvlc_media_player_stop(player);
        LibVlc.libvlc_media_player_release(player);
        playing = false;
        player = IntPtr.Zero;
      }
      else
      {
        throw new InvalidOperationException("VLC is not currently playing media");
      }
    }
  }
}
