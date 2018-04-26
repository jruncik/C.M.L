using System;
using System.Collections.Generic;
using System.Text;
using FullMotion.LiveForSpeed.InSim.Flags;

namespace FullMotion.LiveForSpeed.InSim.Packets.Support
{
  struct HInfo
  {
    public const int SIZE = 40;

    public byte[] HName; //[32];	// Name of the host
	  public byte[]	Track; //[6]	// Short track name
	  public HOS Flags;		// Info flags about the host - see NOTE 1) below
	  public byte NumConns;	// Number of people on the host

    public HInfo(byte[] bytes)
    {
      int position = 0;
      HName = new byte[32];
      Array.Copy(bytes, position, HName, 0, 32);
      position += 32;
      Track = new byte[6];
      Array.Copy(bytes, position, Track, 0, 6);
      position += 6;
      Flags = (HOS)bytes[position++];
      NumConns = bytes[position++];
    }
  }
}
