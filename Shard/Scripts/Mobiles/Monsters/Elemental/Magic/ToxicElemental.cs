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

/* Scripts/Mobiles/Monsters/Elemental/Magic/ToxicElemental.cs
 * ChangeLog
 *  8/16/06, Rhiannon
 *		Changed speed settings to match SpeedInfo table.
 *	7/26/05, erlein
 *		Automated removal of AoS resistance related function calls. 8 lines removed.
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
	[CorpseName("an acid elemental corpse")]
	public class ToxicElemental : BaseCreature
	{
		[Constructable]
		public ToxicElemental()
			: base(AIType.AI_Mage, FightMode.All | FightMode.Closest, 10, 1, 0.25, 0.5)
		{
			Name = "an acid elemental";
			Body = 0x9E;
			BaseSoundID = 278;

			SetStr(326, 355);
			SetDex(66, 85);
			SetInt(271, 295);

			SetHits(196, 213);

			SetDamage(9, 15);

			SetSkill(SkillName.Anatomy, 30.3, 60.0);
			SetSkill(SkillName.EvalInt, 70.1, 85.0);
			SetSkill(SkillName.Magery, 70.1, 85.0);
			SetSkill(SkillName.MagicResist, 60.1, 75.0);
			SetSkill(SkillName.Tactics, 80.1, 90.0);
			SetSkill(SkillName.Wrestling, 70.1, 90.0);

			Fame = 10000;
			Karma = -10000;

			VirtualArmor = 40;
		}

		public override Poison HitPoison { get { return Poison.Lethal; } }
		public override double HitPoisonChance { get { return 0.6; } }

		public override int TreasureMapLevel { get { return Core.UOAI || Core.UOAR ? 3 : 0; } }

		public ToxicElemental(Serial serial)
			: base(serial)
		{
		}

		public override void GenerateLoot()
		{
			if (Core.UOAI || Core.UOAR)
			{
				Katana kat = new Katana();					// Adam: why do we have an unmovable katana?
				kat.Movable = false;
				kat.Crafter = this;
				kat.Quality = WeaponQuality.Exceptional;
				AddItem(kat);

				PackPotion();
				PackGold(200, 250);
				PackScroll(1, 6);
				PackScroll(1, 6);
				PackMagicEquipment(1, 2, 0.20, 0.20);

				// Category 2 MID
				PackMagicItem(1, 1, 0.05);
			}
			else
			{	// http://web.archive.org/web/20021015004725/uo.stratics.com/hunters/acidelemental.shtml
				//	250 - 650 Gold, Magic Items, Gems, Potions, Scrolls
				if (Core.UOSP || Core.UOMO)
				{
					if (Spawning)
					{
						PackGold(250, 650);
					}
					else
					{
						PackMagicStuff(1, 1, 0.05);
						PackGem();
						PackPotion();
						PackScroll(1, 6, .9);
						PackScroll(1, 6, .5);
					}
				}
				else
				{	// runuo standard loot
					AddLoot(LootPack.Rich);
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

			if (BaseSoundID == 263)
				BaseSoundID = 278;

			if (Body == 13)
				Body = 0x9E;

			if (Hue == 0x4001)
				Hue = 0;
		}
	}
}
