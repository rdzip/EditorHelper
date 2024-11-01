using BepInEx.Configuration;
using HarmonyLib;
using RDLevelEditor;
using UnityEngine.UI;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;

namespace EditorHelper.Tweaks
{
	public class EditorPitch : IEditorTweak
	{
		private Harmony harmony;

		private static ConfigEntry<float> slowSpeed;
		private static ConfigEntry<float> fastSpeed;

		private static GameObject root;
		private static HoverCheck hover;
		private static GameObject control;
		private static Image leftImage;
		private static Image rightImage;
		private static Text speedText;

		public void StartTweak(ConfigFile config)
		{
			slowSpeed = config.Bind(GetType().Name, "SlowSpeed", 0.75f, new ConfigDescription("Slow speed.", new AcceptableValueRange<float>(0.01f, 1f)));
			fastSpeed = config.Bind(GetType().Name, "FastSpeed", 1.5f, new ConfigDescription("Fast speed.", new AcceptableValueRange<float>(1f, 5f)));

			harmony = Harmony.CreateAndPatchAll(typeof(PitchPatch));
		}

		public void StopTweak()
		{
			harmony.UnpatchSelf();
			if (root != null) GameObject.DestroyImmediate(root);
		}

		public static class PitchPatch
		{
			[HarmonyPatch(typeof(scnEditor), "Start")]
			[HarmonyPostfix]
			public static void InitControl(scnEditor __instance)
			{
				Vector3 pos = __instance.metronome.rectTransform.localPosition + new Vector3(-70f, -1f);

				if (root != null) GameObject.DestroyImmediate(root);

				root = new GameObject("Speed");
				root.transform.SetParent(__instance.metronome.transform.parent.parent, false);
				root.transform.localPosition = pos;
				var rect = root.AddComponent<RectTransform>();
				rect.pivot = new Vector2(0.5f, 0.5f);
				rect.sizeDelta = new Vector2(50f, 10f);
				hover = root.AddComponent<HoverCheck>();

				control = new GameObject("Speed");
				control.transform.SetParent(root.transform, false);
				rect = control.AddComponent<RectTransform>();
				rect.pivot = new Vector2(0.5f, 0.5f);
				rect.sizeDelta = new Vector2(50f, 10f);

				var btntex = new Texture2D(6, 6);
				btntex.LoadImage(File.ReadAllBytes(EditorHelper.pluginPath + "button.png"));
				btntex.filterMode = FilterMode.Point;

				var btnSprite = Sprite.Create(btntex, new Rect(0, 0, btntex.width, btntex.height), new Vector2(0.5f, 0.5f));

				var speedLeft = new GameObject("SpeedLeft");
				speedLeft.transform.SetParent(control.transform, false);
				rect = speedLeft.AddComponent<RectTransform>();
				rect.anchorMin = new Vector2(0f, 0.5f);
				rect.anchorMax = new Vector2(0f, 0.5f);
				rect.pivot = new Vector2(0.5f, 0.5f);
				rect.sizeDelta = new Vector2(6f, 6f);
				rect.ScaleX(-1f);
				leftImage = speedLeft.AddComponent<Image>();
				leftImage.sprite = btnSprite;
				var button = speedLeft.AddComponent<Button>();
				button.colors = __instance.playPauseButton.colors;
				button.onClick.AddListener(() =>
				{
					if (Input.GetKey(KeyCode.LeftShift))
					{
						fastSpeed.Value -= Input.GetKey(KeyCode.LeftAlt) ? 0.01f : 0.1f;
						fastSpeed.Value = Mathf.Clamp(fastSpeed.Value, 1f, 5f);
						fastSpeed.Value = Mathf.Round(fastSpeed.Value * 100f) / 100f;
					}
					else if (Input.GetKey(KeyCode.LeftControl))
					{
						slowSpeed.Value -= Input.GetKey(KeyCode.LeftAlt) ? 0.01f : 0.1f;
						slowSpeed.Value = Mathf.Clamp(slowSpeed.Value, 0.01f, 1f);
						slowSpeed.Value = Mathf.Round(slowSpeed.Value * 100f) / 100f;
					}
				});
				var outline = speedLeft.AddComponent<Outline>();
				outline.effectColor = new Color(0.2547f, 0.2547f, 0.2547f);
				outline.effectDistance = new Vector2(1f, 1f);

				var speedRight = new GameObject("SpeedRight");
				speedRight.transform.SetParent(control.transform, false);
				rect = speedRight.AddComponent<RectTransform>();
				rect.anchorMin = new Vector2(1f, 0.5f);
				rect.anchorMax = new Vector2(1f, 0.5f);
				rect.pivot = new Vector2(0.5f, 0.5f);
				rect.sizeDelta = new Vector2(6f, 6f);
				rightImage = speedRight.AddComponent<Image>();
				rightImage.sprite = btnSprite;
				button = speedRight.AddComponent<Button>();
				button.colors = __instance.playPauseButton.colors;
				button.onClick.AddListener(() =>
				{
					if (Input.GetKey(KeyCode.LeftShift))
					{
						fastSpeed.Value += Input.GetKey(KeyCode.LeftAlt) ? 0.01f : 0.1f;
						fastSpeed.Value = Mathf.Clamp(fastSpeed.Value, 1f, 5f);
						fastSpeed.Value = Mathf.Round(fastSpeed.Value * 100f) / 100f;
					}
					else if (Input.GetKey(KeyCode.LeftControl))
					{
						slowSpeed.Value += Input.GetKey(KeyCode.LeftAlt) ? 0.01f : 0.1f;
						slowSpeed.Value = Mathf.Clamp(slowSpeed.Value, 0.01f, 1f);
						slowSpeed.Value = Mathf.Round(slowSpeed.Value * 100f) / 100f;
					}
				});
				outline = speedRight.AddComponent<Outline>();
				outline.effectColor = new Color(0.2547f, 0.2547f, 0.2547f);
				outline.effectDistance = new Vector2(1f, 1f);

				var speedText = new GameObject("SpeedText");
				speedText.transform.SetParent(control.transform, false);
				rect = speedText.AddComponent<RectTransform>();
				rect.anchorMin = new Vector2(0.5f, 0.5f);
				rect.anchorMax = new Vector2(0.5f, 0.5f);
				rect.pivot = new Vector2(0.5f, 0.5f);
				rect.sizeDelta = new Vector2(25f, 10f);
				outline = speedText.AddComponent<Outline>();
				outline.effectColor = new Color(0.2547f, 0.2547f, 0.2547f);
				outline.effectDistance = new Vector2(1f, 1f);
				var text = speedText.AddComponent<Text>();
				EditorPitch.speedText = text;
				text.font = __instance.gamePreviewScaleText.font;
				text.fontSize = __instance.gamePreviewScaleText.fontSize;
				text.alignment = TextAnchor.MiddleCenter;
				text.text = "x1.0";
			}

