using System.Linq;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;

namespace Olympus.AIO.SDK
{
    static class MenuManager
    {
        public static Menu PreserveManaMenu = new Menu("PreserveManaMenu", "Preserve Mana");

        public static Menu ComboMenu        = new Menu("ComboMenu", "Combo");

        public static Menu HarassMenu       = new Menu("HarassMenu", "Harass");

        public static Menu LaneClearMenu    = new Menu("LaneClearMenu", "Lane Clear");

        public static Menu JungleClearMenu  = new Menu("JungleClearMenu", "Jungle Clear");

        public static Menu LastHitMenu      = new Menu("LastHitMenu", "Last Hit");

        public static Menu MiscMenu         = new Menu("MiscMenu", "Misc");

        public static Menu MiscKillSteal    = new Menu("MiscKillSteal", "Killsteal");

        public static Menu GapCloserMenu    = new Menu("MiscGapcloser", "Gapcloser");

        public static Menu DrawingsMenu     = new Menu("DrawingsMenu", "Drawings");

        public static Menu CreditsMenu      = new Menu("CreditsMenu", "Credits");

        public static Menu GapcloserWhiteList;

        public static Menu ComboWhiteList;

        public static Menu ComboWhiteList2;

        public static Menu HarassWhiteList;

        public static Menu LaneClearWhiteList;

        public static Menu JungleClearWhiteList;

        public static Menu DamageIndicatorMenu  = new Menu("DamageIndicator", "Damage Indicator");

        public static Menu SpellRangesMenu      = new Menu("SpellRangesMenu", "Spell Ranges");

        public static MenuBool AddBool(Menu menu, string name, string displayName, bool defaultValue)
        {
            return menu.Add(new MenuBool(name, displayName, defaultValue));
        }
        public static MenuSliderButton AddSliderBool(Menu menu, string name, string displayName, int index, int min, int max, bool defaultValue)
        {
            return menu.Add(new MenuSliderButton(name, displayName, index, min, max, defaultValue));
        }
        public static MenuSlider AddSlider(Menu menu, string name, string displayName, int value, int min, int max)
        {
            return menu.Add(new MenuSlider(name, displayName, value, min, max));
        }
        public static MenuKeyBind AddKeyBind(Menu menu, string name, string displayName, System.Windows.Forms.Keys key, KeyBindType type)
        {
            return menu.Add(new MenuKeyBind(name, displayName, key, type));
        }
        public static MenuSeparator AddSeparator(Menu menu, string name, string displayName)
        {
            return menu.Add(new MenuSeparator(name, displayName));
        }
        public static MenuList AddList(Menu menu, string name, string displayName, string[] value, int index)
        {
            return menu.Add(new MenuList(name, displayName, value, index));
        }

