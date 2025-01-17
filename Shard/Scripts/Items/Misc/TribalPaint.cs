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

/* Scripts/Items/Misc/TribalPaint.cs
 * Changelog
 *	11/29/05, erlein
 *		Removed alteration of body type to fix sprite problem with sitting whilst
 *		paint is applied.
 *	7/23/05, Adam
 *		Remove all Necromancy, and Chivalry nonsense
 */
using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class TribalPaint : Item
	{
		public override int LabelNumber { get { return 1040000; } } // savage kin paint

		[Constructable]
		public TribalPaint()
			: base(0x9EC)
		{
			Hue = 2101;
			Weight = 2.0;
		}

		public TribalPaint(Serial serial)
			: base(serial)
		{
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (IsChildOf(from.Backpack))
			{
				if (!from.CanBeginAction(typeof(Spells.Fifth.IncognitoSpell)))
				{
					from.SendLocalizedMessage(501698); // You cannot disguise yourself while incognitoed.
				}
				else if (!from.CanBeginAction(typeof(Spells.Seventh.PolymorphSpell)))
				{
					from.SendLocalizedMessage(501699); // You cannot disguise yourself while polymorphed.
				}
				//else if ( Spells.Necromancy.TransformationSpell.UnderTransformation( from ) )
				//{
				//from.SendLocalizedMessage( 501699 ); // You cannot disguise yourself while polymorphed.
				//}
				else if (from.HueMod != -1 || from.FindItemOnLayer(Layer.Helm) is OrcishKinMask)
				{
					from.SendLocalizedMessage(501605); // You are already disguised.
				}
				else
				{
					if (!Core.UOSP && !Core.UOMO && !Core.UOAI && !Core.UOAR)
						from.BodyMod = (from.Female ? 184 : 183);
					else
						from.HueMod = 0;

					if (from is PlayerMobile)
						((PlayerMobile)from).SavagePaintExpiration = TimeSpan.FromDays(7.0);

					from.SendLocalizedMessage(1042537); // You now bear the markings of the savage tribe.  Your body paint will last about a week or you can remove it with an oil cloth.

					Consume();
				}
			}
			else
			{
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
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