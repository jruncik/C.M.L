/* ------------------------------------------------------------------------- *
 * Copyright (C) 2005-2008 Arne Claassen
 *
 * Arne Claassen <lfslib [at] claassen [dot] net>
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * ------------------------------------------------------------------------- */
using System;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using log4net;
using LFSExceptions = FullMotion.LiveForSpeed.InSim.Exceptions;
using FullMotion.LiveForSpeed.Util;

namespace FullMotion.LiveForSpeed.InSim
{
	/// <summary>
	/// LFSHandler configuration class
	/// </summary>
	public class Configuration
	{
		#region Static Members ########################################################################
		private static readonly ILog log = LogManager.GetLogger(typeof(Configuration));
		#endregion

		#region Constants #############################################################################
		private const string DEFAULT_HOST = "127.0.0.1";
		private const int DEFAULT_HOST_PORT = 30000;
		private const int DEFAULT_REPLY_PORT = 29999;
		private const int DEFAULT_NODE_INTERVAL = 0;
		private const string CONFIGURATION_TAG = "lfsinsim";
		#endregion

		#region Member Variables ######################################################################
    private LfsInSim data = new LfsInSim();
		#endregion

		#region Constructors ##########################################################################
		internal Configuration()
		{
			data.Init();
			data.Host.Address = DEFAULT_HOST;
			data.Host.Port = DEFAULT_HOST_PORT;
			data.InSim.TrackingInterval = DEFAULT_NODE_INTERVAL;
      data.InSim.ProgramName = "LFSLib";
      data.InSim.AutoReconnect = false;
      data.InSim.ReconnectRetry = 5;
		}
		#endregion

		#region Properties ############################################################################
    /// <summary>
    /// Use TCP instead of UDP for communicating with LFS. LFS can have up to 8 TCP connections, but
    /// only one UDP connection. If the <see cref="ReplyPort"/> is set, NodeLap and MultiCarInfo
    /// tracking events will be sent via UDP regardless of what UseTCP is set to
    /// </summary>
    public bool UseTCP
    {
      get { return data.InSim.UseTCP; }
      set { data.InSim.UseTCP = value; }
    }

    /// <summary>
    /// Keep the Message color control sequences in the text strings or strip them out
    /// </summary>
    public bool KeepMessageColors
    {
      get { return data.InSim.KeepMessageColors; }
      set { data.InSim.KeepMessageColors = value; }
    }

    /// <summary>
    /// Try to reconnect after a connection failure (default false)
    /// </summary>
    public bool AutoReconnect
    {
      get { return data.InSim.AutoReconnect; }
      set { data.InSim.AutoReconnect = value; }
    }

    /// <summary>
    /// How many retry attempts before giving up
    /// </summary>
    public int ReconnectRetry
    {
      get { return data.InSim.ReconnectRetry; }
      set { data.InSim.ReconnectRetry = value; }
    }

    /// <summary>
    /// Is this a local connection or a connection to a host (used for button Ids)
    /// </summary>
    public bool Local
    {
      get { return data.InSim.Local; }
      set { data.InSim.Local = value; }
    }

    /// <summary>
    /// Request to receive <see cref="Events.RaceTrackMultiCarInfo"/> events
    /// </summary>
    public bool MultiCarTracking
    {
      get { return data.InSim.MultiCarTracking; }
      set { data.InSim.MultiCarTracking = value; }
    }

    /// <summary>
    /// Request to receive <see cref="Events.RaceTrackNodeLap"/> events
    /// </summary>
    public bool NodeLapTracking
    {
      get { return data.InSim.NodeLapTracking; }
      set { data.InSim.NodeLapTracking = value; }
    }

    /// <summary>
    /// Message prefix character to mark incoming messages with <see cref="Enums.UserType.Prefix"/>
    /// </summary>
    public char Prefix
    {
      get { return data.InSim.Prefix; }
      set { data.InSim.Prefix = value; }
    }

    /// <summary>
    /// Name of the calling program to report to LFS
    /// </summary>
    public string ProgramName
    {
      get { return data.InSim.ProgramName; }
      set { data.InSim.ProgramName = value; }
    }

