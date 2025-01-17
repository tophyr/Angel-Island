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

/* Scripts/Items/Clothing/Hats.cs
 * ChangeLog
 *	7/26/05, erlein
 *		Automated removal of AoS resistance related function calls. 85 lines removed.
 *	11/06/04,Pigpen
 *		Moved BrigandKinBandana to BrethrenClothing.cs
 *  9/16/04. Pigpen
 * 		Added BrigandKinBandana
 *	7/29/04, mith
 *		Added BearMask() and DeerMask(). Couldn't find documentation on resists, used typical hat resists (0,5,9,5,5). 
 *		Since all we use is Physical (0 in this case), this shouldn't matter too much.
 *	7/21/04, mith
 *		Moved OrcishMask and BloodDrenchedBandana to BrethrenClothing.cs
 *  6/5/04, Pix
 *		Merged in 1.0RC0 code.
 *	5/4/04, mith
 *		Modified hue of blood drenched bandana to be darker.
 */

using System;

namespace Server.Items
{
	public abstract class BaseHat : BaseClothing
	{
		public BaseHat(int itemID)
			: this(itemID, 0)
		{
		}

		public BaseHat(int itemID, int hue)
			: base(itemID, Layer.Helm, hue)
		{
		}

		public BaseHat(Serial serial)
			: base(serial)
		{
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

	public class OrcishMask : BaseHat
	{

		public override bool Dye(Mobile from, DyeTub sender)
		{
			from.SendLocalizedMessage(sender.FailMessage);
			return false;
		}

		[Constructable]
		public OrcishMask()
			: this(0)
		{
		}

		[Constructable]
		public OrcishMask(int hue)
			: base(0x141B, hue)
		{
			Weight = 2.0;
			Dyable = false;
		}

		public OrcishMask(Serial serial)
			: base(serial)
		{
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

	[Flipable(0x2306, 0x2305)]
	public class FlowerGarland : BaseHat
	{

		[Constructable]
		public FlowerGarland()
			: this(0)
		{
		}

		[Constructable]
		public FlowerGarland(int hue)
			: base(0x2306, hue)
		{
			Weight = 1.0;
		}

		public FlowerGarland(Serial serial)
			: base(serial)
		{
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

	public class FloppyHat : BaseHat
	{

		[Constructable]
		public FloppyHat()
			: this(0)
		{
		}

		[Constructable]
		public FloppyHat(int hue)
			: base(0x1713, hue)
		{
			Weight = 1.0;
		}

		public FloppyHat(Serial serial)
			: base(serial)
		{
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

	public class WideBrimHat : BaseHat
	{

		[Constructable]
		public WideBrimHat()
			: this(0)
		{
		}

		[Constructable]
		public WideBrimHat(int hue)
			: base(0x1714, hue)
		{
			Weight = 1.0;
		}

		public WideBrimHat(Serial serial)
			: base(serial)
		{
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

	public class Cap : BaseHat
	{

		[Constructable]
		public Cap()
			: this(0)
		{
		}

		[Constructable]
		public Cap(int hue)
			: base(0x1715, hue)
		{
			Weight = 1.0;
		}

		public Cap(Serial serial)
			: base(serial)
		{
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

	public class SkullCap : BaseHat
	{

		[Constructable]
		public SkullCap()
			: this(0)
		{
		}

		[Constructable]
		public SkullCap(int hue)
			: base(0x1544, hue)
		{
			Weight = 1.0;
		}

		public SkullCap(Serial serial)
			: base(serial)
		{
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

	public class Bandana : BaseHat
	{

		[Constructable]
		public Bandana()
			: this(0)
		{
		}

		[Constructable]
		public Bandana(int hue)
			: base(0x1540, hue)
		{
			Weight = 1.0;
		}

		public Bandana(Serial serial)
			: base(serial)
		{
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

	public class HornedTribalMask : BaseHat
	{
		//public override int BasePhysicalResistance { get { return 6; } }
		//public override int BaseFireResistance { get { return 9; } }
		//public override int BaseColdResistance { get { return 0; } }
		//public override int BasePoisonResistance { get { return 4; } }
		//public override int BaseEnergyResistance { get { return 5; } }

		public override int InitMinHits { get { return 20; } }
		public override int InitMaxHits { get { return 30; } }

		[Constructable]
		public HornedTribalMask()
			: this(0)
		{
		}

		[Constructable]
		public HornedTribalMask(int hue)
			: base(0x1549, hue)
		{
			Weight = 2.0;
		}

		public override bool Dye(Mobile from, DyeTub sender)
		{
			from.SendLocalizedMessage(sender.FailMessage);
			return false;
		}

		public HornedTribalMask(Serial serial)
			: base(serial)
		{
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

	public class TribalMask : BaseHat
	{
		//public override int BasePhysicalResistance { get { return 3; } }
		//public override int BaseFireResistance { get { return 0; } }
		//public override int BaseColdResistance { get { return 6; } }
		//public override int BasePoisonResistance { get { return 10; } }
		//public override int BaseEnergyResistance { get { return 5; } }

		public override int InitMinHits { get { return 20; } }
		public override int InitMaxHits { get { return 30; } }

		[Constructable]
		public TribalMask()
			: this(0)
		{
		}

		[Constructable]
		public TribalMask(int hue)
			: base(0x154B, hue)
		{
			Weight = 2.0;
		}

		public override bool Dye(Mobile from, DyeTub sender)
		{
			from.SendLocalizedMessage(sender.FailMessage);
			return false;
		}

		public TribalMask(Serial serial)
			: base(serial)
		{
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

	public class TallStrawHat : BaseHat
	{

		[Constructable]
		public TallStrawHat()
			: this(0)
		{
		}

		[Constructable]
		public TallStrawHat(int hue)
			: base(0x1716, hue)
		{
			Weight = 1.0;
		}

		public TallStrawHat(Serial serial)
			: base(serial)
		{
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

	public class StrawHat : BaseHat
	{

		[Constructable]
		public StrawHat()
			: this(0)
		{
		}

		[Constructable]
		public StrawHat(int hue)
			: base(0x1717, hue)
		{
			Weight = 1.0;
		}

		public StrawHat(Serial serial)
			: base(serial)
		{
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

	public class SavageMask : BaseHat
	{

		public static int GetRandomHue()
		{
			int v = Utility.RandomBirdHue();

			if (v == 2101)
				v = 0;

			return v;
		}

		public override bool Dye(Mobile from, DyeTub sender)
		{
			from.SendLocalizedMessage(sender.FailMessage);
			return false;
		}

		[Constructable]
		public SavageMask()
			: this(GetRandomHue())
		{
		}

		[Constructable]
		public SavageMask(int hue)
			: base(0x154B, hue)
		{
			Weight = 2.0;
		}

		public SavageMask(Serial serial)
			: base(serial)
		{
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

			if (Hue != 0 && (Hue < 2101 || Hue > 2130))
				Hue = GetRandomHue();
		}
	}

	public class WizardsHat : BaseHat
	{

		[Constructable]
		public WizardsHat()
			: this(0)
		{
		}

		[Constructable]
		public WizardsHat(int hue)
			: base(0x1718, hue)
		{
			Weight = 1.0;
		}

		public WizardsHat(Serial serial)
			: base(serial)
		{
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

	public class MagicWizardsHat : BaseHat
	{

		public override int LabelNumber { get { return 1041072; } } // a magical wizard's hat

		#region Stat Mods
		public void AddStatMods(Mobile m)
		{
			if (m == null)
				return;

			string modName = this.Serial.ToString();

			StatMod strMod = new StatMod(StatType.Str, String.Format("[Magic Hat] -Str {0}", modName), -5, TimeSpan.Zero);
			StatMod dexMod = new StatMod(StatType.Dex, String.Format("[Magic Hat] -Dex {0}", modName), -5, TimeSpan.Zero);
			StatMod intMod = new StatMod(StatType.Int, String.Format("[Magic Hat] +Int {0}", modName), +5, TimeSpan.Zero);

			m.AddStatMod(strMod);
			m.AddStatMod(dexMod);
			m.AddStatMod(intMod);
		}

		public void RemoveStatMods(Mobile m)
		{
			if (m == null)
				return;

			string modName = this.Serial.ToString();

			m.RemoveStatMod(String.Format("[Magic Hat] -Str {0}", modName));
			m.RemoveStatMod(String.Format("[Magic Hat] -Dex {0}", modName));
			m.RemoveStatMod(String.Format("[Magic Hat] +Int {0}", modName));
		}

		public override void OnAdded(object parent)
		{
			base.OnAdded(parent);
			AddStatMods(parent as Mobile);
		}

		public override void OnRemoved(object parent)
		{
			base.OnRemoved(parent);
			RemoveStatMods(parent as Mobile);
		}
		#endregion

		[Constructable]
		public MagicWizardsHat()
			: this(0)
		{
		}

		[Constructable]
		public MagicWizardsHat(int hue)
			: base(0x1718, hue)
		{
			Weight = 1.0;
		}

		public MagicWizardsHat(Serial serial)
			: base(serial)
		{
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

			AddStatMods(Parent as Mobile);
		}
	}

	public class Bonnet : BaseHat
	{

		[Constructable]
		public Bonnet()
			: this(0)
		{
		}

		[Constructable]
		public Bonnet(int hue)
			: base(0x1719, hue)
		{
			Weight = 1.0;
		}

		public Bonnet(Serial serial)
			: base(serial)
		{
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

	public class FeatheredHat : BaseHat
	{

		[Constructable]
		public FeatheredHat()
			: this(0)
		{
		}

		[Constructable]
		public FeatheredHat(int hue)
			: base(0x171A, hue)
		{
			Weight = 1.0;
		}

		public FeatheredHat(Serial serial)
			: base(serial)
		{
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

	public class TricorneHat : BaseHat
	{

		[Constructable]
		public TricorneHat()
			: this(0)
		{
		}

		[Constructable]
		public TricorneHat(int hue)
			: base(0x171B, hue)
		{
			Weight = 1.0;
		}

		public TricorneHat(Serial serial)
			: base(serial)
		{
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

	public class JesterHat : BaseHat
	{

		[Constructable]
		public JesterHat()
			: this(0)
		{
		}

		[Constructable]
		public JesterHat(int hue)
			: base(0x171C, hue)
		{
			Weight = 1.0;
		}

		public JesterHat(Serial serial)
			: base(serial)
		{
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

	public class BearMask : BaseHat
	{

		[Constructable]
		public BearMask()
			: this(0)
		{
		}

		[Constructable]
		public BearMask(int hue)
			: base(0x1545, hue)
		{
			Weight = 4.0;
		}

		public BearMask(Serial serial)
			: base(serial)
		{
		}

		public override bool Dye(Mobile from, DyeTub sender)
		{
			from.SendLocalizedMessage(sender.FailMessage);
			return false;
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

	public class DeerMask : BaseHat
	{

		[Constructable]
		public DeerMask()
			: this(0)
		{
		}

		[Constructable]
		public DeerMask(int hue)
			: base(0x1547, hue)
		{
			Weight = 4.0;
		}

		public DeerMask(Serial serial)
			: base(serial)
		{
		}

		public override bool Dye(Mobile from, DyeTub sender)
		{
			from.SendLocalizedMessage(sender.FailMessage);
			return false;
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
