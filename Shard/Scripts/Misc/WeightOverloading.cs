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

/* Scripts/Misc/WeightOverloading.cs
 * ChangeLog
 *	6/28/04, Pix
 *		Tweaked to only drain low-on-stamina people when they're running.
 *  6/5/04, Pix
 *		Merged in 1.0RC0 code.
 */

using System;
using Server;
using Server.Mobiles;

namespace Server.Misc
{
	public enum DFAlgorithm
	{
		Standard,
		PainSpike
	}

	public class WeightOverloading
	{
		public static void Initialize()
		{
			EventSink.Movement += new MovementEventHandler(EventSink_Movement);
		}

		private static DFAlgorithm m_DFA;

		public static DFAlgorithm DFA
		{
			get { return m_DFA; }
			set { m_DFA = value; }
		}

		public static void FatigueOnDamage(Mobile m, int damage)
		{
			double fatigue = 0.0;

			switch (m_DFA)
			{
				case DFAlgorithm.Standard:
					{
						fatigue = (damage * (100.0 / m.Hits) * ((double)m.Stam / 100)) - 5.0;
						break;
					}
				case DFAlgorithm.PainSpike:
					{
						fatigue = (damage * ((100.0 / m.Hits) + ((50.0 + m.Stam) / 100) - 1.0)) - 5.0;
						break;
					}
			}

			if (fatigue > 0)
				m.Stam -= (int)fatigue;
		}

		public const int OverloadAllowance = 4; // We can be four stones overweight without getting fatigued

		public static int GetMaxWeight(Mobile m)
		{
			return 40 + (int)(3.5 * m.Str);
		}

		public static void EventSink_Movement(MovementEventArgs e)
		{
			Mobile from = e.Mobile;

			if (!from.Player || !from.Alive || from.AccessLevel >= AccessLevel.GameMaster)
				return;

			int maxWeight = GetMaxWeight(from) + OverloadAllowance;
			int overWeight = (Mobile.BodyWeight + from.TotalWeight) - maxWeight;
			bool mounted = from.Mount as BaseMount != null;
			BaseMount mount = from.Mount as BaseMount;

			if (overWeight > 0)
			{
				from.Stam -= GetStamLoss(from, overWeight, (e.Direction & Direction.Running) != 0);

				if (from.Stam == 0)
				{
					from.SendLocalizedMessage(500109); // You are too fatigued to move, because you are carrying too much weight!
					e.Blocked = true;
					return;
				}
			}

			if (!Core.UOAI && !Core.UOAR && !Core.UOMO && !mounted)
			{	// not mounted on siege
				if (((from.Stam * 100) / Math.Max(from.StamMax, 1)) < 10)
					--from.Stam;
			}
			else if (Core.UOAI || Core.UOAR || Core.UOMO)
			{	// mounted ot not on ai/mo
				if ((e.Direction & Direction.Running) != 0) //if you're running
				{
					if (((from.Stam * 100) / Math.Max(from.StamMax, 1)) < 10)
						--from.Stam;
				}
			}

			if (!Core.UOAI && !Core.UOAR && !Core.UOMO && mounted)
			{	// mounted on siege
				if (mount.Stam == 0)
				{
					from.SendLocalizedMessage(500108); // Your mount is too fatigued to move.
					e.Blocked = true;
					return;
				}
			}

			if (from.Stam == 0)
			{
				if (mounted && (!Core.UOAI && !Core.UOAR && !Core.UOMO))
					from.SendLocalizedMessage(500108); // Your mount is too fatigued to move.
				else
					from.SendLocalizedMessage(500110); // You are too fatigued to move.
				e.Blocked = true;
				return;
			}

			if (from is PlayerMobile)
			{
				PlayerMobile pm = from as PlayerMobile;
				++pm.StepsTaken;

				if (Core.UOAI || Core.UOAR || Core.UOMO || !mounted)
				{	// ai, mo, or not mounted on siege
					int amt = (from.Mounted ? 48 : 16);
					if ((pm.StepsTaken % amt) == 0)
						--from.Stam;
				}
				else if (!Core.UOAI && !Core.UOAR && !Core.UOMO && mounted)
				{	//mounted on siege
					if (((pm.StepsTaken % 6) == 0) && ((e.Direction & Direction.Running) != 0))
					{
						// scale riders stamina loss relative to mount so that the status bar reflects something close
						//	to the stamina available to you right now. I believe this is how old school UO looked
						mount.RiderStamAccumulator += ((from.StamMax * 100.0) / mount.StamMax) / 100; // mount stam to player stam
						int whole = (int)mount.RiderStamAccumulator;
						mount.RiderStamAccumulator -= whole;

						if (whole > 0)
							from.Stam -= whole;

						--mount.Stam;

						if (mount.Stam <= 12 && mount.Stam > 1)
						{
							if (NoSpamMessage.Recall(from) == false)
							{	// NoSpamMessage just keeps this message and horse sound from spamming
								NoSpamMessage.Remember(from, 2.8);
								from.SendLocalizedMessage(500133); // Your mount is very fatigued.
								Effects.PlaySound(from, from.Map, mount.GetAngerSound());
							}
						}
					}
				}
			}
		}

		private static Memory NoSpamMessage = new Memory();

		public static int GetStamLoss(Mobile from, int overWeight, bool running)
		{
			int loss = 5 + (overWeight / 25);

			if (from.Mounted)
				loss /= 3;

			if (running)
				loss *= 2;

			return loss;
		}

		public static bool IsOverloaded(Mobile m)
		{
			if (!m.Player || !m.Alive || m.AccessLevel >= AccessLevel.GameMaster)
				return false;

			return ((Mobile.BodyWeight + m.TotalWeight) > (GetMaxWeight(m) + OverloadAllowance));
		}
	}
}