using System;
using System.Collections.Generic;
using FullMotion.LiveForSpeed.InSim;

using System.Diagnostics;

namespace ReplayTest
{
	class Program
	{
		private static Dictionary<byte, String> drivers = new Dictionary<byte,String>(32);
		private static InSimHandler handler = new InSimHandler();

		static void Main(string[] args)
		{
			handler.Configuration.AdminPass = "3ne494ek";
			handler.Configuration.Local = true;
			handler.Configuration.UseTCP = true;
			handler.Configuration.LFSHostPort = 29999;
			handler.Configuration.LFSHost = "127.0.0.1";

			handler.Initialize();

			handler.RaceTrackConnection += new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackNewConnectionHandler(handler_RaceTrackConnection);
			handler.RaceTrackPlayer += new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerHandler(handler_RaceTrackPlayer);
			handler.RaceTrackPlayerFinish += new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerFinishHandler(handler_RaceTrackPlayerFinish);
			handler.RaceTrackPlayerResult += new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerResultHandler(handler_RaceTrackPlayerResult);
			handler.RaceTrackPlayerLap += new FullMotion.LiveForSpeed.InSim.EventHandlers.RaceTrackPlayerLapHandler(handler_RaceTrackPlayerLap);

			ConsoleKeyInfo	cki;
			bool quit = false;

			do {
				cki = Console.ReadKey(true);

				switch (cki.Key) {
					case ConsoleKey.A : {
						Grid();
					} break;

					case ConsoleKey.Escape : {
						Console.WriteLine("Exit");
						quit = true;
					} break;
				}

			} while(!quit);

		}

		static void Grid()
		{
			List<Byte> arrayId = new List<Byte>();

			foreach(Byte id in drivers.Keys) {
				arrayId.Add(id);
			}

			handler.SetGridOrder(arrayId.ToArray());
		}

		static void handler_RaceTrackConnection(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackConnection e)
		{
			Console.WriteLine(String.Format("RaceTrackConnection - PlayerName '{0}', Username : {1}", e.PlayerName, e.UserName));
		}

		static String PlayerName(Byte id) {
			if (drivers.ContainsKey(id)) {
				return drivers[id];
			}

			return String.Empty;
		}

		static void handler_RaceTrackPlayer(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackPlayer e)
		{
			if (String.IsNullOrEmpty(e.PlayerName))
				return;

			if (drivers.ContainsKey(e.PlayerId))
				return;

			String playerName = e.PlayerName;
			playerName = e.PlayerName.Replace("^0", "");
			playerName = e.PlayerName.Replace("^1", "");
			playerName = e.PlayerName.Replace("^2", "");
			playerName = e.PlayerName.Replace("^3", "");
			playerName = e.PlayerName.Replace("^4", "");
			playerName = e.PlayerName.Replace("^5", "");
			playerName = e.PlayerName.Replace("^6", "");
			playerName = e.PlayerName.Replace("^7", "");
			playerName = e.PlayerName.Replace("^8", "");
			playerName = e.PlayerName.Replace("^9", "");


			drivers.Add(e.PlayerId, playerName);
			Console.WriteLine(String.Format("RaceTrackPlayer - PalyerId '{0}', Name : {1}", e.PlayerId, playerName));
		}

		static void handler_RaceTrackPlayerLap(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackPlayerLap e)
		{
			//Console.WriteLine(String.Format("RaceTrackPlayerLap -Palyer '{0}', time : {1}", PlayerName(e.PlayerId), e.LapTime));
		}

		static void handler_RaceTrackPlayerResult(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackPlayerResult e)
		{
			Console.WriteLine(String.Format("RaceTrackPlayerResult - Palyer '{0}', Mentioned {1}, LapsDone {2}, Confirmed {3}, ResultNumber {4}, UserName {5}, TotalTime {6}, Disqualified {7}, BestLapTime {8}",
				PlayerName(e.PlayerId), e.Mentioned, e.LapsDone, e.Confirmed, e.ResultNumber, e.UserName, e.TotalTime, e.Disqualified, e.BestLapTime));
		}

		static void handler_RaceTrackPlayerFinish(InSimHandler sender, FullMotion.LiveForSpeed.InSim.Events.RaceTrackPlayerFinish e)
		{
			Console.WriteLine(String.Format("handler_RaceTrackPlayerFinish - Palyer '{0}', RaceTime : {1}", PlayerName(e.PlayerId), e.RaceTime));
		}
	}
}