    /// <summary>
		/// The name of the LFS host
		/// </summary>
		public string LFSHost
		{
			get { return data.Host.Address; }
			set { data.Host.Address = value; }
		}

		/// <summary>
		/// The port to hit on the LFS host
		/// </summary>
		public int LFSHostPort
		{
			get { return data.Host.Port; }
			set { data.Host.Port = value; }
		}

		/// <summary>
		/// The port on which LFSHandler will listen for replies
		/// </summary>
		public int ReplyPort
		{
			get { return (int)data.InSim.Port; }
			set { data.InSim.Port = (ushort)value; }
		}
    /// <summary>
    /// Use Split Messages instead of single Datagram Messages
    /// </summary>
    public bool UseSplitMessages
    {
      get { return data.InSim.UseSplitMessages; }
      set { data.InSim.UseSplitMessages = value; }
    }
    /// <summary>
		/// The number of milliseconds between tracking messages sent by the host.
		/// 0 means no tracking messages, otherwise 100 is the smallest amount.
		/// </summary>
		public int TrackingInterval
		{
			get { return data.InSim.TrackingInterval; }
			set 
			{
				if( value < 100 && value != 0 )
				{
					throw new ArgumentException("Interval has to be 0 or greater than 100 milliseconds");
				}
				data.InSim.TrackingInterval = value;
			}
		}

		/// <summary>
		/// The admin password the host expects, if any is set
		/// </summary>
		public string AdminPass
		{
			get { return data.Host.Password; } 
			set { data.Host.Password = value; }
		}

    /// <summary>
    /// The number of hundredths between OutSim messages sent by the host. If 0, OutSim is not
    /// initialized. OutSim messages arrive via the same port as InSim and are accessible via events on
    /// <see cref="InSimHandler"/>
    /// </summary>
    public int OutSimInterval
    {
      get { return data.OutSim.Interval; }
      set { data.OutSim.Interval = value; }
    }

    /// <summary>
    /// The number of hundredths between OutGauge messages sent by the host. If 0, OutGauge is not
    /// initialized. OutGauge messages arrive via the same port as InSim and are accessible via events on
    /// <see cref="InSimHandler"/>
    /// </summary>
    public int OutGaugeInterval
    {
      get { return data.OutGauge.Interval; }
      set { data.OutGauge.Interval = value; }
    }
    #endregion

		#region Methods ###############################################################################
    /// <summary>
    /// Returns the MsgInit object so that it can be sent to the LFS host
    /// </summary>
    /// <returns></returns>
    internal Packets.IS_ISI GetInitPacket()
    {
      Flags.ISF flags = 0;
      if (data.InSim.Local)
      {
        flags |= Flags.ISF.LOCAL;
      }
      if (data.InSim.KeepMessageColors)
      {
        flags |= Flags.ISF.MSO_COLS;
      }
      if (data.InSim.MultiCarTracking)
      {
        flags |= Flags.ISF.MCI;
      }
      if (data.InSim.NodeLapTracking)
      {
        flags |= Flags.ISF.NLP;
      }

      ushort replyPort = data.InSim.Port;
      if (replyPort == 0 && !data.InSim.UseTCP)
      {
        replyPort = DEFAULT_REPLY_PORT;
      }

      Packets.IS_ISI init = new Packets.IS_ISI(
        data.InSim.Port,
        flags,
        (byte)data.InSim.Prefix,
        (ushort)data.InSim.TrackingInterval,
        data.Host.Password,
        data.InSim.ProgramName
        );



      return init;
    }
    /// <summary>
		/// Overwrites any existing configuration with information from config file.
		/// Throws <see cref="LFSExceptions.ConfigurationException.FileNotFound"/>
		/// or <see cref="LFSExceptions.ConfigurationException.InvalidFileFormat"/> on failure
		/// </summary>
		/// <param name="filepath"></param>
		public void Load(string filepath)
		{
			if( !File.Exists(filepath) )
			{
				log.Debug("configuration file not found");
				throw new LFSExceptions.ConfigurationException.FileNotFound();
			}
			try 
			{
        data = LfsInSim.Load(filepath);
			}
			catch(Exception e)
			{
				log.Debug("problem parsing file");
				throw new LFSExceptions.ConfigurationException.InvalidFileFormat(e);
			}
		}

