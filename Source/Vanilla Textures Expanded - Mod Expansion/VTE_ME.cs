// This project is subject to the terms of the Mozilla Public License v2.0
// If a copy of the MPL was not distributed with this file,
// You can obtain one at https://mozilla.org/MPL/2.0/

using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace VTE_ME
{
    [StaticConstructorOnStartup]
    public static class VTE_ME
    {
        public static bool CharEditLoaded =>  ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name == "Character Editor" || m.PackageId == "void.charactereditor");
    }

    [StaticConstructorOnStartup]
    internal static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            if (!VTE_ME.CharEditLoaded)
            {
                Log.Message("[VTE_ME] Character Editor is not loaded, skipping patch. Check returned: " + VTE_ME.CharEditLoaded);
                return;
            }
            
            var harmony = new Harmony("net.netrve.vte_me");

            var charEditTarget = AccessTools.TypeByName("CharacterEditor.DefTool")?.GetMethod("GetCreateMainButton");
            var postfix = AccessTools.TypeByName("VTE_ME.Patch_CharacterEditor")?.GetMethod("AddIcon");

            if (charEditTarget == null)
            {
                Log.Message("[VTE_ME] Character Editor method could not be found.");
                return;
            }

            if (postfix == null)
            {
                Log.Message("[VTE_ME] Postfix method could not be found.");
                return;
            }
            
            Log.Message("[VTE_ME] Applying Harmony patch.");
            harmony.Patch(charEditTarget, postfix: new HarmonyMethod(postfix));
        }
    }

    internal static class Patch_CharacterEditor 
    {
        public static void AddIcon(ref MainButtonDef __result)
        {
            __result.iconPath = "UI/Buttons/MainButtons/IconCharacterEditor";
            Log.Message("[VTE_ME] iconPath set.");
        }
    }
}
