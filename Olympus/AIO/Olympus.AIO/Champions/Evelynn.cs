using System;
using System.Linq;
using SharpDX;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Events;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;

using Olympus.AIO.SDK;
using Olympus.AIO.SDK.Helpers;

namespace Olympus.AIO.Champions
{
    internal class Evelynn
    {
        public static AIHeroClient objPlayer = ObjectManager.Player;

        public static Spell Q, Q2, W, E, R;

        public static void OnLoad()
        {
            Q = new Spell(SpellSlot.Q, 800f);
            Q.SetSkillshot(0.25f, 60f, 2400f, false, SkillshotType.Line);

            Q2  = new Spell(SpellSlot.Q, 550f);
            W   = new Spell(SpellSlot.W, 1200f);
            E   = new Spell(SpellSlot.E, 225f + objPlayer.BoundingRadius);

            R = new Spell(SpellSlot.R, 450f + objPlayer.BoundingRadius);
            R.SetSkillshot(0.25f, UtilityManager.ConvertToDegrees(100), float.MaxValue, false, SkillshotType.Cone);

            /* Main */
            Tick.OnTick += Events.OnTick;

            /* Drawings */
            Drawing.OnDraw += Drawings.OnDraw;
            Drawing.OnEndScene += DamageIndicatorManager.OnEndScene;

            /* Orbwalker */
            Orbwalker.OnAction += Events.OnAction;

            /* Gapcloser */
            Gapcloser.OnGapcloser += Events.OnGapcloser;
        }

