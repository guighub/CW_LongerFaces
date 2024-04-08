using BepInEx;
using HarmonyLib;
using System.Reflection.Emit;
using System.Collections.Generic;
using UnityEngine;
using BepInEx.Logging;

namespace CW_MoreFacesMod
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class ContentWarningMoreFacesMod : BaseUnityPlugin
    {
        public const string modGUID = "guiguig.CW_LongerFacesMod";
        public const string modName = "Content Warning Longer Faces Mod";
        public const string modVersion = "1.0.0";
        private readonly Harmony harmony = new Harmony(modGUID); // Creating a Harmony instance which will run the mod

        public static ManualLogSource BepInExLogSource = BepInEx.Logging.Logger.CreateLogSource(modGUID);

        void Awake() // runs when Content Warning is launched
        {
            BepInExLogSource.LogMessage(modGUID + " has loaded successfully.");

            harmony.PatchAll(typeof(PlayerCustomizer_Scale_Patcher));
            harmony.PatchAll(typeof(PlayerCustomizer_SetFaceText_Patcher));
            harmony.PatchAll(typeof(PlayerCustomizer_RunTerminal_Patcher));
            harmony.PatchAll(typeof(PlayerVisor_RPCA_SetVisorText_Patcher));
        }
    }

    [HarmonyPatch(typeof(PlayerCustomizer), nameof(PlayerCustomizer.OnChangeFaceSize))]
    public static class PlayerCustomizer_Scale_Patcher
    {
        static float scaleAmount = 2f; // Amount to scale the default setting by
        static int steps = 20; // Number of scaling steps
        public static void Prefix(ref Vector2 ___visorFaceSizeMinMax, ref Vector2 ___faceSizeMinMax, ref int ___faceSizeStepCount)
        {
            ___visorFaceSizeMinMax = new Vector2(0.025f / scaleAmount, 0.035f * scaleAmount);
            // I found that the below line actually broke the scaling of the faces and caused desync between the preview and actual visor, so I've left it out.
            //___faceSizeMinMax = new Vector2(0.025f, 0.035f * 2f);
            ___faceSizeStepCount = steps;

            ContentWarningMoreFacesMod.BepInExLogSource.LogMessage("Ran size limit patcher.");
        }
    }

    [HarmonyPatch(typeof(PlayerCustomizer))]
    [HarmonyPatch("SetFaceText")]
    class PlayerCustomizer_SetFaceText_Patcher
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = new List<CodeInstruction>(instructions);

            for (int i = 0; i < code.Count; i++)
            {
                // Change any occurence of "3" in the method to be the integer limit
                if (code[i].opcode == OpCodes.Ldc_I4_3)
                {
                    code[i] = new CodeInstruction(OpCodes.Ldc_I4, int.MaxValue);
                }
            }

            ContentWarningMoreFacesMod.BepInExLogSource.LogMessage("Ran character limit patcher.");
            return code;
        }
    }

    [HarmonyPatch(typeof(PlayerCustomizer))]
    [HarmonyPatch("RunTerminal")]
    class PlayerCustomizer_RunTerminal_Patcher
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            for (int i = 0; i < code.Count; i++)
            {
                // Change any occurence of "3" in the method to be the integer limit
                if (code[i].opcode == OpCodes.Ldc_I4_3)
                {
                    code[i] = new CodeInstruction(OpCodes.Ldc_I4, int.MaxValue);
                }
            }
            
            ContentWarningMoreFacesMod.BepInExLogSource.LogMessage("Ran typing patcher.");
            return code;
        }
    }

    [HarmonyPatch(typeof(PlayerVisor))]
    [HarmonyPatch("RPCA_SetVisorText")]
    class PlayerVisor_RPCA_SetVisorText_Patcher
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = new List<CodeInstruction>(instructions);

            for (int i = 0; i < code.Count; i++)
            {
                // Change any occurence of "3" in the method to be the integer limit
                if (code[i].opcode == OpCodes.Ldc_I4_3)
                {
                    code[i] = new CodeInstruction(OpCodes.Ldc_I4, int.MaxValue);
                }
            }

            ContentWarningMoreFacesMod.BepInExLogSource.LogMessage("Ran visor patcher.");
            return code;
        }
    }
}
