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

/* Scripts/Mobiles/Vendors/SBInfo/SBVagabond.cs
 * ChangeLog
 *	4/24/04, mith
 *		Commented all items from SellList so that NPCs don't buy from players.
 */

using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	public class SBVagabond : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBVagabond()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add(new GenericBuyInfo(typeof(Amber), 90, 20, 0xF25, 0));
				Add(new GenericBuyInfo(typeof(Amethyst), 120, 20, 0xF16, 0));
				Add(new GenericBuyInfo(typeof(Citrine), 60, 20, 0xF15, 0));
				Add(new GenericBuyInfo(typeof(Diamond), 240, 20, 0xF26, 0));
				Add(new GenericBuyInfo(typeof(Emerald), 120, 20, 0xF10, 0));
				Add(new GenericBuyInfo(typeof(Ruby), 90, 20, 0xF13, 0));
				Add(new GenericBuyInfo(typeof(Sapphire), 120, 20, 0xF19, 0));
				Add(new GenericBuyInfo(typeof(StarSapphire), 150, 20, 0xF21, 0));
				Add(new GenericBuyInfo(typeof(Tourmaline), 90, 20, 0xF2D, 0));

				/* Shopkeeper NPCs do not sell any resources (Ingots, Cloth, etc.))
				 * http://www.uoguide.com/Siege_Perilous
				 */
				if (!Core.UOSP)
				{
					Add(new GenericBuyInfo(typeof(Board), 3, 20, 0x1BD7, 0));
					Add(new GenericBuyInfo(typeof(IronIngot), 8, 20, 0x1BF2, 0));
				}

				Add(new GenericBuyInfo(typeof(Necklace), 26, 20, 0x1085, 0));
				Add(new GenericBuyInfo(typeof(GoldRing), 27, 20, 0x108A, 0));
				Add(new GenericBuyInfo(typeof(GoldNecklace), 27, 20, 0x1088, 0));
				Add(new GenericBuyInfo(typeof(GoldBeadNecklace), 27, 20, 0x1089, 0));
				Add(new GenericBuyInfo(typeof(GoldBracelet), 27, 20, 0x1086, 0));
				Add(new GenericBuyInfo(typeof(GoldEarrings), 27, 20, 0x1087, 0));
				Add(new GenericBuyInfo(typeof(Beads), 27, 20, 0x108B, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				if (!Core.UOAI && !Core.UOAR && !Core.UOSP && !Core.UOMO)
				{	// cash buyback
					Add(typeof(Amber), 45);
					Add(typeof(Amethyst), 60);
					Add(typeof(Citrine), 30);
					Add(typeof(Diamond), 120);
					Add(typeof(Emerald), 60);
					Add(typeof(Ruby), 45);
					Add(typeof(Sapphire), 60);
					Add(typeof(StarSapphire), 75);
					Add(typeof(Tourmaline), 45);

					Add(typeof(Board), 1);
					Add(typeof(IronIngot), 4);

					Add(typeof(Necklace), 13);
					Add(typeof(GoldRing), 13);
					Add(typeof(GoldNecklace), 13);
					Add(typeof(GoldBeadNecklace), 13);
					Add(typeof(GoldBracelet), 13);
					Add(typeof(GoldEarrings), 13);
					Add(typeof(Beads), 13);
				}
			}
		}
	}
}