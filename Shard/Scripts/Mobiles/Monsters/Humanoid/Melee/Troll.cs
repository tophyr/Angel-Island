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

/* Scripts/Mobiles/Monsters/Humanoid/Melee/Troll.cs
 * ChangeLog
 *  8/16/06, Rhiannon
 *		Changed speed settings to match SpeedInfo table.
 *	7/26/05, erlein
 *		Automated removal of AoS resistance related function calls. 6 lines removed.
 *  6/5/04, Pix
 *		Merged in 1.0RC0 code.
 */

using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName("a troll corpse")]
	public class Troll : BaseCreature
	{
		[Constructable]
		public Troll()
			: base(AIType.AI_Melee, FightMode.All | FightMode.Closest, 10, 1, 0.25, 0.5)
		{
			Name = "a troll";
			Body = Utility.RandomList(53, 54);
			BaseSoundID = 461;

			SetStr(176, 205);
			SetDex(46, 65);
			SetInt(46, 70);

			SetHits(106, 123);

			SetDamage(8, 14);

			SetSkill(SkillName.MagicResist, 45.1, 60.0);
			SetSkill(SkillName.Tactics, 50.1, 70.0);
			SetSkill(SkillName.Wrestling, 50.1, 70.0);

			Fame = 3500;
			Karma = -3500;

			VirtualArmor = 40;
		}

		public override bool CanRummageCorpses { get { return Core.UOAI || Core.UOAR ? true : true; } }
		public override int TreasureMapLevel { get { return Core.UOAI || Core.UOAR ? 1 : 1; } }
		public override int Meat { get { return 2; } }

		public Troll(Serial serial)
			: base(serial)
		{
		}

		public override void GenerateLoot()
		{
			if (Core.UOAI || Core.UOAR)
				PackGold(50, 100);
			else
			{
				if (Core.UOSP || Core.UOMO)
				{	// http://web.archive.org/web/20021215174532/uo.stratics.com/hunters/troll.shtml
					// 50 to 200 Gold, Potions, Arrows, Weapon Carried, 2 Raw Ribs (carved), Level 1 Treasure Maps

					if (Spawning)
					{
						PackGold(50, 200);
					}
					else
					{
						PackPotion();
						PackPotion(0.5);
						PackItem(new Arrow(Utility.Random(1, 3)));

						if (Body == 53)
							PackItem(new LargeBattleAxe());
					}

				}
				else
				{
					AddLoot(LootPack.Average);
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
