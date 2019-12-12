using EnsoulSharp;
using EnsoulSharp.SDK;

using Olympus.AIO.Champions;

namespace Olympus.AIO.SDK.Helpers
{
    internal class Extensions
    {
        public static float GetComboDamageByChampion(AIBaseClient target, bool q = false, bool w = false, bool e = false, bool r = false)
        {
            float damage = 0;

            if (target == null || target.IsDead)
                return 0;
            if (target.HasBuffOfType(BuffType.Invulnerability))
                return 0;

            switch (OlympusAIO.objPlayer.CharacterName)
            {
                case "Evelynn":
                    if (q && Evelynn.Q.IsReady())
                    {
                        damage += Evelynn.Q.GetDamage(target, Evelynn.Misc.IsEmpowered() ? DamageStage.Empowered : DamageStage.Default);
                    }
                    if (e && Evelynn.E.IsReady())
                    {
                        damage += Evelynn.E.GetDamage(target);
                    }
                    if (r && Evelynn.R.IsReady())
                    {
                        damage += Evelynn.R.GetDamage(target, target.HealthPercent < 30 ? DamageStage.Empowered : DamageStage.Default);
                    }
                    break;
            }

            if (target.HasBuff("ManaBarrier") && target.HasBuff("BlitzcrankManaBarrierCO"))
                damage += target.Mana / 2f;
            if (target.HasBuff("GarenW"))
                damage = damage * 0.7f;

            return damage;
        }
    }
}
