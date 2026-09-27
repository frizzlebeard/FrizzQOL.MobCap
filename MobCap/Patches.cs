using HarmonyLib;

namespace MobCap
{
    [HarmonyPatch(typeof(Character), nameof(Character.SetLevel))]
    internal static class SetLevelPatch
    {
        private static void Postfix(Character __instance, int level)
        {
            if (__instance == null || __instance.IsPlayer() || level < 1)
            {
                return;
            }

            int maxStars = MobCapState.MaxStars != null ? MobCapState.MaxStars.Value : 2;
            int twoStarsAfterDay = MobCapState.TwoStarsAfterDay != null ? MobCapState.TwoStarsAfterDay.Value : 0;
            int day = 0;
            if (EnvMan.instance != null)
            {
                day = EnvMan.instance.GetDay();
            }

            maxStars = MobCapRules.StarsForDay(day, twoStarsAfterDay, maxStars);

            int capped = MobCapRules.CappedCreatureLevel(level, maxStars);
            if (capped != level)
            {
                __instance.SetLevel(capped);
            }
        }
    }
}
