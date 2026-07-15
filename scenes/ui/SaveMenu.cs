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

				long id = DB.CreateSave(_saveFileName.Text, new SaveData());
				if (id > 0)
				{
					PlayerVariables.Instance.CurrentSaveId = id;
					GetTree().ChangeSceneToFile("res://scenes/main/game.tscn");
				}
				else
				{
					GD.PrintErr("Couldn't create save!");
				}
			};
			

		_saveFileName = GetNode<LineEdit>("VBoxContainerRight/SaveListBg/SaveFileName");
		_CurrentDateandTime = Time.GetDatetimeStringFromSystem();
		_saveFileName.Text = "SAVE-" + _CurrentDateandTime;
	}

	
	public override void _Process(double delta)
	{
	}
}
