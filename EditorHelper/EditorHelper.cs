using BepInEx;
using BepInEx.Logging;
using EditorHelper.Tweaks;
using System;
using System.Linq;

namespace EditorHelper
{
	[BepInPlugin("com.rhythmdr.editorhelper", "EditorHelper", "0.1")]
	public class EditorHelper : BaseUnityPlugin
	{
		public const string pluginPath = "BepInEx/plugins/EditorHelper/";

		private void Awake()
		{
			foreach (var type in typeof(EditorHelper).Assembly.GetTypes()
				.Where(type => type.IsClass && type.Namespace.Contains("EditorHelper.Tweaks") && type.GetInterface("IEditorTweak") != null))
			{
				IEditorTweak tweak = Activator.CreateInstance(type) as IEditorTweak;
				tweak.StartTweak(Config);
			}
		}

		private void OnDestroy()
		{
			foreach (var type in typeof(EditorHelper).Assembly.GetTypes()
				.Where(type => type.IsClass && type.Namespace.Contains("EditorHelper.Tweaks") && type.GetInterface("IEditorTweak") != null))
			{
				IEditorTweak tweak = Activator.CreateInstance(type) as IEditorTweak;
				tweak.StopTweak();
			}
		}
	}
}
