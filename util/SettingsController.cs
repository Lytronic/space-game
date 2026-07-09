using Godot;
using Microgravity.util;

/// <summary>
/// This class is one of two singletons for settings management, with the other one being <c>SettingsModel</c>.
/// Both of them are instantiated by the game engine's Autoload system.
///
/// They implement the MVC pattern, which means that the actual settings state is not stored here but in the model.
/// Here, we mainly make sure that the settings in our dictionary are synchronised with the keybind system.
/// </summary>
public partial class SettingsController : Node
{
	public static SettingsController Instance { get; private set; }

	public override void _Ready()
	{
		Instance = this;

		// initialise the model *before* we access settings data
		SettingsModel.Instance.Init();
		SettingsModel.Instance.SettingChanged += ChangedCallback;

		// Register keybinds from Settings in Godot's keybind system
		foreach (var entry in SettingsModel.Instance.Settings)
		{
			if (entry.Value is SettingsEntry.Keybind keybind)
			{
				// settings keys follow the pattern "<category>.<action_name>", e. g. "controls.forward"
				StringName name = entry.Key.Split(".")[1];
				if (!InputMap.HasAction(name))
					InputMap.AddAction(name);
				else
					InputMap.ActionEraseEvents(name);

				var inputEvent = new InputEventKey
				{
					Keycode = OS.FindKeycodeFromString(keybind.Value)
				};

				InputMap.ActionAddEvent(name, inputEvent);
			}
		}

		UpdateFullscreen();
	}	

	/// <summary>
	/// Update a keybind both in the SettingsModel and in Godot's Input system
	/// The model is responsible for saving it to the database.
	/// </summary>
	public void SetKeybind(string settingsKey, Key keycode)
	{
		if (!SettingsModel.Instance.Settings.TryGetValue(settingsKey, out SettingsEntry value))
		{
			GD.Print($"Error: Attempting to set nonexistent keybind {settingsKey}!");
			return;
		}
		
		// update model
		if (value is SettingsEntry.Keybind kb)
		{
			SettingsModel.Instance.SetEntry(settingsKey, kb with { Value = OS.GetKeycodeString(keycode) });
		}

		// update InputMap
		StringName name = settingsKey.Split(".")[1];
		InputMap.ActionEraseEvents(name);
		InputMap.ActionAddEvent(name, new InputEventKey { Keycode = keycode });
	}

	/// <summary>
	/// Set the fullscreen state in the display server.
	/// </summary>
	private void UpdateFullscreen()
	{
		bool enabled = ((SettingsEntry.Bool)SettingsModel.Instance.Settings["video.fullscreen"]).Value;
		DisplayServer.WindowSetMode(enabled ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Windowed);
	}

	/// <summary>
	/// Update state elsewhere when a setting changes.
	/// </summary>
	private void ChangedCallback(string key)
	{
		switch (key)
		{
			case "video.fullscreen":
				UpdateFullscreen();
				break;
		}
	}
}