			[HarmonyPatch(typeof(scnEditor), "Update")]
			[HarmonyPostfix]
			public static void OnUpdate(scnEditor __instance)
			{
				control.SetActive(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift));
				if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift)) return;
				Color color = Input.GetKey(KeyCode.LeftShift) ? Color.red : Color.blue;
				var value = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : slowSpeed;

				if (hover.isHover)
				{
					var mouseScrollDelta = Input.mouseScrollDelta;
					if (Mathf.Abs(Mathf.Abs(mouseScrollDelta.y)) > 0.05f)
					{
						if (mouseScrollDelta.y > 0f)
						{
							value.Value += Input.GetKey(KeyCode.LeftAlt) ? 0.01f : 0.1f;
						}
						else if (mouseScrollDelta.y < 0f)
						{
							value.Value -= Input.GetKey(KeyCode.LeftAlt) ? 0.01f : 0.1f;
						}
						if (Input.GetKey(KeyCode.LeftShift))
							value.Value = Mathf.Clamp(value.Value, 1f, 5f);
						else
							value.Value = Mathf.Clamp(value.Value, 0.01f, 1f);
						value.Value = Mathf.Round(value.Value * 100f) / 100f;
					}
				}

				speedText.color = color;
				speedText.text = "x" + value.Value.ToString("0.0#");
			}

			[HarmonyPatch(typeof(scnEditor), "LevelSpeed", MethodType.Getter)]
			[HarmonyPostfix]
			public static void LevelSpeedPatch(ref float __result)
			{
				if (Input.GetKey(KeyCode.LeftShift))
					__result = fastSpeed.Value;
				if (Input.GetKey(KeyCode.LeftControl))
					__result = slowSpeed.Value;
			}
		}

		public class HoverCheck : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
		{
			public bool isHover = false;

			public void OnPointerEnter(PointerEventData eventData)
			{
				isHover = true;
			}

			public void OnPointerExit(PointerEventData eventData)
			{
				isHover = false;
			}
		}
	}
}
