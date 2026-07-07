using System.Collections.Generic;
using System.Globalization;

namespace Microgravity.util
{
	/// <summary>
	/// Wrapper for storing settings entries of varying types along with their descriptions in the same Dictionary.
	/// </summary>
	public abstract record SettingsEntry
	{
		public sealed record Keybind(string Value, string Description) : SettingsEntry
		{
			public override string ToString() => Value;
		}

		public sealed record Float(float Value, float Min, float Max, string Description) : SettingsEntry
		{
			public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
		}

		public sealed record Bool(bool Value, string Description) : SettingsEntry
		{
			public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
		}

		/// <value>
		/// This is the exhaustive list of all settings options with their names and defaults.
		/// Add new entries here!
		/// </value>
		public static readonly Dictionary<string, SettingsEntry> DefaultSettings = new()
		{
			["video.damage_overlay_intensity"] = new Float(40.0f, 0.0f, 100.0f, "Damage overlay intensity (%)"),
			["controls.forward"] = new Keybind("W", "Move forward"),
			["controls.backward"] = new Keybind("S", "Move backward"),
			["controls.left"] = new Keybind("A", "Strafe left"),
			["controls.right"] = new Keybind("D", "Strafe right"),
			["controls.test_value"] = new Bool(false, "Test Value"),
			["controls.test_slider"] = new Float(1.0f, -5.0f, 10.0f, "Example Slider")
		};
	}
}
