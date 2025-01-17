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

/* Scripts/Skills/Stealth.cs
 * ChangeLog
 *	7/13/04 changes by Old Salty
 *		wearing only plain leather armor now increases the amount of armor you can wear while stealthing. (secret benefit for players to find out)
 *	7/11/04 changes by Old Salty
 * 		modified OnUse so that armor is taken into account.
 *	4/11/04 changes by mith
 *		modified CheckSkill to use max of 100 skill rather than 120.
 */

using System;
using Server.Items;

namespace Server.SkillHandlers
{
	public class Stealth
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.Stealth].Callback = new SkillUseCallback(OnUse);
		}

		public static TimeSpan OnUse(Mobile m)
		{
			int Armor = (int)Math.Round(m.ArmorRating);  		//the "displayed" armor rating
			double stealth = m.Skills[SkillName.Stealth].Value; //stealth skill
			int AllowedArmor = (int)(stealth / 100 * 21);			//armor allowed at current stealth skill

			//What sort of armor the player has on
			Item helm = (Item)m.FindItemOnLayer(Layer.Helm);
			Item gorget = (Item)m.FindItemOnLayer(Layer.Neck);
			Item chest = (Item)m.FindItemOnLayer(Layer.InnerTorso);
			Item arms = (Item)m.FindItemOnLayer(Layer.Arms);
			Item legs = (Item)m.FindItemOnLayer(Layer.Pants);
			Item gloves = (Item)m.FindItemOnLayer(Layer.Gloves);

			//If the player is wearing any non-plain-leather armor, decrese AllowedArmor to max of 15
			if ((helm != null && (helm is BaseArmor) && !(helm is LeatherCap)) ||
					(gorget != null && (gorget is BaseArmor) && !(gorget is LeatherGorget)) ||
					(arms != null && (arms is BaseArmor) && !(arms is LeatherArms)) ||
					(chest != null && (chest is BaseArmor) && !(chest is LeatherChest || chest is LeatherBustierArms)) ||
					(legs != null && (legs is BaseArmor) && !(legs is LeatherLegs || legs is LeatherShorts || legs is LeatherSkirt)) ||
					(gloves != null && (gloves is BaseArmor) && !(gloves is LeatherGloves)))
			{
				AllowedArmor = (int)(stealth / 100 * 15);
			}

			//			debug messages			 
			//			m.Say( "armor: " + Armor.ToString()  );				
			//			m.Say( "allowed: " + AllowedArmor.ToString()  );

			if (!m.Hidden)
			{
				m.SendLocalizedMessage(502725); // You must hide first
			}
			else if (m.Skills[SkillName.Hiding].Base < 80.0)
			{
				m.SendLocalizedMessage(502726); // You are not hidden well enough.  Become better at hiding.
			}
			else if (Armor > AllowedArmor) 	//If wearing too much armor for stealth level - always fail, no gain
			{
				m.SendMessage("You are encumbered by too much armor!");
				m.SendLocalizedMessage(502731); // You fail in your attempt to move unnoticed.
				m.RevealingAction();
			}
			else if (((int)stealth - (Armor * 2)) >= 75)   //If not wearing enough armor for stealth level - always succeed, no gain
			{

				int steps = (int)(m.Skills[SkillName.Stealth].Value / (Core.AOS ? 5.0 : 10.0));

				if (steps < 1)
					steps = 1;

				m.AllowedStealthSteps = steps;

				m.SendMessage("You slip into the shadows with ease.");
				m.SendLocalizedMessage(502730); // You begin to move quietly.

				return TimeSpan.FromSeconds(10.0);
			}
			else if (m.CheckSkill(SkillName.Stealth, (stealth - Armor * 2) / 100))  //wearing an appropriate level of armor - skillcheck processed, can gain
			{
				int steps = (int)(m.Skills[SkillName.Stealth].Value / (Core.AOS ? 5.0 : 10.0));

				if (steps < 1)
					steps = 1;

				m.AllowedStealthSteps = steps;

				m.SendLocalizedMessage(502730); // You begin to move quietly.

				return TimeSpan.FromSeconds(10.0);
			}
			else
			{
				m.SendLocalizedMessage(502731); // You fail in your attempt to move unnoticed.
				m.RevealingAction();
			}

			return TimeSpan.FromSeconds(10.0);
		}
	}
}
