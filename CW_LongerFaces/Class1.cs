using BepInEx;
using HarmonyLib;
using System.Reflection.Emit;
using System.Collections.Generic;
using UnityEngine;
using BepInEx.Logging;
using TMPro;

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
            // harmony.PatchAll(typeof(PlayerCustomizer_CopyPaste));
            // harmony.PatchAll(typeof(PlayerCustomizer_SetFaceText_Patcher));
            harmony.PatchAll(typeof(PlayerCustomizer_RunTerminal_Patcher));
            harmony.PatchAll(typeof(PlayerVisor_RPCA_SetVisorText_Patcher));
        }
    }

    [HarmonyPatch(typeof(PlayerCustomizer))]
    [HarmonyPatch("Awake")]
    public static class PlayerCustomizer_Scale_Patcher // Makes text scaling range a lot broader
    {
        static float scaleAmount = 5f; // Amount to scale original values by
        [HarmonyPostfix]
        public static void Postfix(ref PlayerCustomizer __instance)
        {
            __instance.faceSizeMinMax.x /= scaleAmount;
            __instance.faceSizeMinMax.y *= scaleAmount;

            __instance.visorFaceSizeMinMax.x /= scaleAmount;
            __instance.visorFaceSizeMinMax.y *= scaleAmount;

            __instance.faceSizeStepCount = (int)(scaleAmount * 10);

            ContentWarningMoreFacesMod.BepInExLogSource.LogMessage("faceSizeMinMax: " + __instance.faceSizeMinMax.ToString());
            ContentWarningMoreFacesMod.BepInExLogSource.LogMessage("visorFaceSizeMinMax: " + __instance.visorFaceSizeMinMax.ToString());
        }
    }

    // Text pasting was officially implemented so no need for this anymore
    /*
    [HarmonyPatch(typeof(PlayerCustomizer))]
    [HarmonyPatch("Update")]
    public static class PlayerCustomizer_CopyPaste // Allows you to paste text into the customizer
    {
        [HarmonyPostfix]
        public static void Postfix(ref TextMeshProUGUI ___faceText, ref PlayerCustomizer __instance)
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V))
            {
                ___faceText.text += GUIUtility.systemCopyBuffer; // Paste from clipboard
                __instance.typeSound.Play(__instance.gameObject.transform.position, false, 1f, null); // Play the typing sound
                ContentWarningMoreFacesMod.BepInExLogSource.LogMessage("Pasted text from clipboard.");
                ContentWarningMoreFacesMod.BepInExLogSource.LogMessage(___faceText.text);
            }
        }
    }
    */

    // Function removed from game as of May 3 update
    /*
    [HarmonyPatch(typeof(PlayerCustomizer))]
    [HarmonyPatch("SetFaceText")]
    class PlayerCustomizer_SetFaceText_Patcher
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

            ContentWarningMoreFacesMod.BepInExLogSource.LogMessage("Ran character limit patcher.");
            return code;
        }
    }
    */ 

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
    [HarmonyPatch("SafetyCheckVisorText")]
    class PlayerVisor_RPCA_SetVisorText_Patcher
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

            ContentWarningMoreFacesMod.BepInExLogSource.LogMessage("Ran visor patcher.");
            return code;
        }
    }
}
