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

/* Scripts/Mobiles/Monsters/Humanoid/Melee/OrcishLord.cs
 * ChangeLog
 *  8/16/06, Rhiannon
 *		Changed speed settings to match SpeedInfo table.
 *	7/26/05, erlein
 *		Automated removal of AoS resistance related function calls. 6 lines removed.
 *	4/13/05, Kit
 *		Switch to new region specific loot model
 *	1/1/05, Adam
 *		Changed ControlSlots for IOBF to 2 from 1.
 *	12/11/04, Pix
 *		Changed ControlSlots for IOBF.
 *  11/10/04, Froste
 *      Implemented new random IOB drop system and changed drop change to 12%
 *	11/05/04, Pigpen
 *		Made changes for Implementation of IOBSystem. Changes include:
 *		Removed IsEnemy and Aggressive Action Checks. These are now handled in BaseCreature.cs
 *		Set Creature IOBAlignment to Orcish.
 *	9/19/04, Adam
 *		Add IOB drop 5%
 *  9/16/04, Pigpen
 * 		Added IOB Functionality to item OrcishKinHelm
 *  6/5/04, Pix
 *		Merged in 1.0RC0 code.
 */

using System;
using System.Collections;
using Server.Misc;
using Server.Items;
using Server.Targeting;
using Server.Engines.IOBSystem;

namespace Server.Mobiles
{
	[CorpseName("an orcish corpse")]
	public class OrcishLord : BaseCreature
	{
		public override InhumanSpeech SpeechType { get { return InhumanSpeech.Orc; } }

		[Constructable]
		public OrcishLord()
			: base(AIType.AI_Melee, FightMode.All | FightMode.Closest, 10, 1, 0.25, 0.5)
		{
			Name = "an orcish lord";
			Body = 138;
			BaseSoundID = 0x45A;
			IOBAlignment = IOBAlignment.Orcish;
			ControlSlots = 1;

			SetStr(147, 215);
			SetDex(91, 115);
			SetInt(61, 85);

			SetHits(95, 123);

			SetDamage(4, 14);

			SetSkill(SkillName.MagicResist, 70.1, 85.0);
			SetSkill(SkillName.Swords, 60.1, 85.0);
			SetSkill(SkillName.Tactics, 75.1, 90.0);
			SetSkill(SkillName.Wrestling, 60.1, 85.0);

			Fame = 2500;
			Karma = -2500;
		}

		public override bool CanRummageCorpses { get { return Core.UOAI || Core.UOAR ? true : true; } }
		public override int TreasureMapLevel { get { return Core.UOAI || Core.UOAR ? 1 : 0; } }
		public override int Meat { get { return 1; } }

		public override OppositionGroup OppositionGroup
		{
			get { return OppositionGroup.SavagesAndOrcs; }
		}

		public override bool IsEnemy(Mobile m, RelationshipFilter filter)
		{
			if (!Core.UOAI && !Core.UOAR)
				if (m.Player && m.FindItemOnLayer(Layer.Helm) is OrcishKinMask)
					return false;

			return base.IsEnemy(m, filter);
		}

		public override void AggressiveAction(Mobile aggressor, bool criminal)
		{
			base.AggressiveAction(aggressor, criminal);

			if (!Core.UOAI && !Core.UOAR)
			{
				Item item = aggressor.FindItemOnLayer(Layer.Helm);

				if (item is OrcishKinMask)
				{
					AOS.Damage(aggressor, 50, 0, 100, 0, 0, 0);
					item.Delete();
					aggressor.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
					aggressor.PlaySound(0x307);
				}
			}
		}

		public OrcishLord(Serial serial)
			: base(serial)
		{
		}

		public override void GenerateLoot()
		{
			if (Core.UOAI || Core.UOAR)
			{
				switch (Utility.Random(5))
				{
					case 0: PackItem(new Lockpick()); break;
					case 1: PackItem(new MortarPestle()); break;
					case 2: PackItem(new Bottle()); break;
					case 3: PackItem(new RawRibs()); break;
					case 4: PackItem(new Shovel()); break;
				}

				PackItem(new RingmailChest());

				if (0.3 > Utility.RandomDouble())
					PackItem(Loot.RandomPossibleReagent());

				if (0.2 > Utility.RandomDouble())
					PackItem(new BolaBall());

				// Froste: 12% random IOB drop
				if (0.12 > Utility.RandomDouble())
				{
					Item iob = Loot.RandomIOB();
					PackItem(iob);
				}

				PackGold(100, 130);

				if (IOBRegions.GetIOBStronghold(this) == IOBAlignment)
				{
					// 30% boost to gold
					PackGold(base.GetGold() / 3);
				}
			}
			else
			{
				if (Core.UOSP || Core.UOMO)
				{	// http://web.archive.org/web/20020207055138/uo.stratics.com/hunters/orclord.shtml
					// 150 to 300 Gold, Magic items, Gems, Two-Handed Axe, Ringmail Tunic, (Evil) Orc Helm, Thigh Boots, 1 Raw Ribs (carved)
					// note: when you open the page for orcish lord from the 2002 stratics archive, you get this 2001 page

					if (Spawning)
					{
						PackGold(150, 300);
					}
					else
					{
						PackMagicEquipment(1, 1);
						PackMagicItem(1, 1, 0.05);
						PackGem(1, .9);
						PackGem(1, .05);
						PackItem(new TwoHandedAxe());
						PackItem(new RingmailChest());

						// TODO: don't know drop rate. this guy says semi rare, so maybe 1 in 100?
						// http://www.iceweasel.net/wzl_guild/semi/
						PackItem(typeof(EvilOrcHelm), 0.01);

						PackItem(new ThighBoots());
					}
				}
				else
				{
					if (Spawning)
					{
						switch (Utility.Random(5))
						{
							case 0: PackItem(new Lockpick()); break;
							case 1: PackItem(new MortarPestle()); break;
							case 2: PackItem(new Bottle()); break;
							case 3: PackItem(new RawRibs()); break;
							case 4: PackItem(new Shovel()); break;
						}

						PackItem(new RingmailChest());

						if (0.3 > Utility.RandomDouble())
							PackItem(Loot.RandomPossibleReagent());

						// http://www.uoguide.com/Savage_Empire
						// http://uo.stratics.com/secrets/archive/orcsavage.shtml
						// Bola balls have appeared as loot on Orc Bombers. Balls on Bombers are rather common, around a 50/50% chance of getting a ball or not. They are only appearing as loot on bombers.
						if (Core.PublishDate >= Core.EraSAVE)
							if (0.2 > Utility.RandomDouble())
								PackItem(new BolaBall());
					}

					AddLoot(LootPack.Meager);
					AddLoot(LootPack.Average);
					// TODO: evil orc helm
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
