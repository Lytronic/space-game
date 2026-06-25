using Godot;
using System;
using System.Collections.Generic;
using SpaceGame.util;

public partial class SettingsModel : Node
{
	public static SettingsModel Instance { get; private set; }

	public Dictionary<string, SettingsEntry> Settings { get; private set; }

	public override void _Ready()
	{
		Instance = this;
	}

	public void Init()
	{
		Settings = DB.GetSettings();
	}

	/// <summary>
	/// Update an entry both in memory and on disk in the database
	/// </summary>
	public void SetEntry(string key, SettingsEntry value)
	{
		Settings[key] = value;
		DB.UpdateSettingsEntry(key, value);
	}
}
