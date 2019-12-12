using System;
using System.Linq;
using SharpDX;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI.Values;

using Olympus.AIO.SDK.Helpers;
using Extensions = Olympus.AIO.SDK.Helpers.Extensions;

namespace Olympus.AIO.SDK
{
    internal class DamageIndicatorManager
    {
        public static void OnEndScene(EventArgs args)
        {
            if (MenuManager.DrawingsMenu["Disable"].GetValue<MenuBool>().Enabled)
                return;

            if (MenuManager.DamageIndicatorMenu["Enable"].GetValue<MenuBool>().Enabled)
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(2000) && !x.IsDead && x.IsHPBarRendered))
                {
                    Vector2 pos = Drawing.WorldToScreen(target.Position);

                    if (!pos.IsOnScreen())
                        return;

                    float damage = Extensions.GetComboDamageByChampion(target, true, true, true, true);

                    var hpBar = target.HPBarPosition;

                    if (damage > target.Health)
                    {
                        Drawing.DrawText(hpBar.X + 69, hpBar.Y - 45, System.Drawing.Color.White, "KILLABLE");
                    }

                    var damagePercentage = ((target.Health - damage) > 0 ? (target.Health - damage) : 0) / target.MaxHealth;
                    var currentHealthPercentage = target.Health / target.MaxHealth;

                    var startPoint  = new Vector2(hpBar.X - 45 + damagePercentage * 104, hpBar.Y - 18);
                    var endPoint    = new Vector2(hpBar.X - 45 + currentHealthPercentage * 104, hpBar.Y - 18);

                    Drawing.DrawLine(startPoint, endPoint, 12, System.Drawing.Color.Gold);
                }
            }
        }
    }
}