        public static class OrbwalkerModes
        {
            public static void Combo()
            {
                var target = TargetSelector.GetTarget(W.Range);

                if (target == null)
                    return;

                if (MenuManager.ComboMenu["UseW"].GetValue<MenuBool>().Enabled && W.IsReady())
                {
                    var targets = TargetSelector.GetTargets(W.Range);

                    switch (MenuManager.ComboMenu["WSelectBy"].GetValue<MenuList>().SelectedValue)
                    {
                        case "Most AD":
                            targets = targets.OrderByDescending(x => x.TotalAttackDamage).ToList();
                            break;
                        case "Most AP":
                            targets = targets.OrderByDescending(x => x.TotalMagicalDamage).ToList();
                            break;
                        case "Lowest Health":
                            targets = targets.OrderByDescending(x => x.Health).ToList();
                            break;
                        case "Most Priority":
                            targets = targets.OrderByDescending(x => TargetSelector.GetPriority(x) == TargetPriority.Max).ToList();
                            break;
                    }

                    if (Misc.IsAllured(targets.First()) || !targets.First().IsValidTarget(W.Range))
                        return;

                    if (MenuManager.ComboWhiteList[target.CharacterName.ToLower()].GetValue<MenuBool>().Enabled)
                    {
                        W.CastOnUnit(targets.First());
                    }
                }
                if (MenuManager.ComboMenu["UseR"].GetValue<MenuBool>().Enabled && R.IsReady())
                {
                    if (!target.IsValidTarget(R.Range))
                        return;

                    var position = objPlayer.Position.Extend(R.GetPrediction(target).CastPosition, -700);

                    if (MenuManager.ComboMenu["RAoE"].GetValue<MenuSliderButton>().Enabled && position.CountEnemyHeroesInRange(MenuManager.ComboMenu["RSafetyRange"].GetValue<MenuSlider>().Value) >= MenuManager.ComboMenu["RAoE"].GetValue<MenuSliderButton>().Value)
                    {
                        return;
                    }

                    if (MenuManager.ComboWhiteList2[target.CharacterName.ToLower()].GetValue<MenuBool>().Enabled)
                    {
                        R.CastIfWillHit(target, MenuManager.ComboMenu["RAoE"].GetValue<MenuSliderButton>().Value);
                    }
                }
                if (MenuManager.ComboMenu["UseQ"].GetValue<MenuBool>().Enabled && Q.IsReady())
                {
                    if (!target.IsValidTarget(Q.Range))
                        return;

                    if (Misc.IsAllured(target))
                    {
                        if (!Misc.IsFullyAllured(target) || MenuManager.ComboMenu["QOnlyIfFullyAllured"].GetValue<MenuBool>().Enabled)
                            return;

                        if (Misc.IsSpikeSkillShot())
                        {
                            Q.Cast(target);
                        }
                        else
                        {
                            Q.CastOnUnit(target);
                        }
                    }
                    else
                    {
                        Q.Cast(target);
                    }
                }
                if (MenuManager.ComboMenu["UseE"].GetValue<MenuBool>().Enabled && E.IsReady())
                {
                    if (!target.IsValidTarget(E.Range))
                        return;

                    if (Misc.IsAllured(target))
                    {
                        if (!Misc.IsFullyAllured(target) || MenuManager.ComboMenu["EOnlyIfFullyAllured"].GetValue<MenuBool>().Enabled)
                            return;

                        E.CastOnUnit(target);
                    }
                    else
                    {
                        E.CastOnUnit(target);
                    }
                }
            }
            public static void Harass()
            {
                var target = TargetSelector.GetTarget(Misc.GetRealQRange());

                if (target == null)
                    return;

                if (MenuManager.HarassMenu["UseQ"].GetValue<MenuBool>().Enabled && Q.IsReady())
                {
                    if (objPlayer.ManaPercent < ManaManager.GetNeededMana(Q.Slot, MenuManager.HarassMenu["MinMana"].GetValue<MenuSlider>()))
                        return;
                    if (!target.IsValidTarget(Misc.GetRealQRange()))
                        return;

                    if (MenuManager.HarassMenu[target.CharacterName.ToLower()].GetValue<MenuBool>().Enabled)
                    {
                        Q.Cast(target);
                    }
                }
                if (MenuManager.HarassMenu["UseE"].GetValue<MenuBool>().Enabled && E.IsReady())
                {
                    if (objPlayer.ManaPercent < ManaManager.GetNeededMana(E.Slot, MenuManager.HarassMenu["MinMana"].GetValue<MenuSlider>()))
                        return;
                    if (!target.IsValidTarget(E.Range))
                        return;

                    if (MenuManager.HarassMenu[target.CharacterName.ToLower()].GetValue<MenuBool>().Enabled)
                    {
                        E.Cast(target);
                    }
                }
            }
            public static void LaneClear()
            {
                var minions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(Q.Range) && x.IsMinion()).OrderBy(x => x.Health);

                if (minions == null)
                    return;