		/// <summary>
		/// Save the current configuration out to a file
		/// </summary>
		/// <param name="filepath"></param>
		public void Save(string filepath)
		{
			data.Save(filepath);
		}
		#endregion

		#region Private Methods #######################################################################
		internal void InitFromApplicationConfig()
		{
      data = (LfsInSim)ConfigurationManager.GetSection(CONFIGURATION_TAG);
		}

		/// <summary>
		/// Overwrites any existing configuration with information from provided configuration object.
		/// </summary>
		/// <param name="configurationData">Object to internalize</param>
    internal void Init(LfsInSim configurationData)
		{
			data = configurationData;
		}

		#endregion

		#region Nested Classes ########################################################################
		/// <summary>
		/// LFSLib is the container class for the configuration Xml data storage. It and it's contained
		/// objects are XmlSerializable. They shouldn't be created by hand, but could be used if 
		/// the configuration was to be embedded into a separate XmlSerializable configuration scheme.
		/// On being serialized, this class creates the tag &lt;lfslib/&gt;
		/// </summary>
		[XmlRootAttribute(CONFIGURATION_TAG)]
		public class LfsInSim
		{
			/// <summary>
			/// Required for <see cref="XmlSerializer"/>
			/// </summary>
      public LfsInSim()
			{
			}
			/// <summary>
			/// Represents the &lt;host/&gt; tag in the configuration file
			/// </summary>
			[XmlElement("host")]
			public Host Host;

      /// <summary>
			/// Represents the &lt;insim/&gt; tag in the configuration file
			/// </summary>
			[XmlElement("insim")]
			public InSim InSim;

      /// <summary>
      /// Represents the &lt;insim/&gt; tag in the configuration file
      /// </summary>
      [XmlElement("outsim")]
      public OutSim OutSim;

      /// <summary>
      /// Represents the &lt;insim/&gt; tag in the configuration file
      /// </summary>
      [XmlElement("outgauge")]
      public OutGauge OutGauge;

      internal void Save(string path)
			{
				XmlSerializer ser = new XmlSerializer(this.GetType());
				StreamWriter sw = new StreamWriter(path);
				ser.Serialize(sw,this);
				sw.Close();
			}

			internal void Init()
			{
				this.Host = new Host();
				this.InSim = new InSim();
        this.OutSim = new OutSim();
        this.OutGauge = new OutGauge();
			}

      internal static LfsInSim Load(string path)
			{
        LfsInSim returnObj;

        XmlSerializer ser = new XmlSerializer(typeof(LfsInSim));
				StreamReader sr = new StreamReader(path);
        returnObj = (LfsInSim)ser.Deserialize(sr);
				sr.Close();

        if (returnObj.Host == null)
        {
          returnObj.Host = new Host();
        }
        if (returnObj.InSim == null)
        {
          returnObj.InSim = new InSim();
        }
        if (returnObj.OutSim == null)
        {
          returnObj.OutSim = new OutSim();
        }
        if (returnObj.OutGauge == null)
        {
          returnObj.OutGauge = new OutGauge();
        }
        return returnObj;
			}
		}

		/// <summary>
		/// Part of the Configuration XmlSerializable storage data
		/// </summary>
		[XmlRootAttribute("host")]
		public class Host
		{
			/// <summary>
			/// Required for <see cref="XmlSerializer"/>
			/// </summary>
			public Host()
			{
			}
			/// <summary>
			/// Don't access directly to set configuration
			/// </summary>
			[XmlAttribute("addr")]
			public string Address;
			/// <summary>
			/// Don't access directly to set configuration
			/// </summary>
			[XmlAttribute("pass")]
			public string Password;
			/// <summary>
			/// Don't access directly to set configuration
			/// </summary>
			[XmlAttribute("port")]
			public int Port;
		}

