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

using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	public class SBLeatherArmor : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBLeatherArmor()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add(new GenericBuyInfo(typeof(LeatherArms), 80, 20, 0x13CD, 0));
				Add(new GenericBuyInfo(typeof(LeatherChest), 101, 20, 0x13CC, 0));
				Add(new GenericBuyInfo(typeof(LeatherGloves), 60, 20, 0x13C6, 0));
				Add(new GenericBuyInfo(typeof(LeatherGorget), 74, 20, 0x13C7, 0));
				Add(new GenericBuyInfo(typeof(LeatherLegs), 80, 20, 0x13cb, 0));
				Add(new GenericBuyInfo(typeof(LeatherCap), 10, 20, 0x1DB9, 0));
				Add(new GenericBuyInfo(typeof(FemaleLeatherChest), 116, 20, 0x1C06, 0));
				Add(new GenericBuyInfo(typeof(LeatherBustierArms), 97, 20, 0x1C0A, 0));
				Add(new GenericBuyInfo(typeof(LeatherShorts), 86, 20, 0x1C00, 0));
				Add(new GenericBuyInfo(typeof(LeatherSkirt), 87, 20, 0x1C08, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				/*				Add( typeof( LeatherArms ), 40 );
				 *				Add( typeof( LeatherChest ), 52 );
				 *				Add( typeof( LeatherGloves ), 30 );
				 *				Add( typeof( LeatherGorget ), 37 );
				 *				Add( typeof( LeatherLegs ), 40 );
				 *				Add( typeof( LeatherCap ), 5 );
				 *				Add( typeof( FemaleLeatherChest ), 58 );
				 *				Add( typeof( LeatherBustierArms ), 48 );
				 *				Add( typeof( LeatherShorts ), 43 );
				 *				Add( typeof( LeatherSkirt ), 43 );
				 */
			}
		}
	}
}
