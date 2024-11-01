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
		public static EditorHelper Instance { get; private set; }

		internal ManualLogSource EH_Logger { get { return Logger; } }

		private void Awake()
		{
			Instance = this;
			foreach (var type in typeof(EditorHelper).Assembly.GetTypes()
				.Where(type => type.IsClass && type.Namespace.Contains("EditorHelper.Tweaks") && type.GetInterface("IEditorTweak") != null))
			{
				try
				{
					IEditorTweak tweak = Activator.CreateInstance(type) as IEditorTweak;
					tweak.StartTweak(Config);
				}
				catch (Exception e)
				{
					Logger.LogError(e);
				}
			}
		}

		private void OnDestroy()
		{
			foreach (var type in typeof(EditorHelper).Assembly.GetTypes()
				.Where(type => type.IsClass && type.Namespace.Contains("EditorHelper.Tweaks") && type.GetInterface("IEditorTweak") != null))
			{
				try
				{
					IEditorTweak tweak = Activator.CreateInstance(type) as IEditorTweak;
					tweak.StopTweak();
				}
				catch (Exception e)
				{
					Logger.LogError(e);
				}
			}
		}
	}
}
