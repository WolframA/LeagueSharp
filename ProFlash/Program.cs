// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="LeagueSharp - h3h3">
//   Copyright (C) 2015 h3h3
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   TODO The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ProFlash
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;

    internal class Program
    {
        #region Static Fields

        private static readonly Menu Menu = new Menu("ProFlash", "ProFlash", true);
        private static Vector3 FlashPosition;
        private static int LastCastAttempt;
        private static Vector3 WallPosition;

        #endregion

        #region Public Methods and Operators

        public static Vector3? GetFirstWallPoint(Vector3 from, Vector3 to, float step = 25)
        {
            var direction = (to - from).Normalized();

            for (float d = 0; d < from.Distance(to); d = d + step)
            {
                var testPoint = from + d * direction;
                if (testPoint.IsWall())
                {
                    return from + (d - step) * direction;
                }
            }

            return null;
        }

        #endregion

        #region Methods

        private static void Main(string[] args)
        {
            Spellbook.OnCastSpell += OnCastSpell;
            //Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Drawing.OnDraw += OnDraw;

            Menu.AddItem(new MenuItem("active", "Active").SetValue(true));
            //Menu.AddItem(new MenuItem("anti.lee", "Insec Protection").SetValue(true));
            Menu.AddItem(new MenuItem("anit.wall", "Over wall fail Protection").SetValue(true));
            Menu.AddToMainMenu();
        }

        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!Menu.Item("active").GetValue<bool>())
            {
                return;
            }

            if (!Menu.Item("anit.wall").GetValue<bool>())
            {
                return;
            }

            if (!sender.Owner.IsValid<Obj_AI_Hero>())
            {
                return;
            }

            if (!sender.Owner.IsMe)
            {
                return;
            }

            var flashSlot = ObjectManager.Player.GetSpellSlot("summonerflash");
            if (flashSlot == SpellSlot.Unknown || args.Slot != flashSlot)
            {
                return;
            }

            if (ObjectManager.Player.Position.Distance(Game.CursorPos) > 850)
            {
                return;
            }

            LastCastAttempt = Utils.TickCount;
            FlashPosition = new Vector3();
            WallPosition = new Vector3();

            var firstWall = GetFirstWallPoint(ObjectManager.Player.Position, Game.CursorPos);

            if (firstWall.HasValue && Game.CursorPos.IsWall()
                && (ObjectManager.Player.Distance(firstWall.Value) > 100
                    && ObjectManager.Player.Distance(firstWall.Value) < 850))
            {
                args.Process = false;

                WallPosition = firstWall.Value;
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, firstWall.Value);

                return;
            }

            var currentPosition = Game.CursorPos;

            for (var distance = ObjectManager.Player.Distance(Game.CursorPos); distance < 850; distance += 50)
            {
                currentPosition = ObjectManager.Player.Position.Extend(Game.CursorPos, distance);

                if (!currentPosition.IsWall())
                {
                    break;
                }
            }

            if (!currentPosition.IsWall())
            {
                FlashPosition = currentPosition;
            }

            if (!FlashPosition.IsZero)
            {
                Console.WriteLine("- FLASH -");
                ObjectManager.Player.Spellbook.CastSpell(flashSlot, FlashPosition, false);
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (LastCastAttempt + 1000 > Utils.TickCount)
            {
                Render.Circle.DrawCircle(FlashPosition, 100, FlashPosition.IsWall() ? Color.Red : Color.Green);
                Render.Circle.DrawCircle(WallPosition, 50, Color.Aqua);
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!Menu.Item("active").GetValue<bool>())
            {
                return;
            }

            if (!Menu.Item("anti.lee").GetValue<bool>())
            {
                return;
            }

            if (!args.Target.IsValid<Obj_AI_Hero>())
            {
                return;
            }

            if (!sender.IsValid<Obj_AI_Hero>())
            {
                return;
            }

            if (!args.Target.IsMe)
            {
                return;
            }

            if (args.SData.Name != "BlindMonkRKick")
            {
                return;
            }

            var flashSlot = ObjectManager.Player.GetSpellSlot("summonerflash");
            if (flashSlot == SpellSlot.Unknown)
            {
                return;
            }

            if (!ObjectManager.Player.GetSpell(flashSlot).IsReady())
            {
                return;
            }

            FlashPosition = ObjectManager.Player.Position.Extend(sender.Position, 450);
            ObjectManager.Player.Spellbook.CastSpell(flashSlot, FlashPosition, false);
        }

        #endregion
    }
}