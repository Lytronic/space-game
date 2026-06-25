using Godot;
using System;
using SpaceGame.util;


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

		// Register keybinds from Settings in Godot's keybind system
		foreach (var entry in SettingsModel.Instance.Settings)
		{
			if (entry.Value is SettingsEntry.Keybind keybind)
			{
				// settings keys follow the pattern "<category>.<action_name>", e. g. "controls.forward"
				StringName name = entry.Key.Split(".")[1];
				GD.Print(name);
				InputMap.AddAction(name);

				var inputEvent = new InputEventKey
				{
					Keycode = OS.FindKeycodeFromString(keybind.Value)
				};

				InputMap.ActionAddEvent(name, inputEvent);
			}
		}
	}	
}
