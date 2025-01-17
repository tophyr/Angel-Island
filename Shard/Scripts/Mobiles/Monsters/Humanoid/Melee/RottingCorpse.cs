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

/* Scripts/Mobiles/Monsters/Humanoid/Melee/RottingCorpse.cs
 * ChangeLog
 *  8/16/06, Rhiannon
 *		Changed speed settings to match SpeedInfo table.
 *	7/26/05, erlein
 *		Automated removal of AoS resistance related function calls. 8 lines removed.
 *	4/13/05, Kit
 *		Switch to new region specific loot model
 *	12/11/04, Pix
 *		Changed ControlSlots for IOBF.
 *  11/16/04, Froste
 *      Added IOBAlignment=IOBAlignment.Undead, added the random IOB drop to loot
 *  9/20/04, Jade
 *      Increased gold drop from (200, 400) to (350, 500)
 *	7/6/04, Adam
 *		1. implement Jade's new Category Based Drop requirements
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
	[CorpseName("a zombie corpse")] // ?
	public class RottingCorpse : BaseCreature
	{
		[Constructable]
		public RottingCorpse()
			: base(AIType.AI_Melee, FightMode.All | FightMode.Closest, 10, 1, 0.3, 0.6)
		{
			Name = "a rotting corpse";
			Body = 155;
			BaseSoundID = 471;
			IOBAlignment = IOBAlignment.Undead;
			ControlSlots = 5;

			SetStr(301, 350);
			SetDex(75);
			SetInt(151, 200);

			SetHits(1200);
			SetStam(150);
			SetMana(0);

			SetDamage(8, 10);

			SetSkill(SkillName.Poisoning, 120.0);
			SetSkill(SkillName.MagicResist, 250.0);
			SetSkill(SkillName.Tactics, 100.0);
			SetSkill(SkillName.Wrestling, 90.1, 100.0);

			Fame = 6000;
			Karma = -6000;

			VirtualArmor = 40;
		}

		public override Poison PoisonImmune { get { return Poison.Lethal; } }

		public override Poison HitPoison { get { return Poison.Lethal; } }

		public RottingCorpse(Serial serial)
			: base(serial)
		{
		}

		public override void GenerateLoot()
		{
			if (Core.UOAI || Core.UOAR)
			{
				PackGold(350, 500);
				PackMagicEquipment(1, 3, 0.30, 0.30);
				PackMagicEquipment(1, 3, 0.10, 0.10);
				// Category 2 MID
				PackMagicItem(1, 1, 0.05);

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
				{	// http://web.archive.org/web/20021015001310/uo.stratics.com/hunters/rottingcorpse.shtml
					// 300 - 600 Gold, Magic Items

					if (Spawning)
					{
						PackGold(300, 600);
					}
					else
					{
						if (Utility.RandomBool())		// TODO: no idea as to the level and rate
							PackMagicEquipment(2, 3);
						else
							PackMagicItem(2, 3, 0.30);
					}
				}
				else
				{
					AddLoot(LootPack.FilthyRich, 2);
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
