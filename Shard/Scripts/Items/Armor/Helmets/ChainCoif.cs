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

/* Scripts/Items/Armor/Helmets/ChainCoif.cs
* ChangeLog
*	7/26/05, erlein
*		Automated removal of AoS resistance related function calls. 10 lines removed.
*	9/11/04, Pigpen
*	Add Wyrm Skin variant of this Helmet piece.
*/

using System;
using Server.Items;
using System.Collections;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute(0x13BB, 0x13C0)]
	public class ChainCoif : BaseArmor
	{

		public override int InitMinHits { get { return 35; } }
		public override int InitMaxHits { get { return 60; } }

		public override int AosStrReq { get { return 60; } }
		public override int OldStrReq { get { return 20; } }

		public override int ArmorBase { get { return 28; } }

		public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Chainmail; } }

		[Constructable]
		public ChainCoif()
			: base(0x13BB)
		{
			Weight = 1.0;
		}

		public ChainCoif(Serial serial)
			: base(serial)
		{
		}

// old name removed, see base class

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

	public class WyrmSkinHelmet : BaseArmor
	{

		public override int InitMinHits { get { return 35; } }
		public override int InitMaxHits { get { return 60; } }

		public override int AosStrReq { get { return 60; } }
		public override int OldStrReq { get { return 20; } }

		public override int ArmorBase { get { return 28; } }

		public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Chainmail; } }

		[Constructable]
		public WyrmSkinHelmet()
			: base(0x13BB)
		{
			Weight = 1.0;
			Hue = 1367;
			ProtectionLevel = ArmorProtectionLevel.Guarding;
			Durability = ArmorDurabilityLevel.Massive;
			if (Utility.RandomDouble() < 0.20)
				Quality = ArmorQuality.Exceptional;
			Name = "Wyrm Skin Helmet";
		}

		public WyrmSkinHelmet(Serial serial)
			: base(serial)
		{
		}

		// Special version that DOES NOT show armor attributes and tags
		public override void OnSingleClick(Mobile from)
		{
			ArrayList attrs = new ArrayList();

			int number;

			if (Name == null)
			{
				number = LabelNumber;
			}
			else
			{
				this.LabelTo(from, Name);
				number = 1041000;
			}

			if (attrs.Count == 0 && Crafter == null && Name != null)
				return;

			EquipmentInfo eqInfo = new EquipmentInfo(number, Crafter, false, (EquipInfoAttribute[])attrs.ToArray(typeof(EquipInfoAttribute)));

			from.Send(new DisplayEquipmentInfo(this, eqInfo));

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
