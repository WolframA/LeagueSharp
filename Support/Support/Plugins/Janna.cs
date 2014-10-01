﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Support.Plugins
{
    /// <summary>
    /// TODO:
    /// Shild Towers if enemys get tower aggro
    /// Boost Allies Dmg Output
    /// Evade intergration
    /// </summary>
    public class Janna : PluginBase
    {
        private readonly string[] _attackBoost =
        {
            ""
        };

        public Janna()
            : base("h3h3", new Version(4, 17, 14))
        {
            Q = new Spell(SpellSlot.Q, 1100);
            W = new Spell(SpellSlot.W, 600);
            E = new Spell(SpellSlot.E, 800);
            R = new Spell(SpellSlot.R, 725);

            Q.SetSkillshot(0.5f, 200f, 900f, false, SkillshotType.SkillshotLine);
            GameObject.OnCreate += GameObjectOnOnCreate;
        }

        private void GameObjectOnOnCreate(GameObject sender, EventArgs args)
        {
            if (!E.IsReady() || Player.Mana < Player.MaxMana * GetValue<Slider>("ManaE").Value / 100)
                return;

            // TODO: change to events + move to util
            if (sender is Obj_SpellMissile && sender.IsValid)
            {
                var missile = (Obj_SpellMissile)sender;

                // Ally Turret -> Enemy Hero
                if (missile.SpellCaster is Obj_AI_Turret && missile.SpellCaster.IsValid && missile.SpellCaster.IsAlly &&
                    missile.Target is Obj_AI_Hero && missile.Target.IsValid && missile.Target.IsEnemy)
                {
                    var turret = (Obj_AI_Turret)missile.SpellCaster;

                    if (Player.Distance(turret) < E.Range)
                    {
                        E.Cast(turret, true);
                    }
                }

                // Enemy Hero -> Ally Hero AA
                if (missile.SpellCaster is Obj_AI_Hero && missile.SpellCaster.IsValid && missile.SpellCaster.IsEnemy &&
                    missile.Target is Obj_AI_Hero && missile.Target.IsValid && missile.Target.IsAlly &&
                    Orbwalking.IsAutoAttack(missile.SData.Name))
                {
                    var ally = (Obj_AI_Hero)missile.Target;

                    if (Player.Distance(ally) < E.Range)
                    {
                        E.Cast(ally, true);
                    }
                }

                //// Ally Hero special attack
                //if (missile.SpellCaster is Obj_AI_Hero && missile.SpellCaster.IsValid && missile.SpellCaster.IsAlly)
                //{
                //}
            }
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (Q.IsValidTarget(Target, "ComboQ"))
                {
                    if (Q.Cast(Target, true) == Spell.CastStates.SuccessfullyCasted)
                        Utility.DelayAction.Add(100, () => Q.Cast());
                }

                if (W.IsValidTarget(Target, "ComboW"))
                {
                    W.Cast(Target, true);
                }

                var ally = Utils.AllyBelowHp(GetValue<Slider>("ComboHealthR").Value, R.Range);
                if (R.IsValidTarget(ally, "ComboR", true, false))
                {
                    R.Cast();
                }
            }

            if (HarassMode)
            {
                if (W.IsValidTarget(Target, "HarassW"))
                {
                    W.Cast(Target, true);
                }
            }
        }

        public override void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly)
                return;

            if (Q.IsValidTarget(gapcloser.Sender, "GapcloserQ"))
            {
                if (Q.Cast(Target, true) == Spell.CastStates.SuccessfullyCasted)
                    Utility.DelayAction.Add(100, () => Q.Cast());
            }

            if (W.IsValidTarget(gapcloser.Sender, "GapcloserW"))
            {
                W.Cast(gapcloser.Sender, true);
            }
        }

        public override void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (spell.DangerLevel < InterruptableDangerLevel.High || unit.IsAlly)
                return;

            if (Q.IsValidTarget(unit, "InterruptQ"))
            {
                if (Q.Cast(Target, true) == Spell.CastStates.SuccessfullyCasted)
                    Utility.DelayAction.Add(100, () => Q.Cast());
                return;
            }

            if (!Q.IsReady() && R.IsValidTarget(unit, "InterruptR"))
            {
                R.Cast();
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboR", "Use R", true);
            config.AddSlider("ComboHealthR", "Health to Ult", 15, 1, 100);
        }

        public override void HarassMenu(Menu config)
        {
            config.AddBool("HarassW", "Use W", true);
        }

        public override void ItemMenu(Menu config)
        {
            //config.AddBool("FrostQueen", "Use Frost Queen", true);
            //config.AddBool("TwinShadows", "Use Twin Shadows", true);
            //config.AddBool("Locket", "Use Locket", true);
            //config.AddBool("Talisman", "Use Talisman", true);
            //config.AddBool("Mikael", "Use Mikael", true);
        }

        public override void ManaMenu(Menu config)
        {
            config.AddSlider("ManaE", "Shield Attacks", 30, 1, 100);
        }

        public override void MiscMenu(Menu config)
        {
            config.AddBool("GapcloserQ", "Use Q to Interrupt Gapcloser", true);
            config.AddBool("GapcloserW", "Use W to Interrupt Gapcloser", true);

            config.AddBool("InterruptQ", "Use Q to Interrupt Spells", true);
            config.AddBool("InterruptR", "Use R to Interrupt Spells", true);
        }
    }
}