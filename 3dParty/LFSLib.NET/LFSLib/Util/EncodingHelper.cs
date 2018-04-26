/* ------------------------------------------------------------------------- *
 * Copyright (C) 2007 Arne Claassen
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
using System.Collections.Generic;
using System.Text;

namespace FullMotion.LiveForSpeed.Util
{
	public class EncodingHelper
	{
		// TODO: there are some mappings that under Japanese encoding come out wrong and need to be remapped
		public const int LATIN1 = 1252;
		public const int BALTIC = 1257;
		public const int CYRILLIC = 1251;
		public const int TURKISH = 1254;
		public const int JAPANESE = 932;
		public const int CENTRAL_EUROPE = 1250;
		public const int GREEK = 1253;

		public static readonly Encoding encodingLatin1 = System.Text.Encoding.GetEncoding(LATIN1);
		public static readonly Encoding encodingBaltic = System.Text.Encoding.GetEncoding(BALTIC);
		public static readonly Encoding encodingCyrillic = System.Text.Encoding.GetEncoding(CYRILLIC);
		public static readonly Encoding encodingTurkish = System.Text.Encoding.GetEncoding(TURKISH);
		public static readonly Encoding encodingJapanese = System.Text.Encoding.GetEncoding(JAPANESE);
		public static readonly Encoding encodingCentralEurope = System.Text.Encoding.GetEncoding(CENTRAL_EUROPE);
		public static readonly Encoding encodingGreek = System.Text.Encoding.GetEncoding(GREEK);

		static object padLock = new object();
		static Dictionary<int, Encoding> encodingMap;
		static Dictionary<int, Encoding> encodingLookup;

		public static Encoding GetEncoding(int unicode) {
			if (encodingMap == null) {
				lock (padLock) {
					if (encodingMap == null) {

						encodingMap = new Dictionary<int, Encoding>();
						encodingLookup = new Dictionary<int, Encoding>();
						encodingMap.Add(LATIN1, encodingLatin1);
						encodingMap.Add(BALTIC, encodingBaltic);
						encodingMap.Add(CYRILLIC, encodingCyrillic);
						encodingMap.Add(TURKISH, encodingTurkish);
						encodingMap.Add(JAPANESE, encodingJapanese);
						encodingMap.Add(CENTRAL_EUROPE, encodingCentralEurope);
						encodingMap.Add(GREEK, encodingGreek);
						encodingLookup.Add(128, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(129, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(131, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(136, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(138, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(140, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(141, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(142, encodingMap[1254]); // Turkish (Windows)
						encodingLookup.Add(143, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(144, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(152, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(154, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(156, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(157, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(158, encodingMap[1254]); // Turkish (Windows)
						encodingLookup.Add(159, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(160, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(161, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(162, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(163, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(164, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(165, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(166, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(167, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(168, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(169, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(170, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(171, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(172, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(173, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(174, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(175, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(176, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(177, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(178, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(179, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(180, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(181, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(182, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(183, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(184, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(185, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(186, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(187, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(188, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(189, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(190, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(191, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(192, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(193, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(194, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(195, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(196, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(197, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(198, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(199, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(200, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(201, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(202, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(203, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(204, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(205, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(206, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(207, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(208, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(209, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(210, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(211, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(212, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(213, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(214, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(215, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(216, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(217, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(218, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(219, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(220, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(221, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(222, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(223, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(224, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(225, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(226, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(227, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(228, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(229, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(230, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(231, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(232, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(233, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(234, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(235, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(236, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(237, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(238, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(239, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(240, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(241, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(242, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(243, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(244, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(245, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(246, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(247, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(248, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(249, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(250, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(251, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(252, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(253, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(254, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(256, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(257, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(258, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(259, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(260, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(261, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(262, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(263, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(268, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(269, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(270, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(271, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(272, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(273, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(274, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(275, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(278, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(279, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(280, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(281, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(282, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(283, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(286, encodingMap[1254]); // Turkish (Windows)
						encodingLookup.Add(287, encodingMap[1254]); // Turkish (Windows)
						encodingLookup.Add(290, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(291, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(298, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(299, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(302, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(303, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(304, encodingMap[1254]); // Turkish (Windows)
						encodingLookup.Add(305, encodingMap[1254]); // Turkish (Windows)
						encodingLookup.Add(310, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(311, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(313, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(314, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(315, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(316, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(317, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(318, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(321, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(322, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(323, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(324, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(325, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(326, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(327, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(328, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(332, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(333, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(336, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(337, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(338, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(339, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(340, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(341, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(342, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(343, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(344, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(345, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(346, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(347, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(350, encodingMap[1254]); // Turkish (Windows)
						encodingLookup.Add(351, encodingMap[1254]); // Turkish (Windows)
						encodingLookup.Add(352, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(353, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(354, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(355, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(356, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(357, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(362, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(363, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(366, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(367, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(368, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(369, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(370, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(371, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(376, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(377, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(378, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(379, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(380, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(381, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(382, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(402, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(710, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(711, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(728, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(731, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(732, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(733, encodingMap[1250]); // Central European (Windows)
						encodingLookup.Add(900, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(901, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(902, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(904, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(905, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(906, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(908, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(910, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(911, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(912, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(913, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(914, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(915, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(916, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(917, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(918, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(919, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(920, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(921, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(922, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(923, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(924, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(925, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(926, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(927, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(928, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(929, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(931, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(932, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(933, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(934, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(935, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(936, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(937, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(938, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(939, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(940, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(941, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(942, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(943, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(944, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(945, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(946, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(947, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(948, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(949, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(950, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(951, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(952, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(953, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(954, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(955, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(956, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(957, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(958, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(959, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(960, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(961, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(962, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(963, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(964, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(965, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(966, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(967, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(968, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(969, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(970, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(971, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(972, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(973, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(974, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(1025, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1026, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1027, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1028, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1029, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1030, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1031, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1032, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1033, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1034, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1035, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1036, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1038, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1039, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1040, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1041, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1042, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1043, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1044, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1045, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1046, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1047, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1048, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1049, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1050, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1051, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1052, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1053, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1054, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1055, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1056, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1057, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1058, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1059, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1060, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1061, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1062, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1063, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1064, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1065, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1066, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1067, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1068, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1069, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1070, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1071, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1072, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1073, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1074, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1075, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1076, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1077, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1078, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1079, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1080, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1081, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1082, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1083, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1084, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1085, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1086, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1087, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1088, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1089, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1090, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1091, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1092, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1093, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1094, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1095, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1096, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1097, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1098, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1099, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1100, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1101, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1102, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1105, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1106, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1107, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1108, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1109, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1110, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1111, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1112, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1113, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1114, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1115, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1116, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1118, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1119, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1168, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(1169, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(8211, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(8212, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(8213, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(8216, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(8217, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(8218, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(8220, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(8221, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(8222, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(8224, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(8225, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(8226, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(8230, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(8240, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(8249, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(8250, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(8364, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(8470, encodingMap[1251]); // Cyrillic (Windows)
						encodingLookup.Add(8482, encodingMap[1252]); // Western European (Windows)
						encodingLookup.Add(12539, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(63728, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(63729, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(63730, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(63737, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(63738, encodingMap[1253]); // Greek (Windows)
						encodingLookup.Add(63740, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(63741, encodingMap[1257]); // Baltic (Windows)
						encodingLookup.Add(65377, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65378, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65379, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65380, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65381, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65382, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65383, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65384, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65385, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65386, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65387, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65388, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65389, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65390, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65391, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65392, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65393, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65394, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65395, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65396, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65397, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65398, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65399, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65400, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65401, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65402, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65403, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65404, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65405, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65406, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65407, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65408, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65409, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65410, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65411, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65412, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65413, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65414, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65415, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65416, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65417, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65418, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65419, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65420, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65421, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65422, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65423, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65424, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65425, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65426, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65427, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65428, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65429, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65430, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65431, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65432, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65433, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65434, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65435, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65436, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65437, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65438, encodingMap[932]); // Japanese (Shift-JIS)
						encodingLookup.Add(65439, encodingMap[932]); // Japanese (Shift-JIS)
					}
				}
			}

			if (encodingLookup.ContainsKey(unicode)) {
				return encodingLookup[unicode];
			} else {
				return null;
			}
		}
	}
}
