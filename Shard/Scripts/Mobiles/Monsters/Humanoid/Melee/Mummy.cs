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

/* Scripts/Mobiles/Monsters/Humanoid/Melee/Mummy.cs
 * ChangeLog
 *  8/16/06, Rhiannon
 *		Changed speed settings to match SpeedInfo table.
 *	7/26/05, erlein
 *		Automated removal of AoS resistance related function calls. 7 lines removed.
 *	4/13/05, Kit
 *		Switch to new region specific loot model
 *	12/11/04, Pix
 *		Changed ControlSlots for IOBF.
 *  11/16/04, Froste
 *      Added IOBAlignment=IOBAlignment.Undead, added the random IOB drop to loot
 *  6/5/04, Pix
 *		Merged in 1.0RC0 code.
 */

using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Engines.IOBSystem;

namespace Server.Mobiles
{
	[CorpseName("a mummy corpse")]
	public class Mummy : BaseCreature
	{
		[Constructable]
		public Mummy()
			: base(AIType.AI_Melee, FightMode.All | FightMode.Closest, 10, 1, 0.3, 0.6)
		{
			Name = "a mummy";
			Body = 154;
			BaseSoundID = 471;
			IOBAlignment = IOBAlignment.Undead;
			ControlSlots = 2;

			SetStr(346, 370);
			SetDex(71, 90);
			SetInt(26, 40);

			SetHits(208, 222);

			SetDamage(13, 23);

			SetSkill(SkillName.MagicResist, 15.1, 40.0);
			SetSkill(SkillName.Tactics, 35.1, 50.0);
			SetSkill(SkillName.Wrestling, 35.1, 50.0);

			Fame = 4000;
			Karma = -4000;

			VirtualArmor = 50;
		}

		public override Poison PoisonImmune { get { return Poison.Lesser; } }

		public Mummy(Serial serial)
			: base(serial)
		{
		}

		public override void GenerateLoot()
		{
			if (Core.UOAI || Core.UOAR)
			{
				PackGem();
				PackPotion();
				PackScroll(4, 7);
				PackItem(new Garlic(5));
				PackItem(new Bandage(10));
				PackGold(150, 250);
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
				{	// http://web.archive.org/web/20020213040515/uo.stratics.com/hunters/mummy.shtml
					// 50 to 150 Gold, Potions, Gems, Scrolls (circle 4-7), Garlic, Bandages
					if (Spawning)
					{
						PackGold(50, 150);
					}
					else
					{
						PackPotion();
						PackPotion(.2);
						PackGem(1, .9);
						PackGem(1, .5);
						PackScroll(4, 7);
						PackScroll(4, 7, .5);
						PackItem(new Garlic(5));
						PackItem(new Bandage(10));
					}
				}
				else
				{
					if (Spawning)
					{
						PackItem(new Garlic(5));
						PackItem(new Bandage(10));
					}

					AddLoot(LootPack.Rich);
					AddLoot(LootPack.Gems);
					AddLoot(LootPack.Potions);
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
