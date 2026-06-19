using Godot;
using System;
using SpaceGame.util;
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
			DB.UpdateSettingsEntry(KVP.Key, SettingsEntry.DefaultSettings[KVP.Key]);
			button.Text =  SettingsEntry.DefaultSettings[KVP.Key].ToString();
		};
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (_listening && @event is InputEventKey)
		{
			var asText = @event.AsText();
			DB.UpdateSettingsEntry(KVP.Key, KVP.Value with { Value = asText });
			GetNode<Button>("./ValueButton").Text = asText;
			_listening = false;
		}
	}
}
