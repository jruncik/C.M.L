/* ------------------------------------------------------------------------- *
 * Copyright (C) 2008-2009 Jaroslav Runcik
 *
 * Jaroslav Runcik <J [dot] Runcik [at] seznam [dot] cz>
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
 * ------------------------------------------------------------------------- */

using System;

using SR.CML.Core.InSimCommon;

using log4net;

using System.Diagnostics;

namespace SR.CML.Common
{
	public class LfsCommands
	{
		private static ILog	_log		= LogManager.GetLogger(typeof(LfsCommands));
		private static bool	_logDebug	= _log.IsDebugEnabled;

		private static String YES	= "yes";
		private static String NO	= "no";

		private static String BoolToString(bool value) {
			String result = NO;
			if (value) {
				result = YES;
			}
			return result;
		}

		public static String Spectate(IInSimDriver driver)
		{
			Debug.Assert(driver!=null);
			if (driver == null) {
				_log.Error("Spectate driver null");
				return String.Empty;
			}

			return String.Format(String.Format("/spec {0}", driver.ColorizedNickName));
		}

		public static String Spectate(IInSimDriverAi aiDriver)
		{
			Debug.Assert(aiDriver!=null);
			if (aiDriver == null) {
				_log.Error("Spectate Ai driver null");
				return String.Empty;
			}

			return String.Format(String.Format("/spec {0}", aiDriver.Name));
		}

		public static String StarQualify()
		{
			return "/qualify";
		}

		public static String SetQualifyTime(Int32 minuts)
		{
			return String.Format(String.Format("/qual {0}", minuts.ToString()));
		}

		public static String StarRacte()
		{
			return "/restart";
		}

		public static String SetRaceTime(Int32 hours)
		{
			return String.Format(String.Format("/hours {0}", hours.ToString()));
		}

		public static String SetRaceLaps(Int32 laps)
		{
			return String.Format(String.Format("/laps {0}", laps.ToString()));
		}

		public static String End()
		{
			return "/end";
		}

		public static String AddAiDriver()
		{
			return "/ai AI 1";
		}

		public static String RcmMessageText(String message)
		{
			return String.Format(String.Format("/rcm {0}", message));
		}

		public static String RcmSendToPlayer(String lfsUsername)
		{
			return String.Format(String.Format("/rcm_ply {0}", lfsUsername));
		}

		public static String RcmSendToAll()
		{
			return "/rcm_all";
		}

		public static String ClearPlayerRcm(String lfsUsername)
		{
			return String.Format(String.Format("/rcc_ply {0}", lfsUsername));
		}

		public static String ClearAllRcm()
		{
			return "/rcc_all";
		}

		public static String SetWind(LfsWind wind) {
			return String.Format(String.Format("/wind={0}", (int)wind));
		}

		public static String SetWeather(LfsWheather weather) {
			return String.Format(String.Format("/weather={0}", (int)weather));
		}

		public static String SetHostName(String hostName) {
			return String.Format(String.Format("/host={0}", hostName));
		}

		public static String SetPassword(String password) {
			return String.Format(String.Format("/pass={0}", password));
		}

		public static String SetTrack(LfsTrack track) {
			return String.Format(String.Format("/track={0}", track.ToString()));
		}
		
		 
		// max guests that can join host
		public static String SetMaxGuests(Int32 maxguests) {
			return String.Format(String.Format("/maxguests={0}", maxguests));
		}

		// max cars in a race
		public static String SetMaxCars(Int32 carsmax) {
			return String.Format(String.Format("/carsmax={0}", carsmax));
		}

		// max cars (real+ai) on host pc
		public static String SetCarsHost(Int32 carshost) {
			return String.Format(String.Format("/carshost={0}", carshost));
		}

		// max cars (real+ai) per guest pc
		public static String SetCarsGuest(Int32 carsguest) {
			return String.Format(String.Format("/carsguest={0}", carsguest));
		}

		// smoothness (3-6) number of car updates per second
		public static String SetPps(Int32 pps) {
			return String.Format(String.Format("/pps={0}", pps));
		}

		public static String SetVote(bool vote) {
			String value = BoolToString(vote);
			return String.Format(String.Format("/vote={0}", value));
		}

		// no/yes: can guests select track
		public static String SetSelectTrack(bool selectTrack) {
			String value = BoolToString(selectTrack);
			return String.Format(String.Format("/select={0}", value));
		}

		public static String SetMidRaceConnect(bool midrace) {
			String value = BoolToString(midrace);
			return String.Format(String.Format("/midrace={0}", value));
		}

		public static String SetMustPit(bool mustpit) {
			String value = BoolToString(mustpit);
			return String.Format(String.Format("/mustpit={0}", value));
		}

		public static String SetCanReset(bool canreset) {
			String value = BoolToString(canreset);
			return String.Format(String.Format("/canreset={0}", value));
		}

		public static String SetForceCockpitView(bool fcv) {
			String value = BoolToString(fcv);
			return String.Format(String.Format("/fcv={0}", value));
		}

		// no/yes: allow wrong way driving
		public static String SetAllowWrongWay(bool cruise) {
			String value = BoolToString(cruise);
			return String.Format(String.Format("/cruise={0}", value));
		}

		public static String SetNetworkDebug(bool ndebug) {
			String value = BoolToString(ndebug);
			return String.Format(String.Format("/ndebug={0}", value));
		}

		public static String SetStartGridOrder(GridOrder order) {
			String orderCmd = order.ToString().ToLower();
			return String.Format(String.Format("/start={0}", orderCmd));
		}


		///kick X              :disconnect user X
		///ban X Y             :ban user X for Y days (0 = 12 hours)
		///unban X             :remove ban on user X
		///pitlane X           :send user X to the pit lane
		///pit_all             :send all cars to the pit lane
		///p_dt USERNAME       :give drive through penalty
		///p_sg USERNAME       :give stop-go penalty
		///p_30 USERNAME       :give 30 second time penalty
		///p_45 USERNAME       :give 45 second time penalty
		///p_clear USERNAME    :clear a time or pit penalty

	}
}
