using System;
using System.Linq;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D9;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Events;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;

namespace Olympus.JungleTracker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }
        private static void OnGameLoad()
        {
            JungleTracker.OnLoad();
        }
    }
    internal class MenuSettings
    {
        public class Main
        {
            public static MenuBool Enable                   = new MenuBool("Enable", "Enable");
        }
        public class OnScreen
        {
            public static MenuBool OnScreenEnable           = new MenuBool("OnScreenEnable", "Enable");
            public static MenuBool OnScreenAntialiased      = new MenuBool("OnScreenAntialiased", "Antialiased");
            public static MenuSlider OnScreenHeight         = new MenuSlider("OnScreenHeight", "Height", 20, 5, 30);
        }
        public class OnMiniMap
        {
            public static MenuBool OnMiniMapEnable          = new MenuBool("OnMiniMapEnable", "Enable");
            public static MenuBool OnMiniMapAntialiased     = new MenuBool("OnMiniMapAntialiased", "Antialiased");
            public static MenuSlider OnMiniMapHeight        = new MenuSlider("OnMiniMapHeight", "Height", 15, 5, 30);
        }
    }
    internal class JungleCamp
    {
        public JungleCamp (Vector3 position, uint respawnTime, string[] names, GameObjectTeam team = GameObjectTeam.Neutral, bool childrenMobs = true)
        {
            this.Position       = position;
            this.RespawnTime    = respawnTime;
            this.Names          = names;
            this.Team           = team;
            this.ChildrenMobs   = childrenMobs;

            this.ObjectsAlive = new List<string>();
            this.ObjectsDead  = new List<string>();
        }

        public Vector3 Position { get; set; }
        public uint RespawnTime { get; set; }
        public string[] Names { get; set; }
        public GameObjectTeam Team { get; set; }
        public bool ChildrenMobs { get; set; }
        public bool Dead { get; set; }
        public float NextRespawnTime { get; set; }
        public List<string> ObjectsAlive { get; set; }
        public List<string> ObjectsDead { get; set; }
        public Vector2 MinimapPosition
        {
            get
            {
                return TacticalMap.WorldToMinimap(this.Position);
            }
        }
    }
    internal class JungleTracker
    {
        #region Jungle Camps

        private static List<JungleCamp> jungleCamps = new List<JungleCamp>
        {
            new JungleCamp // down side gromp
            (
                new Vector3(2110.628f, 8450.984f, 51.77732f),
                120000,
                new[] { "SRU_Gromp13.1.1" }
            ),
            new JungleCamp // down side blue
            (
                new Vector3(3821.489f, 7901.054f, 52.03593f),
                300000,
                new[] { "SRU_Blue1.1.1" }
            ),
            new JungleCamp // down side wolfs
            (
                new Vector3(3782f, 6444f, 52.4632f),
                120000,
                new[] { "SRU_Murkwolf2.1.1", "SRU_MurkwolfMini2.1.2", "SRU_MurkwolfMini2.1.3" }
            ),
            new JungleCamp // down side razorbeak
            (
                new Vector3(6962.718f, 5354.354f, 50.31254f),
                120000,
                new[] { "SRU_Razorbeak3.1.1", "SRU_RazorbeakMini3.1.2", "SRU_RazorbeakMini3.1.3", "SRU_RazorbeakMini3.1.4", "SRU_RazorbeakMini3.1.5" }
            ),
            new JungleCamp // down side red
            (
                new Vector3(7765.244f, 4020.187f, 53.95644f),
                300000,
                new[] { "SRU_Red4.1.1" }
            ),
            new JungleCamp // down side krug
            (
                new Vector3(8366f, 2690f, 51.04887f),
                120000,
                new[] { "SRU_Krug5.1.1", "SRU_KrugMini5.1.2"}
            ),
            new JungleCamp // top side gromp
            (
                new Vector3(12703.63f, 6443.984f, 51.69078f),
                120000,
                new[] { "SRU_Gromp14.1.1" }
            ),
            new JungleCamp // top side blue
            (
                new Vector3(11031.73f, 6990.844f, 51.72364f),
                300000,
                new[] { "SRU_Blue7.1.1" }
            ),
            new JungleCamp // top side wolfs
            (
                new Vector3(11008.15f, 8387.408f, 62.0905f),
                120000,
                new[] { "SRU_Murkwolf8.1.1", "SRU_MurkwolfMini8.1.2", "SRU_MurkwolfMini8.1.3" }
            ),
            new JungleCamp // top side razorbeak
            (
                new Vector3(7854.389f, 9610.474f, 52.26575f),
                120000,
                new[] { "SRU_Razorbeak9.1.1", "SRU_RazorbeakMini9.1.2", "SRU_RazorbeakMini9.1.3", "SRU_RazorbeakMini9.1.4", "SRU_RazorbeakMini9.1.5", "SRU_RazorbeakMini9.1.6" }
            ),
            new JungleCamp // top side red
            (
                new Vector3(7101.869f, 10900.55f, 56.28268f),
                300000,
                new[] { "SRU_Red10.1.1" }
            ),
            new JungleCamp // top side krug
            (
                new Vector3(6452f, 12252f, 56.4768f),
                120000,
                new[] { "SRU_Krug11.1.1", "SRU_KrugMini11.1.2" }
            ),
            new JungleCamp // top side crab
            (
                new Vector3(4452.812f, 9541.812f, -65.5869f),
                120000,
                new[] { "Sru_Crab16.1.1" }
            ),
            new JungleCamp // bot side crab
            (
                new Vector3(10452.96f, 5213.525f, -62.8102f),
                120000,
                new[] { "Sru_Crab15.1.1" }
            ),
            new JungleCamp // dragon water
            (
                new Vector3(9866.148f, 4414.014f, -71.2406f),
                300000,
                new[] { "SRU_Dragon_Water6.3.1" }
            ),
            new JungleCamp // dragon fire
            (
                new Vector3(9866.148f, 4414.014f, -71.2406f),
                300000,
                new[] { "SRU_Dragon_Fire6.2.1" }
            ),
            new JungleCamp // dragon air
            (
                new Vector3(9866.148f, 4414.014f, -71.2406f),
                300000,
                new[] { "SRU_Dragon_Air6.1.1" }
            ),
            new JungleCamp // dragon earth
            (
                new Vector3(9866.148f, 4414.014f, -71.2406f),
                300000,
                new[] { "SRU_Dragon_Earth6.4.1" }
            ),
            new JungleCamp // dragon elder
            (
                new Vector3(9866.148f, 4414.014f, -71.2406f),
                360000,
                new[] { "SRU_Dragon_Elder6.5.1" }
            ),
            new JungleCamp // baron
            (
                new Vector3(5007.124f, 10471.45f, -71.2406f),
                360000,
                new[] { "SRU_Baron12.1.1" }
            ),
        };

        #endregion

        #region vars

        private static IEnumerable<JungleCamp> DeadCamps
        {
            get
            {
                return jungleCamps.Where(x => x.Dead);
            }
        }

        private static Font fontToScreen;
        private static Font fontToMiniMap;
        private static event EventHandler<JungleCamp> CampDied;
        private static Menu myMenu;

        #endregion

        public static void OnLoad()
        {
            #region Menu

            myMenu = new Menu("Jungle Tracker", "Jungle Tracker", true);

            myMenu.Add(new MenuSeparator("0", "Jungle Tracker Settings"));
            myMenu.Add(MenuSettings.Main.Enable);

            var onScreenMenu = new Menu("onScreenMenu", "On Screen")
            {
                new MenuSeparator("0", "On Screen Settings"),
                MenuSettings.OnScreen.OnScreenEnable,
                MenuSettings.OnScreen.OnScreenAntialiased,
                MenuSettings.OnScreen.OnScreenHeight,
            };
            myMenu.Add(onScreenMenu);

            var onMiniMapMenu = new Menu("onMiniMapMenu", "On MiniMap")
            {
                new MenuSeparator("0", "On MiniMap Settings"),
                MenuSettings.OnMiniMap.OnMiniMapEnable,
                MenuSettings.OnMiniMap.OnMiniMapAntialiased,
                MenuSettings.OnMiniMap.OnMiniMapHeight,
            };
            myMenu.Add(onMiniMapMenu);

            var creditsMenu = new Menu("creditsMenu", "Credits")
            {
                new MenuSeparator("0", "brian0305"),
                new MenuSeparator("1", "sayuto"),
            };
            myMenu.Add(creditsMenu);

            myMenu.Attach();

            #endregion

            if (MenuSettings.OnScreen.OnScreenAntialiased.Enabled)
                fontToScreen = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Tahoma", Height = MenuSettings.OnScreen.OnScreenHeight.Value, OutputPrecision = FontPrecision.Default, Quality = FontQuality.Antialiased });
            else
                fontToScreen = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Tahoma", Height = MenuSettings.OnScreen.OnScreenHeight.Value, OutputPrecision = FontPrecision.Default, Quality = FontQuality.NonAntialiased });

            if (MenuSettings.OnMiniMap.OnMiniMapAntialiased.Enabled)
                fontToMiniMap = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Tahoma", Height = MenuSettings.OnMiniMap.OnMiniMapHeight.Value, OutputPrecision = FontPrecision.Default, Quality = FontQuality.Antialiased });
            else
                fontToMiniMap = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Tahoma", Height = MenuSettings.OnMiniMap.OnMiniMapHeight.Value, OutputPrecision = FontPrecision.Default, Quality = FontQuality.NonAntialiased });

            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;

            Drawing.OnEndScene += OnEndScane;
            Drawing.OnPreReset  += args => { fontToScreen.OnLostDevice(); fontToMiniMap.OnLostDevice(); };
            Drawing.OnPostReset += args => { fontToScreen.OnResetDevice(); fontToMiniMap.OnResetDevice(); };

        }
        private static void OnCreate(GameObject obj, EventArgs args)
        {
            if (obj.Type != GameObjectType.AIMinionClient)
                return;

            var camp = jungleCamps.FirstOrDefault(x => x.Names.Select(y => y.ToLower()).Any(z => z.Equals(obj.Name.ToLower())));

            if (camp == null)
                return;

            camp.ObjectsAlive.Add(obj.Name);
            camp.ObjectsDead.Remove(obj.Name);

            if (camp.ObjectsAlive.Count != camp.Names.Length)
                return;

            camp.Dead               = false;
            camp.NextRespawnTime    = 0;
        }
        private static void OnDelete(GameObject obj, EventArgs args)
        {
            if (obj.Type != GameObjectType.AIMinionClient)
                return;

            var camp = jungleCamps.FirstOrDefault(x => x.Names.Select(y => y.ToLower()).Any(z => z.Equals(obj.Name.ToLower())));

            if (camp == null)
                return;

            camp.ObjectsDead.Add(obj.Name);
            camp.ObjectsAlive.Remove(obj.Name);

            if (camp.ObjectsDead.Count != camp.Names.Length && camp.ChildrenMobs)
                return;

            camp.Dead = true;
            camp.NextRespawnTime = Game.Time + camp.RespawnTime / 1000f - 3;
            CampDied?.Invoke(obj, camp);
        }
        private static void OnEndScane(EventArgs args)
        {
            if (!MenuSettings.Main.Enable.Enabled || Drawing.Direct3DDevice.IsDisposed || fontToScreen.IsDisposed || fontToMiniMap.IsDisposed)
                return;

            #region getCamps

            /*
            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                Console.WriteLine("=======");

                foreach (var obj in GameObjects.GetJungles(600))
                {
                    Console.WriteLine(obj.Position);
                    Console.WriteLine(obj.Name);
                    Console.WriteLine(obj.Team);
                }
            }*/

            #endregion

            foreach (var camp in DeadCamps.Where(x => x.NextRespawnTime - Game.Time > 0))
            {
                var time = TimeSpan.FromSeconds(camp.NextRespawnTime - Game.Time);
                var text = time.ToString(@"m\:ss");

                var worldToScreen = Drawing.WorldToScreen(new Vector3(camp.Position.X, camp.Position.Y, camp.Position.Z));

                if (worldToScreen.IsOnScreen() && MenuSettings.OnScreen.OnScreenEnable.Enabled)
                {
                    fontToScreen.DrawText(null, text, (int)worldToScreen.X, (int)worldToScreen.Y, new ColorBGRA(255, 255, 255, 255));
                }

                if (MenuSettings.OnMiniMap.OnMiniMapEnable.Enabled)
                {
                    fontToMiniMap.DrawText(null, text, (int)camp.MinimapPosition.X - 10, (int)camp.MinimapPosition.Y, new ColorBGRA(255, 255, 255, 255));
                }
            }
        }
    }
}
