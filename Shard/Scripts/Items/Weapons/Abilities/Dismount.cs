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

/* Items/Weapons/Abilties/Dismount.cs
 * CHANGELOG:
 *  6/5/04, Pix
 *		Merged in 1.0RC0 code.
 */

using System;
using Server.Mobiles;

namespace Server.Items
{
	/// <summary>
	/// Perfect for the foot-soldier, the Dismount special attack can unseat a mounted opponent.
	/// The fighter using this ability must be on his own two feet and not in the saddle of a steed
	/// (with one exception: players may use a lance to dismount other players while mounted).
	/// If it works, the target will be knocked off his own mount and will take some extra damage from the fall!
	/// </summary>
	public class Dismount : WeaponAbility
	{
		public Dismount()
		{
		}

		public override int BaseMana { get { return 20; } }

		public override bool Validate(Mobile from)
		{
			if (!base.Validate(from))
				return false;

			if (from.Mounted && !(from.Weapon is Lance))
			{
				from.SendLocalizedMessage(1061283); // You cannot perform that attack while mounted!
				return false;
			}

			return true;
		}

		public static readonly TimeSpan BlockMountDuration = TimeSpan.FromSeconds(10.0); // TODO: Taken from bola script, needs to be verified

		public override void OnHit(Mobile attacker, Mobile defender, int damage)
		{
			if (!Validate(attacker))
				return;

			if (attacker.Mounted && !(defender.Weapon is Lance)) // TODO: Should there be a message here?
				return;

			ClearCurrentAbility(attacker);

			IMount mount = defender.Mount;

			if (mount == null)
			{
				attacker.SendLocalizedMessage(1060848); // This attack only works on mounted targets
				return;
			}

			if (!CheckMana(attacker, true))
				return;

			attacker.SendLocalizedMessage(1060082); // The force of your attack has dislodged them from their mount!

			if (attacker.Mounted)
				defender.SendLocalizedMessage(1062315); // You fall off your mount!
			else
				defender.SendLocalizedMessage(1060083); // You fall off of your mount and take damage!

			defender.PlaySound(0x140);
			defender.FixedParticles(0x3728, 10, 15, 9955, EffectLayer.Waist);

			mount.Rider = null;

			defender.BeginAction(typeof(BaseMount));
			Timer.DelayCall(BlockMountDuration, new TimerStateCallback(ReleaseMountLock), defender);

			if (!attacker.Mounted)
				AOS.Damage(defender, attacker, Utility.RandomMinMax(15, 25), 100, 0, 0, 0, 0);
		}

		private void ReleaseMountLock(object state)
		{
			((Mobile)state).EndAction(typeof(BaseMount));
		}
	}
}