		/// <summary>
		/// Part of the Configuration XmlSerializable storage data
		/// </summary>
		[XmlRootAttribute("insim")]
		public class InSim
		{
			/// <summary>
			/// Required for <see cref="XmlSerializer"/>
			/// </summary>
			public InSim()
			{
			}
      /// <summary>
      /// Don't access directly to set configuration
      /// </summary>
      [XmlAttribute("use_tcp")]
      public bool UseTCP;

      /// <summary>
      /// Don't access directly to set configuration
      /// </summary>
      [XmlAttribute("auto_reconnect")]
      public bool AutoReconnect;

      /// <summary>
      /// Don't access directly to set configuration
      /// </summary>
      [XmlAttribute("reconnect_retry")]
      public int ReconnectRetry;

      /// <summary>
			/// Don't access directly to set configuration
			/// </summary>
			[XmlAttribute("port")]
			public ushort Port;
			/// <summary>
			/// Don't access directly to set configuration
			/// </summary>
			[XmlAttribute("local")]
			public bool Local;
			/// <summary>
			/// Don't access directly to set configuration
			/// </summary>
			[XmlAttribute("keep_message_colors")]
      public bool KeepMessageColors;
			/// <summary>
			/// Don't access directly to set configuration
			/// </summary>
			[XmlAttribute("tracking_interval")]
			public int TrackingInterval;
			/// <summary>
			/// Don't access directly to set configuration
			/// </summary>
			[XmlAttribute("multi_car_tracking")]
      public bool MultiCarTracking;
			/// <summary>
			/// Don't access directly to set configuration
			/// </summary>
			[XmlAttribute("nodelap_tracking")]
      public bool NodeLapTracking;
      /// <summary>
      /// Don't access directly to set configuration
      /// </summary>
      [XmlAttribute("program_name")]
      public string ProgramName;
      /// <summary>
      /// Don't access directly to set configuration
      /// </summary>
      [XmlAttribute("prefix")]
      public char Prefix;
      /// <summary>
      /// Don't access directly to set configuration
      /// </summary>
      [XmlAttribute("split_msg")]
      public bool UseSplitMessages;
    }

    /// <summary>
    /// Part of the Configuration XmlSerializable storage data
    /// </summary>
    [XmlRootAttribute("outgauge")]
    public class OutGauge
    {
      /// <summary>
      /// Required for <see cref="XmlSerializer"/>
      /// </summary>
      public OutGauge()
      {
      }
      /// <summary>
      /// Don't access directly to set configuration
      /// </summary>
      [XmlAttribute("interval")]
      public int Interval;
    }

    /// <summary>
    /// Part of the Configuration XmlSerializable storage data
    /// </summary>
    [XmlRootAttribute("outgauge")]
    public class OutSim
    {
      /// <summary>
      /// Required for <see cref="XmlSerializer"/>
      /// </summary>
      public OutSim()
      {
      }
      /// <summary>
      /// Don't access directly to set configuration
      /// </summary>
      [XmlAttribute("interval")]
      public int Interval;
    }

    #endregion
	}
}

namespace FullMotion.LiveForSpeed.InSim.Exceptions
{
	/// <summary>
	/// Abstract base for all Configuration related Exceptions
	/// </summary>
	public abstract class ConfigurationException : Exception
	{
		internal ConfigurationException(string message, Exception InnerException) : base( message, InnerException )
		{
		}

		internal ConfigurationException() : base()
		{
		}

		/// <summary>
		/// The configuration File was not found
		/// </summary>
		public class FileNotFound : ConfigurationException
		{
		}
		
		/// <summary>
		/// The configuration format is not valid
		/// </summary>
		public class InvalidFileFormat : ConfigurationException
		{
			const string ERROR = "Caught exception trying to process Configuration File";
			internal InvalidFileFormat(Exception InnerException) : base( ERROR, InnerException )
			{
			}
		}
		/// <summary>
		/// The configuration format is not valid
		/// </summary>
		public class InvalidConfiguration : ConfigurationException
		{
			const string ERROR = "Caught exception trying to process Configuration xml";
			internal InvalidConfiguration(Exception InnerException) : base( ERROR, InnerException )
			{
			}
		}
	}
}
