using System;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI.Values;

using Olympus.AIO.SDK;

namespace Olympus.AIO.General
{
    internal class General
    {
        public static void OnLoad()
        {
            Orbwalker.OnAction      += OnAction;
            Spellbook.OnCastSpell   += OnCastSpell;
        }
        private static void OnAction(object sender, OrbwalkerActionArgs args)
        {
            if (args.Type == OrbwalkerType.BeforeAttack)
            {
                switch (Orbwalker.ActiveMode)
                {
                    case OrbwalkerMode.Combo:
                        if (OlympusAIO.objPlayer.Level >= OlympusAIO.MainMenu["DisableAAInCombo"].GetValue<MenuSliderButton>().Value)
                        {
                            if (!OlympusAIO.MainMenu["DisableAAInCombo"].GetValue<MenuSliderButton>().Enabled)
                                return;

                            args.Process = false;
                        }
                        break;
                    case OrbwalkerMode.LaneClear:
                        if (OlympusAIO.MainMenu["SupportMode"].GetValue<MenuBool>().Enabled)
                        {
                            var enemyMinions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(OlympusAIO.objPlayer.AttackRange));

                            if (enemyMinions.Contains(args.Target))
                            {
                                args.Process = !GameObjects.AllyHeroes.Any(x => !x.IsMe && x.DistanceToPlayer() > 2000);
                            }
                        }
                        break;
                }
            }
        }
        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            var Slot = args.Slot;

            if (sender.Owner.IsMe && UtilityManager.SpellSlots.Contains(Slot))
            {
                var ChampionSpellManaCost = UtilityManager.ManaCostArray.FirstOrDefault(x => x.Key == OlympusAIO.objPlayer.CharacterName).Value;

                if (ChampionSpellManaCost == null)
                    return;

                var Data = UtilityManager.PreserveManaData;

                var Spell = sender.GetSpell(Slot);

                if (MenuManager.PreserveManaMenu[Slot.ToString().ToLower()].GetValue<MenuBool>().Enabled)
                {
                    var SpellData = Data.FirstOrDefault(x => x.Key == Slot).Value;
                    var NewSpellData = ChampionSpellManaCost[Slot][Spell.Level - 1];

                    if (Data.ContainsKey(Slot) && SpellData != NewSpellData)
                    {
                        Data.Remove(Slot);
                        Console.WriteLine($"Preserve Mana List: Removed {Slot}.");
                    }
                    if (!Data.ContainsKey(Slot) && !Spell.State.HasFlag(SpellState.NotLearned))
                    {
                        Data.Add(Slot, NewSpellData);
                        Console.WriteLine($"Preserve Mana List: Added {Slot}, Cost: {NewSpellData}.");
                    }
                }
                else
                {
                    if (Data.ContainsKey(Slot))
                    {
                        Data.Remove(Slot);
                        Console.WriteLine($"Preserve Mana List: Removed {Slot} (Disabled).");
                    }
                }

                var CheckSum = Data.Where(x => MenuManager.PreserveManaMenu[x.Key.ToString().ToLower()].GetValue<MenuBool>().Enabled).Sum(x => x.Value);

                if (CheckSum <= 0)
                    return;

                if (ObjectManager.Get<GameObject>().Any(x => x.Type == GameObjectType.EffectEmitter && x.Name == "Perks_ManaFlowBand_Buff" && x.DistanceToPlayer() <= 75))
                    return;
                if (ObjectManager.Get<GameObject>().Any(x => x.Type == GameObjectType.EffectEmitter && x.Name == "Perks_LastResort_Buf" && x.DistanceToPlayer() <= 75))
                    return;

                var SpellCost = ChampionSpellManaCost[Slot][OlympusAIO.objPlayer.GetSpell(Slot).Level - 1];
                
                if (!Data.Keys.Contains(Slot) && OlympusAIO.objPlayer.Mana - SpellCost < CheckSum)
                {
                    args.Process = false;
                }
            }
        }
    }
}
