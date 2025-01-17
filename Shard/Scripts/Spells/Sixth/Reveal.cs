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
/* ChangeLog
  * 5/30/05, Kit
		Made internal target class public so AI could access it
  */
using System;
using System.Collections;
using Server.Misc;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Sixth
{
    public class RevealSpell : Spell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Reveal", "Wis Quas",
                SpellCircle.Sixth,
                206,
                9002,
                Reagent.Bloodmoss,
                Reagent.SulfurousAsh
            );

        public RevealSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(IPoint3D p)
        {
            if (!Caster.CanSee(p))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (CheckSequence())
            {
                SpellHelper.Turn(Caster, p);

                SpellHelper.GetSurfaceTop(ref p);

                ArrayList targets = new ArrayList();

                Map map = Caster.Map;

                if (map != null)
                {
                    IPooledEnumerable eable = map.GetMobilesInRange(new Point3D(p), 1 + (int)(Caster.Skills[SkillName.Magery].Value / 20.0));

                    foreach (Mobile m in eable)
                    {
                        if (m.Hidden && (m.AccessLevel == AccessLevel.Player || Caster.AccessLevel > m.AccessLevel) && Caster.CanBeHarmful(m, false, true))
                            targets.Add(m);
                    }

                    eable.Free();
                }

                for (int i = 0; i < targets.Count; ++i)
                {
                    Mobile m = (Mobile)targets[i];

                    m.RevealingAction();

                    m.FixedParticles(0x375A, 9, 20, 5049, EffectLayer.Head);
                    m.PlaySound(0x1FD);
                }
            }

            FinishSequence();
        }

        public class InternalTarget : Target
        {
            private RevealSpell m_Owner;

            public InternalTarget(RevealSpell owner)
                : base(12, true, TargetFlags.None)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D p = o as IPoint3D;

                if (p != null)
                    m_Owner.Target(p);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}