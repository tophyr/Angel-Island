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

/* Scripts/Mobiles/Monsters/Humanoid/Melee/SavageWitchDoctor.cs
 *	ChangeLog:
 *  7/02/06, Kit
 *		InitBody/InitOutfit additions
 *	7/26/05, erlein
 *		Automated removal of AoS resistance related function calls. 1 lines removed.
  *	4/13/05, Kit
 *		Switch to new region specific loot model
 *	1/16/05, Adam
 *		level 5 mob balance
 *			Change SetDamage from 8, 16 to 14, 18
 *			16 seems to be the magic number where can compete with other lvl5s
 *	1/15/05, Adam
 *		first time checkin
 *		Improve loot
 *		Remove drop of 'plain' mask
 */

using System;
using Server;
using Server.Misc;
using Server.Items;
using Server.Engines.IOBSystem;

namespace Server.Mobiles
{
	[CorpseName("a witch doctor's corpse")]
	public class SavageWitchDoctor : BaseCreature
	{
		[Constructable]
		public SavageWitchDoctor()
			: base(AIType.AI_Mage, FightMode.All | FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Title = "the witch doctor";

			IOBAlignment = IOBAlignment.Savage;
			ControlSlots = 5;

			SetStr(416, 505);
			SetDex(146, 165);
			SetInt(566, 655);

			SetHits(250, 303);

			SetDamage(14, 18);

			SetSkill(SkillName.EvalInt, 80.1, 90.1);
			SetSkill(SkillName.Magery, 80.1, 90.1);
			SetSkill(SkillName.MagicResist, 57.5, 80.0);
			SetSkill(SkillName.Fencing, 60.0, 82.5);
			SetSkill(SkillName.Tactics, 60.0, 82.5);

			PackItem(new Bandage(Utility.RandomMinMax(1, 15)));

			Fame = 15000;
			Karma = -15000;

			InitBody();
			InitOutfit();

			VirtualArmor = 40;
		}

		public override int Meat { get { return 1; } }
		public override bool AlwaysMurderer { get { return true; } }
		public override bool ShowFameTitle { get { return false; } }
		public override bool CanRummageCorpses { get { return Core.UOAI || Core.UOAR ? true : false; } }

		public override bool CanBandage { get { return true; } }
		// since we have bandages AND magic, double bandage time from normal
		public override TimeSpan BandageDelay { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(10 * 2, 13 * 2)); } }

		public override void InitBody()
		{
			Name = NameList.RandomName("savage");

			if (Female = Utility.RandomBool())
				Body = 184;
			else
				Body = 183;
		}
		public override void InitOutfit()
		{
			WipeLayers();
			Item hair = new LongHair();
			hair.Hue = 0x47E;
			hair.Layer = Layer.Hair;
			hair.Movable = false;
			AddItem(hair);

			Item Necklace = new GoldNecklace();
			Necklace.LootType = LootType.Newbied;
			Necklace.Hue = 2006; // basilisk hue
			AddItem(Necklace);

			// NEVER drop one of these! - (never runs out of poison)
			BasilisksFang fang = new BasilisksFang();
			fang.Quality = WeaponQuality.Exceptional;
			fang.LootType = LootType.Newbied;
			if (Utility.RandomBool())
				fang.Poison = Poison.Regular;
			else
				fang.Poison = Poison.Lesser;
			fang.PoisonCharges = 30;
			fang.Movable = false;
			AddItem(fang);

			AddItem(new BoneArms());
			AddItem(new BoneLegs());

			// all savages wear a mask, but won't drop this one
			//	see OnBeforeDeath for the mask they do drop
			SavageMask mask = new SavageMask();
			mask.LootType = LootType.Newbied;
			AddItem(mask);

		}

		public override void AlterMeleeDamageTo(Mobile to, ref int damage)
		{
			//if ( to is Dragon || to is WhiteWyrm || to is SwampDragon || to is Drake || to is Nightmare || to is Daemon )
			//	damage *= 5;
		}

		public SavageWitchDoctor(Serial serial)
			: base(serial)
		{
		}

		public override void GenerateLoot()
		{
			if (Core.UOAI || Core.UOAR)
			{
				PackMagicEquipment(1, 3, 0.80, 0.80);
				PackMagicEquipment(1, 3, 0.10, 0.10);
				PackGold(600, 700);

				// Category 3 MID
				PackMagicItem(1, 2, 0.10);
				PackMagicItem(1, 2, 0.05);

				// berry or bola
				if (Female)
				{
					if (Utility.RandomDouble() < 0.30)
						PackItem(new TribalBerry());
				}
				else
				{
					if (Utility.RandomDouble() < 0.05)
						PackItem(new BolaBall());
				}

				// Froste: 12% random IOB drop
				if (0.12 > Utility.RandomDouble())
				{
					Item iob = Loot.RandomIOB();
					PackItem(iob);
				}

				if (IOBRegions.GetIOBStronghold(this) == IOBAlignment)
				{
					// 30% boost to gold
					PackGold(base.GetGold() / 3);
				}
			}
			else
			{
				if (Core.UOSP || Core.UOMO)
				{	// ai special creature
					if (Spawning)
					{
						PackGold(0);
					}
					else
					{
					}
				}
				else
				{
					// ai special creature
				}
			}
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
