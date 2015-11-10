namespace creepyTristana
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    public class Settings
    {
        public static void SetSpells()
        {
            Variables.Q = new Spell(SpellSlot.Q);
            Variables.E = new Spell(SpellSlot.E, 550 + (7 * (ObjectManager.Player.Level - 1)));
            Variables.R = new Spell(SpellSlot.R, 550 + (7 * (ObjectManager.Player.Level - 1)));
        }

        public static void SetMenu()
        {
            // Main Menu
            Variables.Menu = new Menu("creepyTristana", "creepy.tristana", true);
            {
                //Orbwalker
                Variables.OrbwalkerMenu = new Menu("Orbwalker", "creepy.tristana.orbwalker");
                {
                    Variables.Orbwalker = new Orbwalking.Orbwalker(Variables.OrbwalkerMenu);
                }
                Variables.Menu.AddSubMenu(Variables.OrbwalkerMenu);

                //Settings Menu
                Variables.SettingsMenu = new Menu("Spell Menu", "creepy.tristana.settingsmenu");
                {
                    Variables.SettingsMenu.AddItem(new MenuItem("creepy.tristana.settings.useq", "Use Q")).SetValue(true);
                    Variables.SettingsMenu.AddItem(new MenuItem("creepy.tristana.settings.useelc", "Use E to clear the wave...creepily.")).SetValue(false);
                    Variables.SettingsMenu.AddItem(new MenuItem("creepy.tristana.settings.usee", "Use E")).SetValue(true);
                    Variables.SettingsMenu.AddItem(new MenuItem("creepy.tristana.settings.user", "Use R")).SetValue(true);
                }
                Variables.Menu.AddSubMenu(Variables.SettingsMenu);
            }
            Variables.Menu.AddToMainMenu();
        }
    }

    /// <summary>
    ///    The variables.
    /// </summary>
    public class Variables
    {
        public static Menu Menu, OrbwalkerMenu, SettingsMenu;
        public static Spell Q, E, R;
        public static Orbwalking.Orbwalker Orbwalker;
        public static double GetExecuteDamage(Obj_AI_Base target)
        {
            return 
                target.GetBuffCount("TristanaECharge") != 0 ? 
                    Variables.E.GetDamage(target) * (0.3 * target.GetBuffCount("TristanaECharge") + 1)
                        + (target.BaseAttackDamage + target.FlatPhysicalDamageMod)
                        + (ObjectManager.Player.TotalMagicalDamage * 0.5)
                    : 0;
        }
    }

    /// <summary>
    ///    The targets.
    /// </summary>
    public class Targets
    {
        public static Obj_AI_Base Target => TargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(ObjectManager.Player), TargetSelector.DamageType.Physical);
        public static Obj_AI_Base ChargeTarget => HeroManager.Enemies.Find(x => x.HasBuff("TristanaECharge"));
    }
}
