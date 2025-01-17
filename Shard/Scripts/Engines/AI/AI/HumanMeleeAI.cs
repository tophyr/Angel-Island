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

/* Scripts/Engines/AI/AI/HumanMageAI.cs
 * CHANGELOG
 *	7/11/10, adam
 *		o major reorganization of AI
 *			o push most smart-ai logic from the advanced magery classes down to baseAI so that we can use potions and bandages from 
 *				the new advanced melee class
 *	1/1/09, Adam
 *		Add new Serialization model for creature AI.
 *		BaseCreature will already be serializing this data when I finish the upgrade, so all you need to do is add your data to serialize. 
 *		Make sure to use the SaveFlags to optimize saves by not writing stuff that is non default.
 *	12/30/08, Adam
 *		Make sure equipped weapons are dropped and reequipped if we are drinking a potion and 
 *			Pot.RequireFreeHand && BasePotion.HasFreeHand(m) == false
 *	12/28/08, Adam
 *		Redesign to work with the all new HybridAI (add PreferMagic() method)
 *  10/31/06, Kit
 *		Fixed bug with trapped pouchs and them being stoled(cancel spell target if item doesnt exsist)
 *  08/13/06, Kit
 *		Various tweaks, added new expermintal movement code RunAround()
 *  12/10/05, Kit
 *		Various tweaks, added in HealPot usage if potions available, check for nox spells if target is poisonable.
 *  11/07/05, Kit
 *		Moved GetPackItems funtion to BaseAI
 *  6/05/05, Kit
 *		Fixed problem with when at below 15 mana mobile would no longer run, but only stand still and attempt to heal
 *  5/30/05, Kit
 *		Initial Creation
 *		HumanMageAI always uses combos, traps pouchs and drinks pots when available,
 *		Casts magic reflect or reactive armor, takes down reflect on enemys before dumping.
 */
using System;
using System.Collections;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Spells;
using Server.Spells.First;
using Server.Spells.Second;
using Server.Spells.Third;
using Server.Spells.Fourth;
using Server.Spells.Fifth;
using Server.Spells.Sixth;
using Server.Spells.Seventh;
using Server.Misc;
using Server.Regions;
using Server.SkillHandlers;
using CalcMoves = Server.Movement.Movement;
using MoveImpl = Server.Movement.MovementImpl;

namespace Server.Mobiles
{
	public class HumanMeleeAI : MeleeAI
	{
		private bool m_EnemyCountersPara;
		private bool m_RegainingMana;

		public bool EnemyCountersPara { get { return m_EnemyCountersPara; } set { m_EnemyCountersPara = value; } }
		public bool RegainingMana { get { return m_RegainingMana; } set { m_RegainingMana = value; } }

		public override bool SmartAI
		{
			get { return true; }
		}

		public HumanMeleeAI(BaseCreature m)
			: base(m)
		{
			DmgSlowsMovement = false;
			CanRun = true;
			UsesPotions = true;
			CanReveal = true;
			UsesBandages = true;
		}

		public override bool DoActionCombat()
		{
			m_Mobile.DebugSay("doing HumanMeleeAI combat action");
			Mobile c = m_Mobile.Combatant;
			m_Mobile.Warmode = true;


			if (c == null || c.Deleted || !c.Alive || c.IsDeadBondedPet || !m_Mobile.CanSee(c) || !m_Mobile.CanBeHarmful(c, false) || c.Map != m_Mobile.Map)
			{
				// Our combatant is deleted, dead, hidden, or we cannot hurt them
				// Try to find another combatant

				if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
				{
					if (m_Mobile.Debug)
						m_Mobile.DebugSay("Something happened to my combatant, so I am going to fight {0}", m_Mobile.FocusMob.Name);

					m_Mobile.Combatant = c = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
				else
				{
					m_Mobile.DebugSay("Something happened to my combatant, and nothing is around. I am on guard.");
					Action = ActionType.Guard;
					return true;
				}
			}

			if (!m_Mobile.InLOS(c))
			{
				if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
				{
					m_Mobile.Combatant = c = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
			}

			if (SmartAI && !m_Mobile.StunReady && m_Mobile.Skills[SkillName.Wrestling].Value >= 80.0 && m_Mobile.Skills[SkillName.Anatomy].Value >= 80.0)
				EventSink.InvokeStunRequest(new StunRequestEventArgs(m_Mobile));

			if (!m_Mobile.InRange(c, m_Mobile.RangePerception))
			{
				// They are somewhat far away, can we find something else?

				if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
				{
					m_Mobile.Combatant = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
				else if (!m_Mobile.InRange(c, m_Mobile.RangePerception * 3))
				{
					m_Mobile.Combatant = null;
				}

				c = m_Mobile.Combatant;

				if (c == null)
				{
					m_Mobile.DebugSay("My combatant has fled, so I am on guard");
					Action = ActionType.Guard;

					return true;
				}
			}

			if (m_Mobile.Hits < m_Mobile.HitsMax * 20 / 100)
			{
				// We are low on health, should we flee?

				bool flee = false;

				if (m_Mobile.Hits < c.Hits)
				{
					// We are more hurt than them

					int diff = c.Hits - m_Mobile.Hits;

					flee = (Utility.Random(0, 100) > (10 + diff)); // (10 + diff)% chance to flee
				}
				else
				{
					flee = Utility.Random(0, 100) > 10; // 10% chance to flee
				}

				if (flee)
				{
					if (m_Mobile.Debug)
						m_Mobile.DebugSay("I am going to flee from {0}", c.Name);

					Action = ActionType.Flee;
					return true;
				}
			}

			//try an cure with a pot first if the poison is serious or where in the middle of dumping
			if (UsesPotions && CurePotCount >= 1 && (m_Mobile.Poisoned && m_Mobile.Poison.Level >= 3))
				DrinkCure(m_Mobile);

			// start a bandage now, even though we will likely be drinking a pot
			if (UsesBandages && BandageCount >= 1 && (IsDamaged || IsPoisoned) && m_Mobile.Skills.Healing.Base > 20.0)
			{
				TimeSpan ts = TimeUntilBandage;
				if (ts == TimeSpan.MaxValue)
					StartBandage(m_Mobile, m_Mobile);
			}

			// at 20% life, start using potions
			if (UsesPotions && HealPotCount >= 1 && !m_Mobile.Poisoned && (double)m_Mobile.Hits < m_Mobile.HitsMax * .20)
				DrinkHeal(m_Mobile);

			// I don't know, need a better test here
			if (UsesPotions && AgilityPotCount >= 1 && Utility.RandomChance(2))
				DrinkAgility(m_Mobile);

			// I don't know, need a better test here
			if (UsesPotions && StrengthPotCount >= 1 && Utility.RandomChance(2))
				DrinkStrength(m_Mobile);

			// at 80% stam, drink up!
			if (UsesPotions && RefreshPotCount >= 1 && (double)m_Mobile.Stam < m_Mobile.StamMax * .80)
				DrinkRefresh(m_Mobile);

			RunTo(c, CanRun);

			return true;
		}

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

