using Godot;
using System;
using SpaceGame.util;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

public partial class SettingsScreen : HBoxContainer
{
	public Dictionary<string, SettingsEntry> Settings;

	[Signal]
	public delegate void CloseEventHandler();

	public override void _Ready()
	{
		Settings = DB.GetSettings();

		GetNode<TextureButton>("./VBoxContainerLeft/BackButton").Pressed += CloseScreen;

		foreach (string category in new List<string> { "General", "Video", "Audio", "Controls" })
		{
			SettingsPage page = (SettingsPage)ResourceLoader.Load<PackedScene>("res://scenes/ui/SettingsPage.tscn").Instantiate();
			page.Name = category;
			page.Category = category.ToLower(); // used as part of the settings keys
			page.Settings = Settings;
			GetNode<TabContainer>("VBoxContainerRight/TabContainer").AddChild(page);
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_close_dialog"))
		{
			GetViewport().SetInputAsHandled();
			CloseScreen();
		}
	}

	private void CloseScreen()
	{
		QueueFree();
		EmitSignal(SignalName.Close);
	}
}
