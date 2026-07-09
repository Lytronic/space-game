using System.Linq;
using Godot;

namespace Microgravity.util;
public partial class LoadMenu : HBoxContainer
{
	private Node _soundManager;
	public override void _Ready()
	{
		_soundManager = GetNode("/root/SoundManager");
		GetNode<TextureButton>("VBoxContainerLeft/BackButton").Pressed += () => {
			_soundManager.Call("PlaySound", 0, 0);
			GetTree().ChangeSceneToFile("res://scenes/ui/GameMenu.tscn");
		};
		GetNode<TextureButton>("VBoxContainerRight/SaveListBg/LoadButton").Pressed += () => {
				var saves = DB.GetSaves();
				PlayerVariables.LoadFromSave(saves.Keys.Max());

				GetTree().ChangeSceneToFile("res://scenes/main/game.tscn");
			};
		UpdateSavesList();
	}


	public override void _Process(double delta)
	{
	}

	private void UpdateSavesList()
		{
			var savesList = GetNode<ItemList>("VBoxContainerRight/SaveListBg/ItemList");

			int count = savesList.ItemCount;
			for (int i = count - 1; i >= 0; i--)
			{
				savesList.RemoveItem(i);
			}
			
			foreach (var save in DB.GetSaves())
			{
				savesList.AddItem($"{save.Key}        {save.Value}");
			}
		}
}