        public static void GeneralExecute()
        {
            AddSeparator(OlympusAIO.MainMenu, "GeneralSettings", "General Settings");
            AddBool(OlympusAIO.MainMenu, "SupportMode", "Support Mode", false);
            AddSliderBool(OlympusAIO.MainMenu, "DisableAAInCombo", "Disable AA in Combo | If level >= x", 2, 2, 18, false);
            AddBool(OlympusAIO.MainMenu, "DisableManaMangerIfBlueBuff", "Ignore Mana Manager If Blue Buff", true);

            var ChampionSpellManaCosts = UtilityManager.ManaCostArray.FirstOrDefault(x => x.Key == OlympusAIO.objPlayer.CharacterName).Value;

            if (ChampionSpellManaCosts != null)
            {
               AddSeparator(PreserveManaMenu, "PreserveManaSeperator", "Preserve Mana For:");

                foreach (var slot in UtilityManager.SpellSlots)
                {
                    AddBool(PreserveManaMenu, slot.ToString().ToLower(), slot.ToString(), false);
                }
            }
            else
            {
                AddSeparator(PreserveManaMenu, "PreserveManaSeperator", "Preserve Mana is not needed");
            }

            OlympusAIO.MainMenu.Add(PreserveManaMenu);

            AddSeparator(OlympusAIO.MainMenu, "ChampionSettings", "Champion Settings");
            OlympusAIO.MainMenu.Add(ComboMenu);
            OlympusAIO.MainMenu.Add(HarassMenu);
            OlympusAIO.MainMenu.Add(LaneClearMenu);
            OlympusAIO.MainMenu.Add(JungleClearMenu);
            OlympusAIO.MainMenu.Add(LastHitMenu);
            OlympusAIO.MainMenu.Add(MiscMenu);
            OlympusAIO.MainMenu.Add(DrawingsMenu);
            OlympusAIO.MainMenu.Add(CreditsMenu);

            switch (OlympusAIO.objPlayer.CharacterName)
            {
                case "Annie":
                    break;
                case "Evelynn":
                    EvelynnExecute();
                    break;
            }
        }
        public static void AnnieExecute()
        {

        }
        public static void EvelynnExecute()
        {
            AddBool(ComboMenu, "UseQ", "Use Q", true);
            AddBool(ComboMenu, "UseW", "Use W", true);
            AddBool(ComboMenu, "UseE", "Use E", true);
            AddBool(ComboMenu, "UseR", "Use R", true);

            AddSeparator(ComboMenu, "QSettings", "Q Settings");
            AddBool(ComboMenu, "QOnlyIfFullyAllured", "Only If Enemy Is Fully Allured", true);
            AddSeparator(ComboMenu, "WSettings", "W Settings");
            AddList(ComboMenu, "WSelectBy", "Select By:", new[] { "Most AD", "Most AP", "Lowest Health", "Most Priority" }, 2);

            ComboWhiteList = new Menu("ComboWhiteList", "W WhiteList");
            {
                foreach (var target in GameObjects.EnemyHeroes)
                {
                    AddBool(ComboWhiteList, target.CharacterName.ToLower(), target.CharacterName, true);
                }
                ComboMenu.Add(ComboWhiteList);
            }

            AddSeparator(ComboMenu, "ESettings", "E Settings");
            AddBool(ComboMenu, "EOnlyIfFullyAllured", "Only If Enemy Is Fully Allured", true);
            AddSeparator(ComboMenu, "RSettings", "R Settings");
            AddSliderBool(ComboMenu, "RAoE", "If can hit >= x", 2, 1, 5, true);
            AddSlider(ComboMenu, "RSafetyRange", "Safety Range", 450, 200, 800);

            ComboWhiteList2 = new Menu("ComboWhiteList2", "R WhiteList");
            {
                foreach (var target in GameObjects.EnemyHeroes)
                {
                    AddBool(ComboWhiteList2, target.CharacterName.ToLower(), target.CharacterName, true);
                }
                ComboMenu.Add(ComboWhiteList2);
            }

            AddBool(HarassMenu, "UseQ", "Use Q", true);
            AddBool(HarassMenu, "UseE", "Use E", true);
            AddSeparator(HarassMenu, "ManaManager", "Mana Manager");
            AddSlider(HarassMenu, "MinMana", "If Mana >= x", 60, 0, 100);
            AddSeparator(HarassMenu, "WhiteList", "White List");

            foreach (var target in GameObjects.EnemyHeroes)
            {
                AddBool(HarassMenu, target.CharacterName.ToLower(), target.CharacterName, true);
            } 

            AddBool(LaneClearMenu, "UseQ", "Use Q", true);
            AddBool(LaneClearMenu, "UseE", "Use E", true);
            AddSeparator(LaneClearMenu, "QSettings", "Q Settings");
            AddSlider(LaneClearMenu, "QHits", "If Hitable minions >= x", 3, 1, 5);
            AddSeparator(LaneClearMenu, "ManaManager", "Mana Manager");
            AddSlider(LaneClearMenu, "MinMana", "If Mana >= x", 50, 0, 100);

            JungleClearWhiteList = new Menu("JungleClearWhiteList", "Allure: WhiteList");
            {
                foreach (var target in UtilityManager.JungleList)
                {
                    AddBool(JungleClearWhiteList, target, target, true);
                }
            } JungleClearMenu.Add(JungleClearWhiteList);

            AddBool(JungleClearMenu, "UseQ", "Use Q", true);
            AddBool(JungleClearMenu, "UseW", "Use W", true);
            AddBool(JungleClearMenu, "UseE", "Use E", true);
            AddSeparator(JungleClearMenu, "WSettings", "W Settings");
            AddSeparator(JungleClearMenu, "ManaManager", "Mana Manager");
            AddSlider(JungleClearMenu, "MinMana", "If Mana >= x", 10, 0, 100);

            AddBool(MiscMenu, "AASemiAllured", "Don't AA Semi-Allured Targets", true);
            AddBool(MiscKillSteal, "KSEnable", "Enable", true);
            AddBool(MiscKillSteal, "KSwithE", "Killsteal with E", true);
            AddBool(MiscKillSteal, "KSwithR", "Killsteal with R", true);
            MiscMenu.Add(MiscKillSteal);
            AddBool(GapCloserMenu, "Gapcloser", "Enable", true);
            MiscMenu.Add(GapCloserMenu);

            GapcloserWhiteList = new Menu("GapcloserWhiteList", "Gapcloser WiteList");
            {
                foreach (var target in GameObjects.EnemyHeroes)
                {
                    AddBool(GapcloserWhiteList, target.CharacterName.ToLower(), target.CharacterName, true);
                }
                GapCloserMenu.Add(GapcloserWhiteList);
            }

            AddBool(DrawingsMenu, "Disable", "Disable", false);
            AddBool(DamageIndicatorMenu, "Enable", "Enable", true);
            DrawingsMenu.Add(DamageIndicatorMenu);
            AddBool(SpellRangesMenu, "QRange", "Q Range", true);
            AddBool(SpellRangesMenu, "WRange", "W Range", true);
            AddBool(SpellRangesMenu, "ERange", "E Range", true);
            AddBool(SpellRangesMenu, "RRange", "R Range", true);
            DrawingsMenu.Add(SpellRangesMenu);

            AddSeparator(CreditsMenu, "Exory", "Exory");
            AddSeparator(CreditsMenu, "Sayuto", "Sayuto");
            AddSeparator(CreditsMenu, "N1ghtMoon", "N1ghtMoon");
        }
    }
}
