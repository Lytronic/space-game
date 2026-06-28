using Godot;
using System;
using Microgravity.util;
using System.Collections.Generic;

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
		if (@event is InputEventKey)
		{
			GetViewport().SetInputAsHandled();
		}

		if (@event.IsActionPressed("ui_close_dialog"))
		{
			CloseScreen();
		}
	}

	private void CloseScreen()
	{
		QueueFree();
		EmitSignal(SignalName.Close);
	}
}
