using System;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;

using System.Collections.Generic;

namespace Olympus.AIO.SDK
{
    internal class UtilityManager
    {
        public static SpellSlot[] SpellSlots =
        {
            SpellSlot.Q,
            SpellSlot.W,
            SpellSlot.E,
            SpellSlot.R,
        };

        public static readonly string[] JungleList =
        {
            "SRU_Dragon_Air", "SRU_Dragon_Fire", "SRU_Dragon_Water",
            "SRU_Dragon_Earth", "SRU_Dragon_Elder", "SRU_Baron",
            "SRU_RiftHerald", "SRU_Red", "SRU_Blue", "SRU_Gromp",
            "Sru_Crab", "SRU_Krug", "SRU_Razorbeak", "SRU_Murkwolf"
        };

        public static Dictionary<SpellSlot, int> PreserveManaData = new Dictionary<SpellSlot, int>();

        public static Dictionary<string, Dictionary<SpellSlot, int[]>> ManaCostArray = new Dictionary<string, Dictionary<SpellSlot, int[]>>
        {
            {
                "Evelynn", new Dictionary<SpellSlot, int[]>
                {
                    { SpellSlot.Q, new [] { 40, 45, 50, 55, 60} },
                    { SpellSlot.W, new [] { 60, 70, 80, 90, 100} },
                    { SpellSlot.E, new [] { 40, 45, 50, 55, 60} },
                    { SpellSlot.R, new [] { 100, 100, 100} },
                }
            }
        };
        public static int GetManaCost(SpellSlot slot)
        {
            var ChampionSlots = ManaCostArray.FirstOrDefault(x => x.Key == OlympusAIO.objPlayer.CharacterName).Value;
            var Slot = ChampionSlots.FirstOrDefault(x => x.Key == slot);
            var SlotValue = Slot.Value[OlympusAIO.objPlayer.Spellbook.GetSpell(slot).Level - 1];

            return SlotValue;
        }
        public static float ConvertToDegrees(float degrees)
        {
            return (float)(degrees * Math.PI / 180f);
        }
        public static float GetRealHealth(AIBaseClient unit)
        {
            return unit.Health + unit.PhysicalShield;
        }
        public static double GetRemainingCooldownTime(Spell spell)
        {
            return spell.Instance.CooldownExpires - Variables.GameTimeTickCount;
        }
    }
}
