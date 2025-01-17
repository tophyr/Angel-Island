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

/* Scripts\Engines\AI\AI\TaxCollectorAI.cs
 * ChangeLog
 *	8/18/10, adam
 *		Add an OnEvent handler so that the AI can notify us when we recall away
 *  8/7/10, Adam
 *		Initial creation
 */

using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Spells;
using Server.Spells.First;
using Server.Spells.Second;
using Server.Spells.Third;
using Server.Spells.Fourth;
using Server.Spells.Fifth;
using Server.Spells.Sixth;
using Server.Spells.Seventh;

namespace Server.Mobiles
{
	public class TaxCollectorAI : BaseAI
	{
		public TaxCollectorAI(BaseCreature guard)
			: base(guard)
		{

		}

		public override bool Think()
		{
			if (m_Mobile.Deleted)
				return false;

			// remember players in the area, we may want to report them to the king later for failure to pay the taxes due
			if (Utility.RandomChance(25))
				LookAround(m_Mobile.RangePerception);

			if (m_Mobile.Target != null)
			{
				ProcessTarget(m_Mobile.Target);
				return true;
			}
			else
			{
				return base.Think();
			}
		}

		public void ProcessTarget(Target targ)
		{
			if (targ is RecallSpell.InternalTarget)
			{
				m_Mobile.OnEvent(targ);
				m_Mobile.Hidden = true;
				m_Mobile.PlaySound(0x1FC);
				m_Mobile.Delete();
			}
			else
				return;
		}

		public override bool DoActionFlee()
		{
			Mobile from = m_Mobile.FocusMob;

			if (from == null || from.Deleted || from.Hidden || from.Map != m_Mobile.Map)
			{
				m_Mobile.DebugSay("I have lost 'em");
				Action = ActionType.Guard;
				return true;
			}

			if (m_Mobile.GetDistanceToSqrt(from) < 10)
			{
				// move away from the mobile
				Direction d = (m_Mobile.GetDirectionTo(from) - 4) & Direction.Mask;

				// and run
				m_Mobile.Direction = d | Direction.Running;

				if (!DoMove(m_Mobile.Direction, true))
					OnFailedMove();
				else
					m_Mobile.DebugSay("I am fleeing!");
			}
			else
			{
				m_Mobile.DebugSay("I have fled");
				if (m_Mobile.Spell == null)
				{
					m_Mobile.DebugSay("I'm going to try recalling away");
					new RecallSpell(m_Mobile, null).Cast();
				}
			}

			/*
			if (WalkMobileRange(from, 1, true, m_Mobile.RangePerception * 2, m_Mobile.RangePerception * 3))
			{
				m_Mobile.DebugSay("I have fled");
				Action = ActionType.Guard;
				return true;
			}
			else if (m_Mobile.Location == oldLocation)
			{
				OnFailedMove();
			}
			else
			{
				m_Mobile.DebugSay("I am fleeing!");
			}
			 */

			return true;
		}

		public override void OnFailedMove()
		{
			if (m_Mobile.Spell == null)
			{
				m_Mobile.DebugSay("I am stuck, I'm going to try recalling away");
				new RecallSpell(m_Mobile, null).Cast();
			}
		}

		public override bool DoActionWander()
		{
			// New, only flee @ 10%
			double hitPercent = (double)m_Mobile.Hits / m_Mobile.HitsMax;

			if (hitPercent < 0.1) // Less than 10% health
			{
				m_Mobile.DebugSay("I am low on health!");
				Action = ActionType.Flee;
			}
			else if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
			{
				if (m_Mobile.Debug)
					m_Mobile.DebugSay("I have detected {0}, attacking", m_Mobile.FocusMob.Name);

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Flee;
			}
			else
			{
				m_Mobile.DebugSay("I'm fine");

				if (m_Mobile.Combatant != null)
				{
					if (m_Mobile.Debug)
						m_Mobile.DebugSay("{0} is attacking me", m_Mobile.Combatant.Name);

					//m_Mobile.Say(Utility.RandomList(1005305, 501603));
					m_Mobile.FocusMob = m_Mobile.Combatant;
					Action = ActionType.Flee;
				}
				else
				{
					if (m_Mobile.FocusMob != null)
					{
						if (m_Mobile.Debug)
							m_Mobile.DebugSay("{0} has talked to me", m_Mobile.FocusMob.Name);

						Action = ActionType.Interact;
					}
					else
					{
						m_Mobile.Warmode = false;

						base.DoActionWander();
					}
				}
			}

			return true;
		}

