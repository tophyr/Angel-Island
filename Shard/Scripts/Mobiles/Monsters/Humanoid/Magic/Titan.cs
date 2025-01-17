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

/* Scripts/Mobiles/Monsters/Humanoid/Magic/Titan.cs
 * ChangeLog
 *  8/16/06, Rhiannon
 *		Changed speed settings to match SpeedInfo table.
 *	7/26/05, erlein
 *		Automated removal of AoS resistance related function calls. 6 lines removed.
 *	7/6/04, Adam
 *		1. implement Jade's new Category Based Drop requirements
 *  6/5/04, Pix
 *		Merged in 1.0RC0 code.
 */

using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName("a titans corpse")]
	public class Titan : BaseCreature
	{
		[Constructable]
		public Titan()
			: base(AIType.AI_Mage, FightMode.All | FightMode.Closest, 10, 1, 0.25, 0.5)
		{
			Name = "a titan";
			Body = 76;
			BaseSoundID = 609;

			SetStr(536, 585);
			SetDex(126, 145);
			SetInt(281, 305);

			SetHits(322, 351);

			SetDamage(13, 16);

			SetSkill(SkillName.EvalInt, 85.1, 100.0);
			SetSkill(SkillName.Magery, 85.1, 100.0);
			SetSkill(SkillName.MagicResist, 80.2, 110.0);
			SetSkill(SkillName.Tactics, 60.1, 80.0);
			SetSkill(SkillName.Wrestling, 40.1, 50.0);

			Fame = 11500;
			Karma = -11500;

			VirtualArmor = 40;
		}

		public override int Meat { get { return 4; } }
		public override Poison PoisonImmune { get { return Poison.Regular; } }
		// http://web.archive.org/web/20080803024213/uo.stratics.com/database/view.php?db_content=hunters&id=352
		// this says level 5 map as does RunUO
		public override int TreasureMapLevel { get { return Core.UOAI || Core.UOAR ? 3 : 5; } }

		public Titan(Serial serial)
			: base(serial)
		{
		}

		public override void GenerateLoot()
		{
			if (Core.UOAI || Core.UOAR)
			{
				PackGold(300, 450);
				PackMagicEquipment(1, 2, 0.20, 0.20);
				PackScroll(1, 7);
				// TODO: corn

				// Category 3 MID
				PackMagicItem(1, 2, 0.10);
				PackMagicItem(1, 2, 0.05);
			}
			else
			{
				if (Core.UOSP || Core.UOMO)
				{	// http://web.archive.org/web/20020414130205/uo.stratics.com/hunters/titan.shtml
					// 500 to 800 Gold, Magic Items, Treasure Maps
					if (Spawning)
					{
						PackGold(500, 800);
					}
					else
					{
						if (Utility.RandomBool())
							PackMagicEquipment(1, 2);
						else
							PackMagicItem(1, 2, 0.10);
					}
				}
				else
				{
					AddLoot(LootPack.FilthyRich);
					AddLoot(LootPack.Average);
					AddLoot(LootPack.MedScrolls);
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
