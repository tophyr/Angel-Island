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

/* Scripts/Mobiles/Monsters/Humanoid/Melee/Executioner.cs
 * ChangeLog
 *  8/16/06, Rhiannon
 *		Changed speed settings to match SpeedInfo table.
 *  7/02/06, Kit
 *		InitBody/InitOutfit additions
 *	7/26/05, erlein
 *		Automated removal of AoS resistance related function calls. 6 lines removed.
 *  9/26/04, Jade
 *      Decreased gold drop from (750, 800) to (300, 450)
 *	7/6/04, Adam
 *		1. implement Jade's new Category Based Drop requirements
 *  6/5/04, Pix
 *		Merged in 1.0RC0 code.
 */

using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
	public class Executioner : BaseCreature
	{
		[Constructable]
		public Executioner()
			: base(AIType.AI_Melee, FightMode.All | FightMode.Closest, 10, 1, 0.25, 0.5)
		{
			SpeechHue = Utility.RandomDyedHue();
			Title = "the executioner";
			Hue = Utility.RandomSkinHue();

			if (Core.UOAI || Core.UOAR)
			{
				SetStr(386, 400);
				SetDex(70, 90);
				SetInt(161, 175);

				SetDamage(20, 30);
			}
			else
			{
				SetStr(386, 400);
				SetDex(151, 165);
				SetInt(161, 175);

				SetDamage(8, 10);
			}

			SetSkill(SkillName.Anatomy, 125.0);
			SetSkill(SkillName.Fencing, 46.0, 77.5);
			SetSkill(SkillName.Macing, 35.0, 57.5);
			SetSkill(SkillName.Poisoning, 60.0, 82.5);
			SetSkill(SkillName.MagicResist, 83.5, 92.5);
			SetSkill(SkillName.Swords, 125.0);
			SetSkill(SkillName.Tactics, 125.0);
			SetSkill(SkillName.Lumberjacking, 125.0);

			InitBody();
			InitOutfit();

			Fame = 5000;
			Karma = -5000;

			VirtualArmor = 40;
		}

		public override int Meat { get { return 1; } }
		public override bool AlwaysMurderer { get { return true; } }

		public override void InitBody()
		{
			if (this.Female = Utility.RandomBool())
			{
				Body = 0x191;
				Name = NameList.RandomName("female");
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName("male");
			}
		}
		public override void InitOutfit()
		{
			WipeLayers();
			if (Female)
				AddItem(new Skirt(Utility.RandomNeutralHue()));
			else
				AddItem(new ShortPants(Utility.RandomNeutralHue()));

			Item hair = new Item(Utility.RandomList(0x203B, 0x2049, 0x2048, 0x204A));
			hair.Hue = Utility.RandomNondyedHue();
			hair.Layer = Layer.Hair;
			hair.Movable = false;
			AddItem(hair);

			AddItem(new ThighBoots(Utility.RandomRedHue()));
			AddItem(new Surcoat(Utility.RandomRedHue()));
			AddItem(new ExecutionersAxe());

		}

		public override void OnGaveMeleeAttack(Mobile target)
		{
			if (Core.UOAI || Core.UOAR)
				if (0.25 >= Utility.RandomDouble() && target is PlayerMobile)
					target.Damage(Utility.RandomMinMax(15, 25), this);

			base.OnGaveMeleeAttack(target);
		}

		public Executioner(Serial serial)
			: base(serial)
		{
		}

		public override void GenerateLoot()
		{
			if (Core.UOAI || Core.UOAR)
			{
				PackGold(300, 450);
				// Category 2 MID
				PackMagicItem(1, 1, 0.05);
			}
			else
			{
				if (Core.UOSP || Core.UOMO)
				{	// http://web.archive.org/web/20020606054853/uo.stratics.com/hunters/executioner.shtml
					// 750 - 800 Gold, Magic Items
					if (Spawning)
					{
						PackGold(750, 800);
					}
					else
					{
						PackMagicStuff(1, 1, 0.05);
					}
				}
				else
				{
					AddLoot(LootPack.FilthyRich);
					AddLoot(LootPack.Meager);
				}
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version 
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}