                if (MenuManager.LaneClearMenu["UseQ"].GetValue<MenuBool>().Enabled && Q.IsReady())
                {
                    if (Misc.IsSpikeSkillShot())
                        return;
                    if (objPlayer.ManaPercent < ManaManager.GetNeededMana(Q.Slot, MenuManager.LaneClearMenu["MinMana"].GetValue<MenuSlider>()))
                        return;

                    foreach (var min in minions)
                    {
                        if (Misc.IsSpikeSkillShot())
                        {
                            Q.Cast(min);
                        }
                        else
                        {
                            Q.CastOnUnit(min);
                        }
                    }
                }
                if (MenuManager.LaneClearMenu["UseE"].GetValue<MenuBool>().Enabled && E.IsReady())
                {
                    foreach (var min in minions)
                    {
                        if (!min.IsValidTarget(E.Range))
                            return;
                        if (objPlayer.ManaPercent < ManaManager.GetNeededMana(E.Slot, MenuManager.LaneClearMenu["MinMana"].GetValue<MenuSlider>()))
                            return;

                        if (min.Name.Contains("Siege") || min.Name.Contains("Super"))
                        {
                            E.CastOnUnit(min);
                        }
                        else
                        {
                            E.CastOnUnit(min);
                        }
                    }
                }
            }
            public static void JungleClear()
            {
                var target = GameObjects.GetJungles(Q.Range).Where(x => x.IsValidTarget(Q.Range)).MinBy(x => x.Health / x.MaxHealth);

                if (target == null)
                    return;

                if (MenuManager.JungleClearMenu["UseW"].GetValue<MenuBool>().Enabled && W.IsReady())
                {
                    if (Misc.IsAllured(target) || !target.IsValidTarget(W.Range))
                        return;
                    if (objPlayer.ManaPercent < ManaManager.GetNeededMana(W.Slot, MenuManager.JungleClearMenu["MinMana"].GetValue<MenuSlider>()))
                        return;
                    if (target.Health <= objPlayer.GetAutoAttackDamage(target))
                        return;

                    var targetName = target.SkinName;

                    if (UtilityManager.JungleList.Contains(targetName) && MenuManager.JungleClearWhiteList[targetName].GetValue<MenuBool>().Enabled)
                    {
                        W.CastOnUnit(target);
                    }
                }

                if (MenuManager.JungleClearMenu["UseE"].GetValue<MenuBool>().Enabled && E.IsReady())
                {
                    if (Misc.IsAllured(target) && !Misc.IsFullyAllured(target))
                        return;
                    if (!target.IsValidTarget(E.Range))
                        return;
                    if (objPlayer.ManaPercent < ManaManager.GetNeededMana(E.Slot, MenuManager.JungleClearMenu["MinMana"].GetValue<MenuSlider>()))
                        return;

                    if (Misc.IsEmpowered())
                    {
                        E.CastOnUnit(target);
                    }
                    else
                    {
                        E.CastOnUnit(target);
                    }
                }

                if (MenuManager.JungleClearMenu["UseQ"].GetValue<MenuBool>().Enabled && Q.IsReady())
                {
                    if (!target.IsValidTarget(Q.Range))
                        return;
                    if (Misc.IsAllured(target) && !Misc.IsFullyAllured(target))
                        return;
                    if (objPlayer.ManaPercent < ManaManager.GetNeededMana(Q.Slot, MenuManager.JungleClearMenu["MinMana"].GetValue<MenuSlider>()))
                        return;

                    if (Misc.IsSpikeSkillShot())
                    {
                        Q.Cast(target);
                    }
                    else
                    {
                        Q.CastOnUnit(target);
                    }
                }
            }
        }
        public static class Events
        {
            public static void OnTick(EventArgs args)
            {
                if (objPlayer.IsDead || objPlayer.IsRecalling() || MenuGUI.IsChatOpen)
                    return;

                if (!objPlayer.GetSpell(SpellSlot.W).State.HasFlag(SpellState.NotLearned))
                {
                    W.Range = 1100 + 100 * objPlayer.GetSpell(SpellSlot.W).Level;
                }

                if (MenuManager.MiscKillSteal["KSEnable"].GetValue<MenuBool>().Enabled)
                {
                    Misc.KillSteal();
                }

                switch (Orbwalker.ActiveMode)
                {
                    case OrbwalkerMode.Combo:
                        OrbwalkerModes.Combo();
                        break;
                    case OrbwalkerMode.Harass:
                        OrbwalkerModes.Harass();
                        break;
                    case OrbwalkerMode.LaneClear:
                        OrbwalkerModes.LaneClear();
                        OrbwalkerModes.JungleClear();
                        break;
                    case OrbwalkerMode.LastHit:
                        break;
                }
            }
            public static void OnAction(object sender, OrbwalkerActionArgs args)
            {
                if (MenuManager.MiscMenu["AASemiAllured"].GetValue<MenuBool>().Enabled && args.Type == OrbwalkerType.BeforeAttack)
                {
                    var OrbwalkerTarget = Orbwalker.GetTarget();

                    if (OrbwalkerTarget == null)
                        return;

                    var BaseTarget = OrbwalkerTarget as AIBaseClient;

                    if (BaseTarget == null)
                        return;

                    if (Misc.IsAllured(BaseTarget) && !Misc.IsFullyAllured(BaseTarget))
                    {
                        args.Process = false;
                    }
                }
            }
            public static void OnGapcloser(AIBaseClient sender, Gapcloser.GapcloserArgs args)
            {
                if (!MenuManager.GapCloserMenu["Enable"].GetValue<MenuBool>().Enabled)
                    return;

                if (objPlayer.IsDead)
                    return;
                if (sender == null || sender.IsAlly || !sender.IsMelee)
                    return;
            }
        }
        public static class Misc
        {
            public static void KillSteal()
            {
                if (MenuManager.MiscKillSteal["KSwithE"].GetValue<MenuBool>().Enabled && E.IsReady())
                {
                    foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(E.Range) && Q.GetDamage(x, IsEmpowered() ? DamageStage.Empowered : DamageStage.Default) >= x.Health + x.MagicalShield))
                    {
                        E.CastOnUnit(target);
                    }
                }
                if (MenuManager.MiscKillSteal["KSwithR"].GetValue<MenuBool>().Enabled && R.IsReady())
                {
                    foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(R.Range) && R.GetDamage(x, x.HealthPercent < 30 ? DamageStage.Empowered : DamageStage.Default) >= x.Health + x.MagicalShield))
                    {
                        R.Cast(target.Position);
                    }
                }
            }
            public static bool IsAllured(AIBaseClient target)
            {
                return target.HasBuff("EvelynnW");
            }
            public static bool IsFullyAllured(AIBaseClient target)
            {
                if (!target.HasBuff("EvelynnW"))
                    return false;

                var normalObjects = ObjectManager.Get<GameObject>().Where(x => x.IsValid && x.Name == "Evelynn_Base_W_Fizz_Mark_Decay");

                return normalObjects.Any(x => ObjectManager.Get<AIBaseClient>().Where(c => c.Team != x.Team).MinBy(c => c.Distance(x)) == target);
            }
            public static bool IsSpikeSkillShot()
            {
                return objPlayer.GetSpell(SpellSlot.Q).ToggleState == 1;
            }
            public static bool IsEmpowered()
            {
                return E.SData.Name == "EvelynnE2";
            }
            public static float GetRealQRange()
            {
                return IsSpikeSkillShot() ? Q.Range : Q2.Range;
            }
        }
        public static class Drawings
        {
            public static void OnDraw(EventArgs args)
            {
                if (MenuManager.DrawingsMenu["Disable"].GetValue<MenuBool>().Enabled)
                    return;

                if (MenuManager.SpellRangesMenu["QRange"].GetValue<MenuBool>().Enabled && Q.IsReady())
                {
                    if (Misc.IsSpikeSkillShot())
                    {
                        Render.Circle.DrawCircle(objPlayer.Position, Q.Range, System.Drawing.Color.Firebrick);
                    }
                    else
                    {
                        Render.Circle.DrawCircle(objPlayer.Position, Q2.Range, System.Drawing.Color.Firebrick);
                    }
                }
                if (MenuManager.SpellRangesMenu["WRange"].GetValue<MenuBool>().Enabled && W.IsReady())
                {
                    Render.Circle.DrawCircle(objPlayer.Position, W.Range, System.Drawing.Color.DodgerBlue);
                }
                if (MenuManager.SpellRangesMenu["ERange"].GetValue<MenuBool>().Enabled && E.IsReady())
                {
                    Render.Circle.DrawCircle(objPlayer.Position, E.Range, System.Drawing.Color.Azure);
                }
                if (MenuManager.SpellRangesMenu["RRange"].GetValue<MenuBool>().Enabled && R.IsReady())
                {
                    Render.Circle.DrawCircle(objPlayer.Position, R.Range, System.Drawing.Color.Cyan);
                }
            }
        }
    }
}
