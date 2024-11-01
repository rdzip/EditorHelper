using BepInEx.Configuration;
using HarmonyLib;
using RDLevelEditor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EditorHelper.Tweaks.TrueCameraHelper
{
	public class TrueCameraHelper : IEditorTweak
	{
		private Harmony harmony;

		public void StartTweak(ConfigFile config)
		{
			harmony = Harmony.CreateAndPatchAll(typeof(TrueCamPatch));
		}

		public void StopTweak()
		{
			harmony.UnpatchSelf();
		}

		public static class TrueCamPatch
		{
			[HarmonyPatch(typeof(scnEditor), "Start")]
			[HarmonyPostfix]
			public static void AddInspector(scnEditor __instance)
			{
				//var origInspector = __instance.inspectorPanelManager.Get<InspectorPanel_MoveCamera>();
				//var inspector = GameObject.Instantiate(origInspector, origInspector.transform.parent);
				//var camera = inspector.gameObject.AddComponent<InspectorPanel_TrueCamera>();
				//camera.cameraPosition = inspector.cameraPosition;
				//camera.zoom = inspector.zoom;
				//camera.angle = inspector.angle;
				//camera.duration = inspector.duration;
				//camera.ease = inspector.ease;
				//camera.real = inspector.real;
				//camera.cameraPositionLabel = inspector.cameraPositionLabel;
				//camera.zoomLabel = inspector.zoomLabel;
				//camera.angleLabel = inspector.angleLabel;
				//inspector.DisableComponent<InspectorPanel_MoveCamera>();
				//((Dictionary<Type, InspectorPanel>)__instance.inspectorPanelManager
				//	.Field("inspectorPanels").GetValue(__instance.inspectorPanelManager))
				//	.Add(typeof(InspectorPanel_TrueCamera), camera);
			}

			[HarmonyPatch(typeof(SelectLevelEventPanel), "Awake")]
			[HarmonyPostfix]
			public static void AddSelectEventButton(SelectLevelEventPanel __instance)
			{
				if (__instance.willDisplayLevelEventsFromTab != Tab.Actions) return;
				var origButton = __instance.events.Find((levelEvent) => levelEvent.type == LevelEventType.MoveCamera);

				var button = GameObject.Instantiate(origButton, origButton.transform.parent);
				button.transform.SetSiblingIndex(origButton.transform.GetSiblingIndex() + 1);
				button.label.text = "True Camera";
				button.shortcut.text = "";
				button.type = (LevelEventType)(10000 + (int)LevelEventType.MoveCamera);
				button.button.onClick.RemoveAllListeners();
				button.button.onClick.AddListener(() =>
				{
					__instance.editor.conditionalsPanel.currentPanel = null;
					__instance.editor.SetLevelEventControlType(LevelEventType.Comment, false);
					((LevelEvent_Comment)__instance.editor.selectedControl.levelEvent).text = "()=>trueCameraMove(0,0,0,1,Linear)";
					__instance.editor.selectedControl.levelEvent.inspectorPanel.UpdateUI(__instance.editor.selectedControl.levelEvent);
					__instance.LevelEditorPlaySound("sndEditorEventSetType", "LevelEditorParent", 1f, 1f, 0f);
				});
				__instance.events.Insert(__instance.events.IndexOf(origButton) + 1, button);
			}

			[HarmonyPatch(typeof(LevelEventControl_Action), "UpdateUIInternal")]
			public static class EventControlUIPatch
			{
				private static Color initialColor;
				private static Color imageColor;

				public static void Prefix(LevelEventControl_Action __instance)
				{
					if (!(__instance.levelEvent is LevelEvent_Comment) ||
						!((LevelEvent_Comment)__instance.levelEvent).text.StartsWith("()=>trueCameraMove("))
						return;

					initialColor = __instance.initialColor;
					imageColor = __instance.image.color;
				}

				public static void Postfix(LevelEventControl_Action __instance)
				{
					if (!(__instance.levelEvent is LevelEvent_Comment) ||
						!((LevelEvent_Comment)__instance.levelEvent).text.StartsWith("()=>trueCameraMove("))
						return;

					__instance.initialColor = initialColor;
					__instance.image.color = imageColor;
					__instance.icon.sprite = LevelEventControl_Base.GetIconFromName("LevelEvent_MoveCamera");
				}
			}
		}
	}
}
