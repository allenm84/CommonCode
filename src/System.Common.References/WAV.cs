using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Common.References
{
  internal struct ChunkHeader
  {
    public string dId;
    public int dLen;

    internal void WriteToStream(BinaryWriter fw)
    {
      fw.Write(dId.ToCharArray(), 0, 4);
      fw.Write(dLen);
    }

    internal void ReadFromStream(BinaryReader fw)
    {
      dId = new string(fw.ReadChars(4));
      dLen = fw.ReadInt32();
    }
  }

  internal struct WaveHeader
  {
    public string rID;
    public int rLen;
    public string wID;
    public string fId;
    public int pcm_header_len;
    public short wFormatTag;
    public short nChannels;
    public int nSamplesPerSec;
    public int nAvgBytesPerSec;
    public short nBlockAlign;
    public short nBitsPerSample;

    internal void WriteToStream(BinaryWriter fw)
    {
      fw.Write(rID.ToCharArray(), 0, 4);
      fw.Write(rLen);
      fw.Write(wID.ToCharArray(), 0, 4);
      fw.Write(fId.ToCharArray(), 0, 4);
      fw.Write(pcm_header_len);
      fw.Write(wFormatTag);
      fw.Write(nChannels);
      fw.Write(nSamplesPerSec);
      fw.Write(nAvgBytesPerSec);
      fw.Write(nBlockAlign);
      fw.Write(nBitsPerSample);
    }

    internal void ReadFromStream(BinaryReader fw)
    {
      rID = new string(fw.ReadChars(4));
      rLen = fw.ReadInt32();
      wID = new string(fw.ReadChars(4));
      fId = new string(fw.ReadChars(4));
      pcm_header_len = fw.ReadInt32();
      wFormatTag = fw.ReadInt16();
      nChannels = fw.ReadInt16();
      nSamplesPerSec = fw.ReadInt32();
      nAvgBytesPerSec = fw.ReadInt32();
      nBlockAlign = fw.ReadInt16();
      nBitsPerSample = fw.ReadInt16();
    }
  }

  public class WaveReader
  {
    internal double fs_hz;
    internal int bits_per_sample;
    internal int num_ch;
    internal double[] g_wdata_in;
    internal int g_num_isamp;
    internal long g_max_isamp;

    public WaveReader(string file_name)
    {
      int i;
      BinaryReader fw;
      WaveHeader wav;
      ChunkHeader chk;
      int sflag;
      int rmore;
      char[] wbuff;
      int wbuff_len;

      // set defaults
      g_wdata_in = null;
      g_num_isamp = 0;
      g_max_isamp = 0;

      // allocate wav header
      wav = new WaveHeader();
      chk = new ChunkHeader();

      // # open wav file
      fw = new BinaryReader(File.OpenRead(file_name));

      // # read riff/wav header
      wav.ReadFromStream(fw);

      // # check format of header

      //if (strcmp(obuff, "RIFF") != 0) exit_msg(-587, "bad RIFF format");
      if (!wav.rID.Equals("RIFF")) throw new Exception("bad RIFF format");

      //if (strcmp(obuff, "WAVE") != 0) exit_msg(-587, "bad WAVE format");
      if (!wav.wID.Equals("WAVE")) throw new Exception("bad WAVE format");

      //if (strcmp(obuff, "fmt") != 0) exit_msg(-587, "bad fmt format");
      if (!wav.fId.Equals("fmt ")) throw new Exception("bad fmt format");

      //if (wav->wFormatTag != 1) exit_msg(-587, "bad wav wFormatTag");
      if (wav.wFormatTag != 1) throw new Exception("bad wav wFormatTag");

      //if ((wav->nBitsPerSample != 16) && (wav->nBitsPerSample != 8)) exit_msg(-587, "bad wav stuff");
      if (wav.nBitsPerSample != 16 && wav.nBitsPerSample != 8) throw new Exception("bad wav nBitsPerSample");

      // skip over any remaining portion of wav header
      rmore = wav.pcm_header_len - (Marshal.SizeOf(typeof(WaveHeader)) - 20);
      fw.ReadBytes(rmore);

      // read chunks until a 'data' chunk is found
      sflag = 1;
      while (sflag != 0)
      {
        // check attempts
        if (sflag > 10)
          throw new Exception("too many chunks");

        // read chunk header
        chk.ReadFromStream(fw);

        // check chunk type
        if (chk.dId.Equals("data"))
          break;

        // skip over chunk
        fw.ReadBytes(chk.dLen);

        // increment the size flag
        sflag++;
      }

      /* find length of remaining data */
      wbuff_len = chk.dLen;

      // find number of samples
      g_max_isamp = chk.dLen;
      g_max_isamp /= wav.nBitsPerSample / 8;

      /* allocate new buffers */
      wbuff = new char[wbuff_len];

      //	if(g_wdata_in!=NULL) delete g_wdata_in;
      g_wdata_in = new double[g_max_isamp];

      /* read signal data */
      wbuff = fw.ReadChars(wbuff_len);

      // convert data
      if (wav.nBitsPerSample == 16)
      {
        for (i = 0; i < g_max_isamp; i++)
          g_wdata_in[i] = (double)((ushort)(wbuff[i]));
      }
      else
      {
        for (i = 0; i < g_max_isamp; i++)
          g_wdata_in[i] = (double)((byte)(wbuff[i]));
      }

      // save demographics
      fs_hz = (double)(wav.nSamplesPerSec);
      bits_per_sample = wav.nBitsPerSample;
      num_ch = wav.nChannels;

      // reset buffer stream index
      g_num_isamp = 0;

      // be polite - clean up
      fw.Close();
    }

    // routine for reading one sample from a (previously loaded) wave file
    //  returns current sample as a double 
    public double read_current_input()
    {
      if ((g_wdata_in == null) || (g_max_isamp <= 0) || (g_num_isamp < 0))
        throw new Exception("input file not ready (or not loaded)");

      if (g_num_isamp >= g_max_isamp)
        throw new Exception("attempt to read past end of input buffer");

      return (g_wdata_in[g_num_isamp++]);
    }

    // determines end-of-file condition, returns 1==true if more data ready
    public bool more_data_available()
    {
      return !(g_num_isamp >= g_max_isamp);
    }

    // returns number of samples in file
    public long get_num_samples()
    {
      return g_max_isamp;
    }

    // reports number of channels (1==mono, 2==stereo)
    public int get_num_channels()
    {
      return num_ch;
    }

    // reports the number of bits in each sample
    public int get_bits_per_sample()
    {
      return bits_per_sample;
    }

    // reports sample rate in Hz
    public double get_sample_rate_hz()
    {
      return fs_hz;
    }

    public int get_index()
    {
      return g_num_isamp;
    }
  }

  /// <summary>
  /// Represents a simple .wav writer. This class only stores one chunk of type data. This data contains
  /// all the samples that make up the .wav.
  /// </summary>
  public class WaveWriter
  {
    const double MaxUuu = (65536.0 / 2.0) - 1.0;
    const double MinUuu = -(65536.0 / 2.0);

    const double MaxCcc = 256.0;
    const double MinCcc = 0.0;

    internal double _sampleRate;
    internal int _bitsPerSample;
    internal int _numChannels;
    internal List<double> _waveData;

    public WaveWriter(WaveReader waveReader) :
      this(waveReader.fs_hz, waveReader.bits_per_sample, waveReader.num_ch)
    {

    }

    public WaveWriter(double sampleRate, int bitsPerSample, int numChannels)
    {
      _sampleRate = sampleRate;
      _bitsPerSample = bitsPerSample;
      _numChannels = numChannels;
      _waveData = new List<double>();
    }

    // routine for writing one output sample
    //  samples are stored in a buffer, until save_wave_file() is called
    //  returns 0 on success 
    public int AddOutputSample(double sample)
    {
      // buffer input data
      _waveData.Add(sample);

      // be polite
      return 0;
    }

    // routine for saving a wave file. 
    //  returns 0 on success, negative value on error 
    public int Save(string filename)
    {
      int i;
      double ttt;

      if (_waveData.Count <= 0)
        throw new Exception("warning, no new data written to output");

      // allocate wav header
      WaveHeader wav = new WaveHeader();
      ChunkHeader chk = new ChunkHeader();

      /* allocate new data buffers */
      int wbuff_len = _waveData.Count * _bitsPerSample / 8;
      List<byte> wbuff = new List<byte>(wbuff_len);

      // setup wav header
      wav.rID = "RIFF";
      wav.wID = "WAVE";
      wav.fId = "fmt ";
      wav.nBitsPerSample = (short)_bitsPerSample;
      wav.nSamplesPerSec = (int)_sampleRate;

      wav.nAvgBytesPerSec = (int)_sampleRate;
      wav.nAvgBytesPerSec *= _bitsPerSample / 8;
      wav.nAvgBytesPerSec *= _numChannels;

      wav.nChannels = (short)_numChannels;
      wav.pcm_header_len = 16;
      wav.wFormatTag = 1;
      wav.rLen = Marshal.SizeOf(typeof(WaveHeader)) + Marshal.SizeOf(typeof(ChunkHeader)) + wbuff_len;
      wav.nBlockAlign = (short)(_numChannels * _bitsPerSample / 8);

      // setup chunk header
      chk.dId = "data";
      chk.dLen = wbuff_len;

      // convert data
      if (_bitsPerSample == 16)
      {
        for (i = 0; i < _waveData.Count; i++)
        {
          ttt = Wolfram.Clamp(_waveData[i], MinUuu, MaxUuu);
          wbuff.AddRange(BitConverter.GetBytes((short)ttt));
        }
      }
      else if (_bitsPerSample == 8)
      {
        for (i = 0; i < _waveData.Count; i++)
        {
          ttt = Wolfram.Clamp(_waveData[i], MinCcc, MaxCcc);
          wbuff.AddRange(BitConverter.GetBytes((byte)ttt));
        }
      }
      else
      {
        throw new Exception("bunk bits per sample");
      }

      /* open wav file */
      BinaryWriter fw = new BinaryWriter(File.Open(filename, FileMode.Create, FileAccess.Write));

      /* write riff/wav header */
      wav.WriteToStream(fw);

      /* write chunk header */
      chk.WriteToStream(fw);

      /* convert to byte array */
      byte[] data = wbuff.ToArray();

      /* write data */
      fw.Write(data, 0, data.Length);

      // reset output stream index
      _waveData.Clear();

      // be polite
      fw.Close();
      return 0;
    }
  }
}