using System;
using System.Linq;
using System.Collections.Generic;
using SharpDX;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Events;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;

namespace Olympus.PassingThroughTheWalls
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }
        private static void OnGameLoad()
        {
            PassingThroughTheWalls.OnLoad();
        }
    }
    internal class PassingThroughTheWalls
    {
        private static List<Spots> spots = new List<Spots>
        {
            new Spots // down razor
            (
                new Vector3(6482f, 6016f, 51.75841f),
                new Vector3(6873.323f, 5625.154f, 58.50415f),
                1
            ),
            new Spots // bot
            (
                new Vector3(12022f, 3758f, 66.2472f),
                new Vector3(12196.31f, 4513.543f, 51.72949f),
                1
            ),
            new Spots // down blue
            (
                new Vector3(4374f, 8156f, 48.74362f),
                new Vector3(4105.671f, 8039.588f, 50.66162f), // need fix
                2
            ),
            new Spots // dragon
            (
                new Vector3(9060.863f, 4667.828f, 51.94011f),
                new Vector3(9275.459f, 4528.949f, -71.24048f),
                2
            ),
        };

        private static IEnumerable<Spots> MainSpots
        {
            get
            {

                return spots.Where(x => x.Position.IsOnScreen()); ;
            }
        }

        private static Menu MainMenu;

        private static bool Arrived = false;

        private static float LastOrder = 0f;

        public static void OnLoad()
        {
            MainMenu = new Menu("PassingThroughTheWalls", "Passing Through The Walls", true);

            MainMenu.Add(new MenuKeyBind("Key", "Key:", System.Windows.Forms.Keys.Z, KeyBindType.Press));

            MainMenu.Attach();

            Tick.OnTick += OnTick;
            Drawing.OnEndScene += OnEndScene;
        }
        private static void OnTick(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
                return;

            foreach (var spot in MainSpots)
            {
                if (MainMenu["Key"].GetValue<MenuKeyBind>().Active)
                {
                    if (Variables.GameTimeTickCount - LastOrder > 2500)
                    {
                        Arrived = false;
                    }
                    if (!Arrived)
                    {
                        ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, spot.Position);
                    }
                    if (ObjectManager.Player.Distance((spot.Position)) == 0)
                    {
                        Arrived = true;
                        LastOrder = Variables.GameTimeTickCount;
                        ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, spot.CursorPosition);
                    }
                }
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                Console.WriteLine("Champion Pos: " + ObjectManager.Player.Position);
                Console.WriteLine("Cursor Pos: " + Game.CursorPos);
            }
        }
        private static void OnEndScene(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
                return;

            foreach (var spot in MainSpots)
            {
                Render.Circle.DrawCircle(spot.Position, 40, spot.NeedHighSpeedMovement == 1 ? System.Drawing.Color.LightBlue : System.Drawing.Color.OrangeRed);
                Render.Circle.DrawCircle(spot.CursorPosition, 40, spot.NeedHighSpeedMovement == 1 ? System.Drawing.Color.LightBlue : System.Drawing.Color.OrangeRed);
            }
        }
    }
    internal class Spots
    {
        public Spots(Vector3 pos, Vector3 cursorpos, int speed)
        {
            this.Position               = pos;
            this.CursorPosition         = cursorpos;
            this.NeedHighSpeedMovement  = speed;
        }
        public Vector3 Position { get; set; }
        public Vector3 CursorPosition { get; set; }
        public int NeedHighSpeedMovement { get; set; }
    }
}