		public override bool DoActionInteract()
		{
			Mobile customer = m_Mobile.FocusMob;

			if (m_Mobile.Combatant != null)
			{
				if (m_Mobile.Debug)
					m_Mobile.DebugSay("{0} is attacking me", m_Mobile.Combatant.Name);

				//m_Mobile.Say(Utility.RandomList(1005305, 501603));
				m_Mobile.FocusMob = m_Mobile.Combatant;
				Action = ActionType.Flee;

				return true;
			}

			if (customer == null || customer.Deleted || customer.Hidden || customer.Map != m_Mobile.Map)
			{
				m_Mobile.DebugSay("My customer has disapeared");
				m_Mobile.FocusMob = null;

				Action = ActionType.Wander;
			}
			else
			{
				if (customer.InRange(m_Mobile, m_Mobile.RangeFight))
				{
					if (m_Mobile.Debug)
						m_Mobile.DebugSay("I am with {0}", customer.Name);

					m_Mobile.Direction = m_Mobile.GetDirectionTo(customer);
				}
				else if (m_Mobile.GetDistanceToSqrt(customer) < m_Mobile.RangePerception)
				{	// the customer is close enough for me to be interested...
					if ((int)m_Mobile.GetDistanceToSqrt(customer) > 3)
					{	// and I'd like to get closer
						if (m_Mobile.Home != Point3D.Zero && m_Mobile.GetDistanceToSqrt(m_Mobile.Home) > m_Mobile.RangePerception)
						{	// but they seem to be dragging me from my home
							m_Mobile.DebugSay("{0} is too far away, but I am also too far from home", customer.Name);
							m_Mobile.FocusMob = null;
							Action = ActionType.Wander;
						}
						else
						{	// okay, move closer
							m_Mobile.DebugSay("{0} is too far away, move closer", customer.Name);
							WalkMobileRange(customer, 1, false, 2, 3);
						}
					}
					else
					{
						m_Mobile.DebugSay("I'm chatting with {0}", customer.Name);
					}
				}
				else
				{
					if (m_Mobile.Debug)
						m_Mobile.DebugSay("{0} is gone", customer.Name);

					m_Mobile.FocusMob = null;

					Action = ActionType.Wander;
				}
			}

			return true;
		}

		public override bool DoActionGuard()
		{
			m_Mobile.FocusMob = m_Mobile.Combatant;
			return base.DoActionGuard();
		}

		public override bool HandlesOnSpeech(Mobile from)
		{
			if (from.InRange(m_Mobile, 4))
				return true;

			return base.HandlesOnSpeech(from);
		}

		// Temporary 

		/*public override void OnSpeech(SpeechEventArgs e)
		{
			base.OnSpeech(e);

			Mobile from = e.Mobile;

			if (m_Mobile is BaseVendor && from.InRange(m_Mobile, 4) && !e.Handled)
			{
				string[] SwearingKeywords = new string[2] {"fuck", "damn", "hell", "cunt", "asshole", "bastard", "bitch", "whore", "dick", "jerk", "shit", "bullshit", "ass"};
				for (int ix=0; ix < SwearingKeywords.Length; ix++)
				{
					if (e.Speech.Contains(SwearingKeywords[ix]))
					{
						e.Handled = true;
						m_Mobile.FocusMob = from;
						Action = ActionType.Interact;
						switch (
					}
				}
				
			}
		}*/

		#region Serialize
		private SaveFlags m_flags;

		[Flags]
		private enum SaveFlags
		{	// 0x00 - 0x800 reserved for version
			unused = 0x1000
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			int version = 1;								// current version (up to 4095)
			m_flags = m_flags | (SaveFlags)version;			// save the version and flags
			writer.Write((int)m_flags);

			// add your version specific stuffs here.
			// Make sure to use the SaveFlags for conditional Serialization
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			m_flags = (SaveFlags)reader.ReadInt();				// grab the version an flags
			int version = (int)(m_flags & (SaveFlags)0xFFF);	// maskout the version

			// add your version specific stuffs here.
			// Make sure to use the SaveFlags for conditional Serialization
			switch (version)
			{
				default: break;
			}

		}
		#endregion Serialize
	}
}

