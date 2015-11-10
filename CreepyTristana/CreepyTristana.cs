namespace CreepyTristana
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    public class Tristana
    {
        public static void OnLoad()
        {
            Settings.SetSpells();
            Settings.SetMenu();

            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnDoCast += Obj_AI_Base_OnDoCast;
        }

        public static void Game_OnGameUpdate(EventArgs args)
        {
            if (!ObjectManager.Player.IsDead &&
                Targets.Target != null &&
                Targets.Target.IsValid<Obj_AI_Hero>() &&
                ObjectManager.Player.Distance(Targets.Target) <= ObjectManager.Player.AttackRange &&
                Variables.Menu.Item("creepy.tristana.settings.useq").GetValue<bool>() &&
                Variables.Q.IsReady())
            {
                Variables.Q.Cast();
            }
        }

        public static void Obj_AI_Base_OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None &&
                Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LastHit &&
                sender.IsMe &&
                Orbwalking.IsAutoAttack(args.SData.Name))
            {
                var herotg = args.Target as Obj_AI_Hero;
                var basetg = args.Target as Obj_AI_Base;
                
                if (args.Target.IsValid<Obj_AI_Hero>())
                {
                    if (Variables.Menu.Item("creepy.tristana.settings.usee").GetValue<bool>() &&
                        Variables.E.IsReady())
                    {
                        Variables.E.Cast(herotg);
                    }
                    
                    if (Variables.Menu.Item("creepy.tristana.settings.user").GetValue<bool>() &&
                        Variables.R.IsReady() &&
                        Variables.GetExecuteDamage(herotg) + Variables.R.GetDamage(herotg) > herotg.Health)
                    {
                        Orbwalking.ResetAutoAttackTimer();
                        Variables.R.Cast(herotg);
                        return;
                    }
                }
                else if (args.Target.IsValid<Obj_AI_Base>() &&
                    !(args.Target is Obj_AI_Hero) &&
                    Variables.Menu.Item("creepy.tristana.settings.useelc").GetValue<bool>() &&
                    Variables.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    Variables.E.Cast(basetg);
                    return;
                }
            }
        }
    }
}
