using Godot;
using System;
using Microgravity.util;
using System.Collections.Generic;

public partial class Keybind : HBoxContainer
{
	public KeyValuePair<string, SettingsEntry.Keybind> KVP;
	private bool _listening = false;

	public override void _Ready()
	{
		GetNode<Label>("./Description").Text = KVP.Value.Description;
		var button = GetNode<Button>("./ValueButton");
		button.Text = KVP.Value.ToString();
		button.Pressed += () => {
			button.Text = "...";
			_listening = true;
		};

		GetNode<Button>("./ResetButton").Pressed += () => {
			SettingsController.Instance.SetKeybind(KVP.Key, OS.FindKeycodeFromString(SettingsEntry.DefaultSettings[KVP.Key].ToString()));
			button.Text =  SettingsEntry.DefaultSettings[KVP.Key].ToString();
		};
	}

	public override void _Input(InputEvent @event)
	{
		if (_listening && @event is InputEventKey eventKey)
		{
			var asText = OS.GetKeycodeString(eventKey.Keycode);
			SettingsController.Instance.SetKeybind(KVP.Key, eventKey.Keycode);
			GetNode<Button>("./ValueButton").Text = asText;
			_listening = false;
			GetViewport().SetInputAsHandled();
		}
	}
}
