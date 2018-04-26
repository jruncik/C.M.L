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
using System.Diagnostics;

namespace SR.CML.Common
{
	[Serializable]
	public enum LfsTrack
	{
		None,

		BL1,
		BL1R,
		BL2,
		BL2R,

		FE1,
		FE1R,
		FE2,
		FE2R,
		FE3,
		FE3R,
		FE4,
		FE4R,
		FE5,
		FE5R,
		FE6,
		FE6R,

		SO1,
		SO1R,
		SO2,
		SO2R,
		SO3,
		SO3R,
		SO4,
		SO4R,
		SO5,
		SO5R,
		SO6,
		SO6R,

		AU1,
		AU2,
		AU3,
		AU4,

		KY1,
		KY1R,
		KY2,
		KY2R,
		KY3,
		KY3R,

		WE1,
		WE1R,

		AS1,
		AS1R,
		AS2,
		AS2R,
		AS3,
		AS3R,
		AS4,
		AS4R,
		AS5,
		AS5R,
		AS6,
		AS6R,
		AS7,
		AS7R
	}

	public class LfsTrackNames
	{
		public static String GetFullTrackName(LfsTrack track)
		{
			switch (track) {

				case LfsTrack.BL1:	return "Blackwood GP";
				case LfsTrack.BL1R:	return "Blackwood GP Reverse";
				case LfsTrack.BL2:	return "Blackwood RallyX";
				case LfsTrack.BL2R:	return "Blackwood RallyX Reverse";

				case LfsTrack.FE1:	return "Fern Bay Club";
				case LfsTrack.FE1R:	return "Fern Bay Club Reverse";
				case LfsTrack.FE2:	return "Fern Bay Green";
				case LfsTrack.FE2R:	return "Fern Bay Green Reverse";
				case LfsTrack.FE3:	return "Fern Bay Gold";
				case LfsTrack.FE3R:	return "Fern Bay Gold Reverse";
				case LfsTrack.FE4:	return "Fern Bay Black";
				case LfsTrack.FE4R:	return "Fern Bay Black Reverse";
				case LfsTrack.FE5:	return "Fern Bay Rallycross";
				case LfsTrack.FE5R:	return "Fern Bay Rallycross Reverse";
				case LfsTrack.FE6:	return "Fern Bay RallyX Green";
				case LfsTrack.FE6R:	return "Fern Bay RallyX Green Reverse";

				case LfsTrack.SO1:	return "South City Classic";
				case LfsTrack.SO1R:	return "South City Classic Reverse";
				case LfsTrack.SO2:	return "South City Sprint 1";
				case LfsTrack.SO2R:	return "South City Sprint 1 Reverse";
				case LfsTrack.SO3:	return "South City Sprint 2";
				case LfsTrack.SO3R:	return "South City Sprint 2 Reverse";
				case LfsTrack.SO4:	return "South City Long";
				case LfsTrack.SO4R:	return"South City Long Reverse";
				case LfsTrack.SO5:	return "South Town Course";
				case LfsTrack.SO5R:	return "South Town Course Reverse";
				case LfsTrack.SO6:	return "South Chicane Route";
				case LfsTrack.SO6R:	return "South Chicane Route Reverse";

				case LfsTrack.AU1:	return "Autocross Arena";
				case LfsTrack.AU2:	return "Skidpad";
				case LfsTrack.AU3:	return "Drag Strip";
				case LfsTrack.AU4:	return "8 Lane Drag";

				case LfsTrack.KY1:	return "Kyoto ring Oval";
				case LfsTrack.KY1R:	return "Kyoto ring Oval reversed";
				case LfsTrack.KY2:	return "Kyoto ring National";
				case LfsTrack.KY2R:	return "Kyoto ring National reversed";
				case LfsTrack.KY3:	return "Kyoto ring GP long";
				case LfsTrack.KY3R:	return "Kyoto ring GP long reversed";

				case LfsTrack.WE1:	return "Westhill International";
				case LfsTrack.WE1R:	return "Westhill International reversed";

				case LfsTrack.AS1:	return "Aston Cadet";
				case LfsTrack.AS1R:	return "Aston Cadet reversed";
				case LfsTrack.AS2:	return "Aston Club";
				case LfsTrack.AS2R:	return "Aston Club reversed";
				case LfsTrack.AS3:	return "Aston National";
				case LfsTrack.AS3R:	return "Aston National reversed";
				case LfsTrack.AS4:	return "Aston Historic";
				case LfsTrack.AS4R:	return "Aston Historic reversed";
				case LfsTrack.AS5:	return "Aston Grand prix";
				case LfsTrack.AS5R:	return "Aston Grand prix reversed";
				case LfsTrack.AS6:	return "Aston Grand Touring";
				case LfsTrack.AS6R:	return "Aston Grand Touring reversed";
				case LfsTrack.AS7:	return "Aston North";
				case LfsTrack.AS7R:	return "Aston North reversed";
			}

			Debug.Assert(false, "Unknown track");
			return String.Empty;
		}
	}
}
