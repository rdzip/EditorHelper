using RDLevelEditor;
using System;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

namespace EditorHelper.Tweaks.TrueCameraHelper
{
	public class InspectorPanel_TrueCamera : InspectorPanel_MoveCamera
	{
		private string offString;

		private LevelEvent_Comment trueCamera
		{
			get
			{
				return currentLevelEvent as LevelEvent_Comment;
			}
		}

		private new void Awake()
		{
			name = "TrueCamera";
			base.Awake();
			AddOnEditListeners(null, new object[]
			{
				cameraPosition,
				duration,
				ease,
			});
			duration.transform.parent.position = zoom.transform.parent.position;
			ease.transform.parent.position = angle.transform.parent.position;
			Destroy(zoom.transform.parent.gameObject);
			Destroy(angle.transform.parent.gameObject);
			ease.AddOptionsWithEnumArray(RDEditorConstants.availableEases, true);
			offString = " " + RDString.Get("editor.off");
		}

		public override void UpdateUIInternal(LevelEvent_Base levelEvent)
		{
			LevelEvent_Comment levelEvent_Comment = (LevelEvent_Comment)levelEvent;

			var text = levelEvent_Comment.text.Remove(0, "()=>".Length);
			text = text.RemoveTextIfStartsWith("trueCameraMove");

			string[] parameters = text.GetParameters();
			if (parameters != null && parameters.Length == 5)
			{
				int room = RDEditorUtils.DecodeInt(parameters[0]);
				float x = RDEditorUtils.DecodeFloat(parameters[1]);
				float y = RDEditorUtils.DecodeFloat(parameters[2]);
				float duration = RDEditorUtils.DecodeFloat(parameters[3]);
				Ease ease = RDEditorUtils.DecodeEnum<Ease>(parameters[4]);

				this.duration.text = duration.ToString();
				cameraPosition.pixels = new Float2(x, y);
				this.ease.value = Array.IndexOf(RDEditorConstants.availableEases, ease);
				position.rooms = new int[] { room };

				//targetRoomCam.DOKill();
				//if (durationTime == 0f)
				//{
				//	targetRoomCam.localPosition = targetVector;
				//}
				//else
				//{
				//	targetRoomCam.DOLocalMove(targetVector, durationTime).OnComplete(delegate
				//	{
				//		targetRoomCam.localPosition = targetVector;
				//	}).SetEase(ease);
				//}
			}

			
		}

		protected override void SaveInternal(LevelEvent_Base levelEvent)
		{
			LevelEvent_Comment obj = (LevelEvent_Comment)levelEvent;
			string text =
				$"()=>trueCameraMove({position.rooms[0]},{cameraPosition.pixels.x},{cameraPosition.pixels.y},{float.Parse(duration.text)},{Enum.GetName(typeof(Ease), RDEditorConstants.availableEases[ease.value])})";
			obj.text = text;
		}
	}
}
