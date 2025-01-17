/*
 *	This program is the CONFIDENTIAL and PROPRIETARY property 
 *	of Tomasello Software LLC. Any unauthorized use, reproduction or
 *	transfer of this computer program is strictly prohibited.
 *
 *      Copyright (c) 2004 Tomasello Software LLC.
 *	This is an unpublished work, and is subject to limited distribution and
 *	restricted disclosure only. ALL RIGHTS RESERVED.
 *
 *			RESTRICTED RIGHTS LEGEND
 *	Use, duplication, or disclosure by the Government is subject to
 *	restrictions set forth in subparagraph (c)(1)(ii) of the Rights in
 * 	Technical Data and Computer Software clause at DFARS 252.227-7013.
 *
 *	Angel Island UO Shard	Version 1.0
 *			Release A
 *			March 25, 2004
 */

/* Items/Skill Items/Camping/CampLogoutGump.cs
 * CHANGELOG:
 *	2/22/11, Adam
 *		This is now obsolete as I've updated the camping system to Run 2.0
 *	8/14/06, weaver
 *		Added null check when handling the bedroll passed (for tent cases).
 *	10/28/04 - Pix
 *		Logout confirmation times out now after 20 seconds.
 *		Moving from the location where you doubleclicked the open bedroll now stops
 *		you from instaloggging.
 *	10/22/04 - Pix
 *		Changed camping to not use campfireregion.
 *	5/10/04, Pixie
 *		Initial working revision
 */

#if obsolete

using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Items;

namespace Server.Gumps
{
	public class CampLogoutGump : Gump
	{
		private Mobile m_From;
		private UnrolledBedroll m_Bedroll;
		private Point3D m_Location;

		public CampLogoutGump(Mobile from, UnrolledBedroll bedroll)
			: base(150, 200)
		{
			m_From = from;
			m_Bedroll = bedroll;
			m_Location = from.Location;

			m_From.CloseGump(typeof(CampLogoutGump));

			AddPage(0);

			AddBackground(0, 0, 400, 200, 5054);

			AddHtml(130, 10, 200, 25, "Logging Out Via Camping", false, false);

			AddHtmlLocalized(40, 40, 320, 100, 1011016, 0, true, true);

			AddHtmlLocalized(70, 175, 140, 25, 1011011, false, false); // CONTINUE
			AddButton(40, 175, 4005, 4007, 2, GumpButtonType.Reply, 0);

			AddHtmlLocalized(160, 175, 140, 25, 1011012, false, false); // CANCEL
			AddButton(130, 175, 4005, 4007, 1, GumpButtonType.Reply, 0);

			new CampLogoutGumpTimeoutTimer(from).Start();
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			if (info.ButtonID == 2)
			{
				// wea: 14/Aug/2006 added null check to bedroll (for instances when we log out in tents)
				if (m_Bedroll != null)
				{
					m_Bedroll.Delete();
					state.Mobile.AddToBackpack(new Bedroll());
				}

				if (m_Location != m_From.Location)
				{
					m_From.SendMessage("Moving from the location of the bedroll prohibits logging out via camping.");
				}
				else if (m_From.Criminal)
				{
					m_From.SendMessage("You are criminal, so you cannot logout via camping.");
				}
				else
				{
					//state.Dispose();
					state.Send(new LogoutAck());

					//manually logout: set logout location and logout map and move to internal map
					m_From.LogoutLocation = m_From.Location;
					m_From.LogoutMap = m_From.Map;
					m_From.Map = Map.Internal;
				}
			}
		}

		private class CampLogoutGumpTimeoutTimer : Timer
		{
			private Mobile m_Player;

			public CampLogoutGumpTimeoutTimer(Mobile m)
				: base(TimeSpan.FromSeconds(20.0))
			{
				m_Player = m;
			}

			protected override void OnTick()
			{
				m_Player.CloseGump(typeof(CampLogoutGump));
				Stop();
			}
		}

	}
}

#endif