using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI.Values;
using System.Linq;

namespace Olympus.AIO.SDK.Helpers
{
    internal class ManaManager
    {
        public static int GetNeededMana(SpellSlot slot, MenuSlider value)
        {
            if (OlympusAIO.MainMenu["DisableManaMangerIfBlueBuff"].GetValue<MenuBool>().Enabled)
            {
                if (OlympusAIO.objPlayer.HasBuff("crestoftheancientgolem"))
                {
                    return 0;
                }
            }

            if (ObjectManager.Get<GameObject>().Any(x => x.Type == GameObjectType.EffectEmitter && x.Name == "Perks_ManaflowBand_Buff" && x.DistanceToPlayer() <= 75))
            {
                return 0;
            }
            if (ObjectManager.Get<GameObject>().Any(x => x.Type == GameObjectType.EffectEmitter && x.Name == "Perks_LastResort_Buf" && x.DistanceToPlayer() <= 75))
            {
                return 0;
            }

            var SpellData = UtilityManager.ManaCostArray.FirstOrDefault(x => x.Key == OlympusAIO.objPlayer.CharacterName);
            var SpellCost = SpellData.Value[slot][OlympusAIO.objPlayer.GetSpell(slot).Level - 1];

            return value.GetValue<MenuSlider>().Value + (int)(SpellCost / OlympusAIO.objPlayer.MaxMana * 100);
        }
    }
}