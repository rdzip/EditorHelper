using BepInEx.Configuration;

namespace EditorHelper.Tweaks
{
	public interface IEditorTweak
	{
		void StartTweak(ConfigFile config);

		void StopTweak();
	}
}
