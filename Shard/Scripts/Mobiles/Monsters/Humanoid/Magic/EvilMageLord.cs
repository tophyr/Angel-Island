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

/* Scripts/Mobiles/Monsters/Humanoid/Magic/EvilMageLord.cs
 * ChangeLog
 *  12/03/06 Taran Kain
 *      Set Female = false. No trannies!
 *  8/16/06, Rhiannon
 *		Changed speed settings to match SpeedInfo table.
 *  7/02/06, Kit
 *		InitBody/InitOutfit additions
 *	7/26/05, erlein
 *		Automated removal of AoS resistance related function calls. 6 lines removed.
 *	5/10/05, Adam
 *		Return AI to old AI AI_Mage
 *	5/05/05, Kit
 *		Added in first generation evil mage ai.
 *	4/13/05, Kit
 *		Switch to new region specific loot model
 *	12/11/04, Pix
 *		Changed ControlSlots for IOBF.
 *  11/16/04, Froste
 *      Changed IOBAlignment to Council
 *  11/10/04, Froste
 *      Implemented new random IOB drop system and changed drop change to 12%
 *	11/05/04, Pigpen
 *		Made changes for Implementation of IOBSystem. Changes include:
 *		Removed IsEnemy and Aggressive Action Checks. These are now handled in BaseCreature.cs
 *		Set Creature IOBAlignment to Undead.
 *	11/2/04, Adam
 *		Increase gold if this is IOB mobile resides in it's Stronghold (Wind)
 *	9/26/04, Adam
 *		Add 5% IOB drop (BloodDrenchedBandana)
 *	7/21/04, mith
 *		IsEnemy() and AggressiveAction() code added to support Brethren property of BloodDrenchedBandana.
 *	7/6/04, Adam
 *		1. implement Jade's new Category Based Drop requirements
 *  6/5/04, Pix
 *		Merged in 1.0RC0 code.
 *  10/6/04, Froste
 *		New Fall Fashions!!
 */

using System;
using Server;
using Server.Items;
using Server.Engines.IOBSystem;

namespace Server.Mobiles
{
	[CorpseName("an evil mage lord corpse")]
	public class EvilMageLord : BaseCreature
	{
		// Blackthorn's Revenge is when we got the Todd McFarlane (crap) bodies
		//	Blackthorn's Revenge was release 2/12/2002 according to UOGuide.
		// Since Publish 15 was 1/9/2002, we can safely exclude Todd McFarlane bodies pre Publish 15
		private bool Blackthorns_Revenge = (Core.Publish > 15 && (Core.UOAI || Core.UOAR || Core.UOMO) == false);

		[Constructable]
		public EvilMageLord()
			: base(AIType.AI_Mage, FightMode.All | FightMode.Closest, 10, 1, 0.25, 0.5)
		{
			Hue = Utility.RandomSkinHue();
			IOBAlignment = IOBAlignment.Council;
			ControlSlots = 3;

			SetStr(81, 105);
			SetDex(191, 215);
			SetInt(126, 150);

			SetHits(49, 63);

			SetDamage(5, 10);

			SetSkill(SkillName.EvalInt, 80.2, 100.0);
			SetSkill(SkillName.Magery, 95.1, 100.0);
			SetSkill(SkillName.Meditation, 27.5, 50.0);
			SetSkill(SkillName.MagicResist, 77.5, 100.0);
			SetSkill(SkillName.Tactics, 65.0, 87.5);
			SetSkill(SkillName.Wrestling, 20.3, 80.0);

			Fame = 10500;
			Karma = -10500;

			InitBody();
			InitOutfit();

			VirtualArmor = 16;
		}

		public override bool CanRummageCorpses { get { return Core.UOAI || Core.UOAR ? true : true; } }
		public override bool AlwaysMurderer { get { return true; } }
		public override int Meat { get { return 1; } }

		public EvilMageLord(Serial serial)
			: base(serial)
		{
		}

