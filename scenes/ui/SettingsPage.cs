using Godot;
using System;
using System.Collections.Generic;
using SpaceGame.util;
using System.Globalization;

public partial class SettingsPage : ScrollContainer
{
	public string Category;
	public Dictionary<string, SettingsEntry> Settings;

	public override void _Ready()
	{
		var settingsList = GetNode<VBoxContainer>("./SettingsList");

		foreach (var entry in Settings)
		{
			if (entry.Key.Split(".")[0] == Category)
			{
				switch (entry.Value)
				{
					case SettingsEntry.Keybind kb:
						Keybind k = (Keybind)ResourceLoader.Load<PackedScene>("res://scenes/ui/Keybind.tscn").Instantiate();
						k.KVP = new KeyValuePair<string, SettingsEntry.Keybind>(entry.Key, kb);
						settingsList.AddChild(k);
						break;

					case SettingsEntry.Bool b:
						CheckBox checkBox = new() { ButtonPressed = b.Value };
						checkBox.Toggled += (toggledOn) =>
						{
							DB.UpdateSettingsEntry(entry.Key, b with { Value = toggledOn });
						};

						HBoxContainer hBoxB = new();
						hBoxB.AddChild(new Control() { SizeFlagsHorizontal = SizeFlags.ExpandFill });
						hBoxB.AddChild(new Label() { Text = b.Description, SizeFlagsHorizontal = SizeFlags.ExpandFill });
						hBoxB.AddChild(new Control() { SizeFlagsHorizontal = SizeFlags.ExpandFill });
						hBoxB.AddChild(checkBox);
						hBoxB.AddChild(new Control() { SizeFlagsHorizontal = SizeFlags.ExpandFill });
						settingsList.AddChild(hBoxB);
						break;

					case SettingsEntry.Float f:
						HSlider slider = new()
						{
							Value = f.Value,
							MinValue = f.Min,
							MaxValue = f.Max,
							SizeFlagsHorizontal = SizeFlags.ExpandFill,
							SizeFlagsVertical = SizeFlags.Fill
						};

						Label valueLabel = new() { Text = f.Value.ToString(CultureInfo.InvariantCulture) };
						slider.ValueChanged += (value) =>
						{
							// Updating on every change is potentially expensive when the slider is dragged too often.
							// I didn't notice any issues, however, so it's been kept simple for now.
							// Otherwise there should be a debounce timer of some sort...
							DB.UpdateSettingsEntry(entry.Key, f with { Value = (float)value });
							valueLabel.Text = value.ToString(CultureInfo.InvariantCulture);
						};

						HBoxContainer hBoxF = new();
						hBoxF.AddChild(new Control() { SizeFlagsHorizontal = SizeFlags.ExpandFill });
						hBoxF.AddChild(new Label() { Text = f.Description, SizeFlagsHorizontal = SizeFlags.ExpandFill });
						hBoxF.AddChild(valueLabel);
						hBoxF.AddChild(slider);
						hBoxF.AddChild(new Control() { SizeFlagsHorizontal = SizeFlags.ExpandFill });
						settingsList.AddChild(hBoxF);
						break;

					default:
						break;
				}
			}
		}
	}
}
