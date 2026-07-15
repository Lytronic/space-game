using Godot;
using System;

namespace Microgravity.util;
public partial class SaveMenu : HBoxContainer
{
	private LineEdit _saveFileName;
	private SoundManager _soundManager;
	private string _CurrentDateandTime;
	public override void _Ready()
	{
		_soundManager = GetNode<SoundManager>("/root/SoundManager");
		GetNode<TextureButton>("VBoxContainerLeft/BackButton").Pressed += () => {
			_soundManager.PlaySound(0,0);
			GetTree().ChangeSceneToFile("res://scenes/ui/GameMenu.tscn");
		};
		GetNode<TextureButton>("VBoxContainerRight/SaveListBg/NewGameButton").Pressed += () => {
				_soundManager.PlaySound(0,0);
				var vars = new Stats
				{
					// change something so we can identify this object when we load a game with it
					Score = 0,
					CurrentHealth = PlayerVariables.Stats.MaxHealth,
					CurrentShield = PlayerVariables.Stats.MaxShield,
					Energy = PlayerVariables.Stats.MaxEnergy,
					Fuel = PlayerVariables.Stats.MaxFuel,
					Round = 1,
				};

				var data = new SaveData
				{
					Stats = vars,
					ActiveParts = [ new SavedPart("WeaponPartArc", 1.0f) ]
				};

				DB.SaveGame(_saveFileName.Text, data);
				GetTree().ChangeSceneToFile("res://scenes/main/game.tscn");
			};
			

		_saveFileName = GetNode<LineEdit>("VBoxContainerRight/SaveListBg/SaveFileName");
		_CurrentDateandTime = Time.GetDatetimeStringFromSystem();
		_saveFileName.Text = "SAVE-" + _CurrentDateandTime;
	}

	
	public override void _Process(double delta)
	{
	}
}
