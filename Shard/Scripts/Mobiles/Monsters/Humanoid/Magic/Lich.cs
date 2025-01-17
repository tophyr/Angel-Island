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

/* Scripts/Mobiles/Monsters/Humanoid/Magic/Lich.cs
 * ChangeLog
 *	7/26/05, erlein
 *		Automated removal of AoS resistance related function calls. 8 lines removed.
 *	4/13/05, Kit
 *		Switch to new region specific loot model
 *	12/11/04, Pix
 *		Changed ControlSlots for IOBF.
 *  11/16/04, Froste
 *      Changed IOBAlignment to Council
 *  11/10/04, Froste
 *      Implemented new random IOB drop system and changed drop change to 12%
 *	11/05/04, Pigpen
 *		Made changes for Implementation of IOBSystem. Changes include:
 *		Removed IsEnemy and Aggressive Action Checks. These are now handled in BaseCreature.cs
 *		Set Creature IOBAlignment to Undead.
 *	11/2/04, Adam
 *		Increase gold if this is IOB mobile resides in it's Stronghold (Wind)
 *		Reduce IDWand drop to 10% from 20% and only drop if in Stronghold (Wind)
 *	9/26/04, Adam
 *		Add 5% IOB drop (BloodDrenchedBandana)
 *	7/21/04, mith
 *		IsEnemy() and AggressiveAction() code added to support Brethren property of BloodDrenchedBandana.
 *	7/6/04, Adam
 *		1. implement Jade's new Category Based Drop requirements
 *	7/2/04
 *		Change chance to drop a magic item to 15% 
 *		add a 5% chance for a bonus drop at next intensity level
 *	7/1/04, Adam
 *		Decrease the magic item from 50% to 15%
 * 	6/26/04, Adam
 *		liches carry IDWands. It's historical man!
 *		20% chance to get an IDWand
 *  6/5/04, Pix
 *		Merged in 1.0RC0 code.
 */

using System;
using Server;
using Server.Items;
using Server.Engines.IOBSystem;

namespace Server.Mobiles
{
	[CorpseName("a liche's corpse")]
	public class Lich : BaseCreature
	{
		[Constructable]
		public Lich()
			: base(AIType.AI_Mage, FightMode.All | FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Name = "a lich";
			Body = 24;
			BaseSoundID = 412;
			IOBAlignment = IOBAlignment.Council;
			ControlSlots = 3;

			SetStr(171, 200);
			SetDex(126, 145);
			SetInt(276, 305);

			SetHits(103, 120);

			SetDamage(24, 26);

			SetSkill(SkillName.EvalInt, 100.0);
			SetSkill(SkillName.Magery, 70.1, 80.0);
			SetSkill(SkillName.Meditation, 85.1, 95.0);
			SetSkill(SkillName.MagicResist, 80.1, 100.0);
			SetSkill(SkillName.Tactics, 70.1, 90.0);

			Fame = 8000;
			Karma = -8000;

			VirtualArmor = 50;
		}

		public override bool CanRummageCorpses { get { return Core.UOAI || Core.UOAR ? true : true; } }
		public override Poison PoisonImmune { get { return Poison.Lethal; } }
		public override int TreasureMapLevel { get { return Core.UOAI || Core.UOAR ? 3 : 0; } }

		public Lich(Serial serial)
			: base(serial)
		{
		}

		public override void GenerateLoot()
		{
			if (Core.UOAI || Core.UOAR)
			{
				PackGem();
				PackReg(8, 12);
				PackItem(new GnarledStaff());
				PackScroll(2, 7);
				PackScroll(2, 7);
				PackMagicEquipment(1, 2, 0.25, 0.25);
				PackMagicEquipment(1, 2, 0.05, 0.05);

				// pack the gold
				PackGold(170, 220);

				if (IOBRegions.GetIOBStronghold(this) == IOBAlignment)
				{
					// 30% boost to gold
					PackGold(base.GetGold() / 3);

					if (Utility.RandomDouble() < 0.10)
						PackItem(new IDWand());
				}

				// Froste: 12% random IOB drop
				if (0.12 > Utility.RandomDouble())
				{
					Item iob = Loot.RandomIOB();
					PackItem(iob);
				}

				// Category 2 MID
				PackMagicItem(1, 1, 0.05);
			}
			else
			{
				if (Core.UOSP || Core.UOMO)
				{	// http://web.archive.org/web/20020207053311/uo.stratics.com/hunters/lich.shtml
					// 200 to 250 Gold, Gems, Scrolls (circle 4-7), Reagents, Magic Wand or Staff, Magic items

					if (Spawning)
					{
						PackGold(200, 250);
					}
					else
					{
						PackGem(1, .9);
						PackGem(1, .05);
						PackScroll(4, 7, .9);
						PackScroll(4, 7, .05);
						PackReg(8, 12);
						if (Utility.RandomDouble() < 0.10)
							PackItem(new IDWand());
						else
							PackItem(new GnarledStaff());
						PackMagicEquipment(1, 2);
						PackMagicItem(1, 1, 0.05);
					}
				}
				else
				{
					if (Spawning)
					{
						PackItem(new GnarledStaff());
						if (Core.AOS)
							PackNecroReg(17, 24);
					}

					AddLoot(LootPack.Rich);
					AddLoot(LootPack.MedScrolls, 2);
				}
			}
		}

		public override OppositionGroup OppositionGroup
		{
			get { return OppositionGroup.FeyAndUndead; }
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}