		public override void InitBody()
		{
			Name = NameList.RandomName("evil mage lord");
			Female = false;

			if (Blackthorns_Revenge)
				Body = Utility.RandomList(125, 126);	// Todd McFarlane (crap) bodies
			else
				Body = 0x190;
		}
		public override void InitOutfit()
		{
			WipeLayers();

			if (Core.UOAI || Core.UOAR)
			{
				if (Utility.RandomBool())
					AddItem(new Shoes(Utility.RandomBlueHue()));
				else
					AddItem(new Sandals(Utility.RandomBlueHue()));

				//New Fall Fashions!

				Item EvilMageRobe = new Robe();
				EvilMageRobe.Hue = 0x1;
				EvilMageRobe.LootType = LootType.Newbied;
				AddItem(EvilMageRobe);

				Item EvilWizHat = new WizardsHat();
				EvilWizHat.Hue = 0x1;
				EvilWizHat.LootType = LootType.Newbied;
				AddItem(EvilWizHat);

				Item Bracelet = new GoldBracelet();
				Bracelet.LootType = LootType.Newbied;
				AddItem(Bracelet);

				Item Ring = new GoldRing();
				Ring.LootType = LootType.Newbied;
				AddItem(Ring);

				Item hair = new LongHair();
				hair.Hue = 0x47E;
				hair.Layer = Layer.Hair;
				hair.Movable = false;
				AddItem(hair);

				Item beard = new MediumLongBeard();
				beard.Hue = 0x47E;
				beard.Movable = false;
				beard.Layer = Layer.FacialHair;
				AddItem(beard);
			}
			else
			{
				if (Blackthorns_Revenge == false)
				{	// not Todd's graphocs, so we dress
					// evil mage lord colors 1106 1109
					AddItem(new Robe(Utility.Random(1106, 4)));

					// Don't think we should drop the sandals .. stratics is unclear when it comes to clothes.
					// http://web.archive.org/web/20020414131123/uo.stratics.com/hunters/evilmagelord.shtml
					// Blue Robe: 200 to 250 Gold, Gems, Scrolls (circles 4-7), Reagents
					/*Sandals shoes = new Sandals();
					if (Core.UOSP || Core.UOMO)
						shoes.LootType = LootType.Newbied;
					AddItem(shoes);*/

					/* Publish 8
					 * Shopkeeper Changes
					 * NPC shopkeepers will no longer have colored sandals. Evil NPC Mages will carry these items.
					 */
					if (Core.Publish >= 4)
					{
						// http://forums.uosecondage.com/viewtopic.php?f=8&t=22266
						// runuo.com/community/threads/evil-mage-hues.91540/
						if (0.20 >= Utility.RandomDouble())
							AddItem(new Shoes(Utility.RandomBlueHue()));
						else
							AddItem(new Sandals(Utility.RandomBlueHue()));
					}
					else
						AddItem(new Sandals());
				}

				Item hair = null;
				switch (Utility.Random(4))
				{
					case 0: //  bald
						break;
					case 1:
						hair = new ShortHair();
						break;
					case 2:
						hair = new LongHair();
						break;
					case 3:
						hair = new ReceedingHair();
						break;
				}

				if (hair != null)
				{
					hair.Hue = Utility.RandomHairHue();
					hair.Layer = Layer.Hair;
					hair.Movable = false;
					AddItem(hair);
				}

				Item beard = null;
				switch (Utility.Random(4))
				{
					case 0: //  clean shaven
						break;
					case 1:
						beard = new LongBeard();
						break;
					case 2:
						beard = new ShortBeard();
						break;
					case 3:
						beard = new MediumLongBeard();
						break;
					case 4:
						beard = new MediumShortBeard();
						break;
				}

				if (beard != null)
				{
					beard.Hue = (hair != null) ? hair.Hue : Utility.RandomHairHue(); // do the drapes match the carpet?
					beard.Movable = false;
					beard.Layer = Layer.FacialHair;
					AddItem(beard);
				}
			}
		}

		public override void GenerateLoot()
		{
			if (Core.UOAI || Core.UOAR)
			{
				PackReg(23);
				PackScroll(2, 7);
				PackScroll(2, 7);

				PackItem(new Robe(Utility.RandomMetalHue())); // Former AddItem moved to the loot section
				PackItem(new WizardsHat(Utility.RandomMetalHue())); // Former AddItem moved to the loot section

				// pack the gold
				PackGold(100, 130);

				// Froste: 12% random IOB drop
				if (0.12 > Utility.RandomDouble())
				{
					Item iob = Loot.RandomIOB();
					PackItem(iob);
				}

				// Category 2 MID
				PackMagicItem(1, 1, 0.05);

				if (IOBRegions.GetIOBStronghold(this) == IOBAlignment)
				{
					// 30% boost to gold
					PackGold(base.GetGold() / 3);
				}
			}
			else
			{
				if (Core.UOSP || Core.UOMO)
				{	// http://web.archive.org/web/20020414131123/uo.stratics.com/hunters/evilmagelord.shtml
					// Blue Robe: 200 to 250 Gold, Gems, Scrolls (circles 4-7), Reagents
					if (Spawning)
					{
						PackGold(200, 250);
					}
					else
					{
						if (Blackthorns_Revenge == true)
						{	// don't need to dress Todd's graphics - just add as loot
							// evil mage lord colors 1106 1109
							PackItem(new Robe(Utility.Random(1106, 4)));
							// removed sandals (unsure)
						}

						PackGem(1, .9);
						PackGem(1, .05);
						PackScroll(4, 7);
						PackReg(23);
					}
				}
				else
				{
					if (Spawning)
					{
						PackReg(23);

						if (Blackthorns_Revenge == true)
						{	// don't need to dress Todd's graphics - just add as loot
							PackItem(new Sandals());
							// evil mage lord colors 1106 1109
							PackItem(new Robe(Utility.Random(1106, 4)));
						}
					}

					AddLoot(LootPack.Average);
					AddLoot(LootPack.Meager);
					AddLoot(LootPack.MedScrolls, 2);
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
