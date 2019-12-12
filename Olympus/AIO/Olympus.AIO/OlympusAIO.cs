using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using System;
using System.Linq;
using SharpDX;
using SharpDX.Direct3D9;
using EnsoulSharp.SDK.Utility;

using Olympus.AIO.Champions;
using Olympus.AIO.SDK;
using Olympus.AIO.SDK.Helpers;

namespace Olympus.AIO
{
    class OlympusAIO
    {
        public static AIHeroClient objPlayer;

        public static Menu MainMenu;

        private static string[] allSupportedChampions =
        {
            "Evelynn",
        };

        private static void Main(string[] args)
        {
            GameEvent.OnGameLoad += delegate ()
            {
                objPlayer = ObjectManager.Player;

                SupportChampionsNotify();

                if (allSupportedChampions.All(x => !string.Equals(x, objPlayer.CharacterName, StringComparison.CurrentCultureIgnoreCase)))
                    return; 

                MainMenu = new Menu("Olympus.AIO", "Olympus.AIO: " + objPlayer.CharacterName, true);

                MenuManager.GeneralExecute();

                switch (objPlayer.CharacterName)
                {
                    case "Evelynn":
                        Evelynn.OnLoad();
                        break;
                }

                MainMenu.Attach();
            };
        }
        private static void SupportChampionsNotify()
        {
            var drawPos = new Vector2(120, 120);

            Render.Text MainText = new Render.Text("Olympus.AIO - Supported Champions", drawPos, 20, new ColorBGRA(170, 255, 47, 255));
            MainText.Add(0);
            MainText.OnDraw();

            foreach (var champ in allSupportedChampions)
            {
                drawPos += new Vector2(0, 30);

                Render.Text SupportingChampions = new Render.Text(champ, drawPos, 20, new ColorBGRA(255, 222, 173, 255));
                SupportingChampions.Add(0);
                SupportingChampions.OnDraw();

                DelayAction.Add(13000, () => SupportingChampions.Remove());
            }
            
            DelayAction.Add(13000, () => MainText.Remove());
        }
    }
}
