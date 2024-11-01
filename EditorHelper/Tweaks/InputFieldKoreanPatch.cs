using BepInEx.Configuration;
using HarmonyLib;
using RDLevelEditor;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

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
			[HarmonyPatch(typeof(scnEditor), "LateUpdate")]
			[HarmonyPostfix]
			public static void InputOtherLanguage(scnEditor __instance)
			{
				if (__instance.inspectorPanelManager.GetCurrent() is InspectorPanel_Comment)
				{
					var panel = __instance.inspectorPanelManager.GetCurrent() as InspectorPanel_Comment;
					foreach (Property property in panel.properties)
					{
						if (property.name == "text")
						{
							PropertyControl_InputField inputField = (PropertyControl_InputField)property.control;
							if (inputField.inputField.isFocused)
								RDEditorUtils.UpdateUIText(inputField.inputField.textComponent, inputField.inputField.textComponent.text, false);
						}
					}
				}
			}
		}
	}
}
