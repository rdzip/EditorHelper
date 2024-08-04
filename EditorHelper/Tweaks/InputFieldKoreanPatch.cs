using BepInEx.Configuration;
using HarmonyLib;
using RDLevelEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

namespace EditorHelper.Tweaks
{
	public class InputFieldKoreanPatch : IEditorTweak
	{
		private Harmony harmony;

		public void StartTweak(ConfigFile config)
		{
			harmony = Harmony.CreateAndPatchAll(typeof(KoreanPatch));
		}

		public void StopTweak()
		{
			harmony.UnpatchSelf();
		}

		public static class KoreanPatch
		{
			[HarmonyPatch(typeof(InspectorPanel_Comment), "LateUpdate")]
			[HarmonyPostfix]
			public static void InputOtherLanguage(InspectorPanel_Comment __instance)
			{
				RDEditorUtils.UpdateUIText(__instance.dialogue.textComponent, __instance.dialogue.textComponent.text, false);
			}
		}
	}
}
