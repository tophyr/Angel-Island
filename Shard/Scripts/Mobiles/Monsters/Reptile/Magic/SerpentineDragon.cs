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

/* Scripts/Mobiles/Monsters/Reptile/Magic/SerpentineDragon.cs
 * ChangeLog
 *  8/16/06, Rhiannon
 *		Changed speed settings to match SpeedInfo table.
 *	7/26/05, erlein
 *		Automated removal of AoS resistance related function calls. 7 lines removed.
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
	[CorpseName("a dragon corpse")]
	public class SerpentineDragon : BaseCreature
	{
		[Constructable]
		public SerpentineDragon()
			: base(AIType.AI_Mage, FightMode.Aggressor | FightMode.Evil, 10, 1, 0.25, 0.5)
		{
			Name = "a serpentine dragon";
			Body = 103;
			BaseSoundID = 362;

			SetStr(111, 140);
			SetDex(201, 220);
			SetInt(1001, 1040);

			SetHits(480);

			SetDamage(5, 12);

			SetSkill(SkillName.EvalInt, 100.1, 110.0);
			SetSkill(SkillName.Magery, 110.1, 120.0);
			SetSkill(SkillName.Meditation, 100.0);
			SetSkill(SkillName.MagicResist, 100.0);
			SetSkill(SkillName.Tactics, 50.1, 60.0);
			SetSkill(SkillName.Wrestling, 30.1, 100.0);

			Fame = 15000;
			Karma = 15000;

			VirtualArmor = 36;
		}

		public override int GetIdleSound()
		{
			return 0x2C4;
		}

		public override int GetAttackSound()
		{
			return 0x2C0;
		}

		public override int GetDeathSound()
		{
			return 0x2C1;
		}

		public override int GetAngerSound()
		{
			return 0x2C4;
		}

		public override int GetHurtSound()
		{
			return 0x2C3;
		}

		public override bool HasBreath { get { return true; } } // fire breath enabled
		// Auto-dispel is UOR - http://forums.uosecondage.com/viewtopic.php?f=8&t=6901
		public override bool AutoDispel { get { return Core.UOAI || Core.UOAR ? false : Core.PublishDate >= Core.EraREN ? true : false; } }
		public override HideType HideType { get { return HideType.Barbed; } }
		public override int Hides { get { return 40; } }
		public override int Meat { get { return 20; } }
		public override int Scales { get { return (Core.UOAI || Core.UOAR || Core.PublishDate < Core.PlagueOfDespair) ? 0 : 6; } }
		public override ScaleType ScaleType { get { return (Utility.RandomBool() ? ScaleType.Black : ScaleType.White); } }
		public override int TreasureMapLevel { get { return Core.UOAI || Core.UOAR ? 4 : 0; } }

		public SerpentineDragon(Serial serial)
			: base(serial)
		{
		}

		public override void OnGotMeleeAttack(Mobile attacker)
		{
			base.OnGotMeleeAttack(attacker);

			if (0.25 >= Utility.RandomDouble() && attacker is BaseCreature)
			{
				BaseCreature c = (BaseCreature)attacker;

				if (c.Controlled && c.ControlMaster != null)
				{
					c.ControlTarget = c.ControlMaster;
					c.ControlOrder = OrderType.Attack;
					c.Combatant = c.ControlMaster;
				}
			}
		}

		public override void GenerateLoot()
		{
			if (Core.UOAI || Core.UOAR)
			{
				PackGem();
				PackGem();
				PackGold(800, 900);
				PackMagicEquipment(1, 3, 0.50, 0.50);
				PackMagicEquipment(1, 3, 0.15, 0.15);
				// Category 4 MID
				PackMagicItem(2, 3, 0.10);
				PackMagicItem(2, 3, 0.05);
				PackMagicItem(2, 3, 0.02);
			}
			else
			{
				if (Core.UOSP || Core.UOMO)
				{	// http://web.archive.org/web/20020202092707/uo.stratics.com/hunters/serpentinedragon.shtml
					// 1500 - 2000 Gold, gems, scrolls, magic items
					if (Spawning)
					{
						PackGold(1500, 2000);
					}
					else
					{
						PackGem(1, .9);
						PackGem(1, .6);
						PackGem(1, .2);
						PackScroll(3, 7);
						PackScroll(3, 7, 0.8);
						PackScroll(3, 7, 0.3);

						if (Utility.RandomBool())
							PackMagicEquipment(1, 3);
						else
							PackMagicItem(2, 3, 0.10);
					}
				}
				else
				{	// Standard RunUO
					AddLoot(LootPack.FilthyRich, 2);
					AddLoot(LootPack.Gems, 2);
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
