using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace MobCap
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.frizzqol.mobcap";
        public const string PluginName = "FrizzQOL Mob Cap";
        public const string PluginVersion = "0.3.0";

        internal static Plugin Instance { get; private set; }

        private Harmony _harmony;
        private ConfigEntry<int> _maxStars;
        private ConfigEntry<int> _twoStarsAfterDay;

        private void Awake()
        {
            Instance = this;
            _maxStars = Config.Bind(
                "General",
                "MaxStars",
                2,
                "Most stars a new creature can spawn with once 2 stars are allowed. 2 is the normal Valheim maximum. Creatures already alive keep their stars.");
            _twoStarsAfterDay = Config.Bind(
                "General",
                "TwoStarsAfterDay",
                0,
                "2 star creatures cannot spawn before this day. 0 means 2 stars are allowed from day 0. Example: 100 blocks 2 stars on days 0 through 99.");
            MobCapState.MaxStars = _maxStars;
            MobCapState.TwoStarsAfterDay = _twoStarsAfterDay;
            _harmony = new Harmony(PluginGuid);
            try
            {
                _harmony.PatchAll();
                Logger.LogInfo($"{PluginName} {PluginVersion} loaded");
            }
            catch (System.Exception ex)
            {
                Logger.LogError($"Harmony patch failed: {ex.Message}");
            }
        }

        internal static void LogInfo(string message)
        {
            if (Instance != null)
            {
                Instance.Logger.LogInfo(message);
            }
        }

        internal static void LogWarning(string message)
        {
            if (Instance != null)
            {
                Instance.Logger.LogWarning(message);
            }
        }
    }

    internal static class MobCapState
    {
        internal static ConfigEntry<int> MaxStars;
        internal static ConfigEntry<int> TwoStarsAfterDay;
    }
}
