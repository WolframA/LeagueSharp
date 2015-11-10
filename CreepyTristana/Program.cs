using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace CreepyTristana
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.CharData.BaseSkinName != "Tristana")
            {
                return;
            }

            Tristana.OnLoad();
            Game.PrintChat("<font color='#FFCC00'><b>Rape bronzies with Creepy Tristana</b>...or be raped.</font>");
        }
    }
}