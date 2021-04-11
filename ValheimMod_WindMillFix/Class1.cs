using BepInEx;
using HarmonyLib;
using UnityEngine;
using BepInEx.Configuration;

namespace ValheimMod_WindMillFix
{
    [BepInPlugin("SkYMaN.ValheimMod_WindMillUtilities", "WindMillUtilities", "1.0.2")]
    [BepInProcess("valheim.exe")]
    public class Valheim_WindMillUtilities : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("SkYMaN.ValheimMod_WindMillUtilities");
        private static ConfigEntry<float> configEntry_WindMillVolume;
        private static ConfigEntry<float> configEntry_WindMillRotationSpeedMultiplier;
        void Awake()
        {
            Debug.Log("Starting loading mod - WindMill Utilities");
            harmony.PatchAll();
            configEntry_WindMillVolume = Config.Bind<float>("General", "WindMill Volume", 1f, "WindMill Volume");
            configEntry_WindMillRotationSpeedMultiplier = Config.Bind<float>("General", "Wind Mill Rotation Speed Multiplier", 1000f, "Wind Mill Rotation Speed Multiplier");
            Debug.Log("Finish loading mod - WindMill Utilities");
        }
        void OnDestroy()
        {
            harmony.UnpatchSelf();    
        }
        protected static float GetPowerOutput_result;
        [HarmonyPatch(typeof(Windmill), nameof(Windmill.GetPowerOutput))]
        class Patch_GetPower
        {
            static void Postfix(ref float __result)
            {
                GetPowerOutput_result = __result;
            }
        }
        
        [HarmonyPatch(typeof(Windmill), "Update")]
        class Patch_WindMillAnimation
        {
            static void Prefix(ref float ___m_propellerRotationSpeed, ref float ___m_maxVol, ref Smelter ___m_smelter)
            {
                if (___m_smelter.IsActive())
                {
                    ___m_propellerRotationSpeed = GetPowerOutput_result * configEntry_WindMillRotationSpeedMultiplier.Value;
                    ___m_maxVol = configEntry_WindMillVolume.Value;
                    //Debug.Log("Active:   " + ___m_propellerRotationSpeed + "\t::\t" + GetPowerOutput_result + "\t::\t" + time);
                }
                else
                {
                    ___m_propellerRotationSpeed = 0f;
                    ___m_maxVol = 0;
                }
            }
        }
    }

}
