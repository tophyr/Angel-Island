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

/* Scripts/Mobiles/Monsters/Plant/Magic/Reaper.cs
 * ChangeLog
 *	7/26/05, erlein
 *		Automated removal of AoS resistance related function calls. 7 lines removed.
 *  6/5/04, Pix
 *		Merged in 1.0RC0 code.
 */


using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName("a reapers corpse")]
	public class Reaper : BaseCreature
	{
		[Constructable]
		public Reaper()
			: base(AIType.AI_Mage, FightMode.All | FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Name = "a reaper";
			Body = 47;
			BaseSoundID = 442;

			SetStr(66, 215);
			SetDex(66, 75);
			SetInt(101, 250);

			SetHits(40, 129);
			SetStam(0);

			SetDamage(9, 11);

			SetSkill(SkillName.EvalInt, 90.1, 100.0);
			SetSkill(SkillName.Magery, 90.1, 100.0);
			SetSkill(SkillName.MagicResist, 100.1, 125.0);
			SetSkill(SkillName.Tactics, 45.1, 60.0);
			SetSkill(SkillName.Wrestling, 50.1, 60.0);

			Fame = 3500;
			Karma = -3500;

			VirtualArmor = 40;
		}

		public override Poison PoisonImmune { get { return Poison.Greater; } }
		public override int TreasureMapLevel { get { return Core.UOAI || Core.UOAR ? 2 : 0; } }
		public override bool DisallowAllMoves { get { return true; } }

		public Reaper(Serial serial)
			: base(serial)
		{
		}

		public override void GenerateLoot()
		{
			if (Core.UOAI || Core.UOAR)
			{
				PackItem(new Log(10));
				PackGold(0, 50);
			}
			else
			{
				if (Core.UOSP || Core.UOMO)
				{	// http://web.archive.org/web/20021014235628/uo.stratics.com/hunters/reaper.shtml
					// 0 to 150 Gold, 10 Logs

					if (Spawning)
					{
						PackGold(0, 150);
					}
					else
					{
						PackItem(new Log(10));
					}
				}
				else
				{
					if (Spawning)
					{
						PackItem(new Log(10));
						PackItem(new MandrakeRoot(5));
					}

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
