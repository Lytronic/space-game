using System.Linq;
using Godot;

namespace Microgravity.util;
public partial class LoadMenu : HBoxContainer
{
	private SoundManager _soundManager;

	private ItemList _savesList;

	private int[] _selectedItems;
	private int _selectedItem;
	public override void _Ready()
	{
		_savesList = GetNode<ItemList>("VBoxContainerRight/SaveListBg/SavesList");
		_soundManager = GetNode<SoundManager>("/root/SoundManager");
		GetNode<TextureButton>("VBoxContainerLeft/BackButton").Pressed += () => {
			_soundManager.PlaySound(0,0);
			GetTree().ChangeSceneToFile("res://scenes/ui/GameMenu.tscn");
		};
		GetNode<TextureButton>("VBoxContainerRight/SaveListBg/LoadButton").Pressed += () => {
				var saves = DB.GetSaves();
				PlayerVariables.LoadFromSave(saves.Keys.Max() - _selectedItem);
				_soundManager.Fight();
				GetTree().ChangeSceneToFile("res://scenes/main/game.tscn");
		};
		GetNode<TextureButton>("VBoxContainerRight/SaveListBg/DeleteButton").Pressed += () =>
		{
			if (_selectedItems.Length > 0)
   			{
				var saves = DB.GetSaves();
		
				int saveIndex = _selectedItems[0];
				if (saveIndex < saves.Count)
				{
					int saveId = saves.Keys.ElementAt(saveIndex);
					DB.DeleteGame(saveId);
					UpdateSavesList();
					_soundManager.PlaySound(0,0);
				}
			}

		};
		_savesList.ItemSelected += OnItemSelected;
		_savesList.SelectMode = ItemList.SelectModeEnum.Single;
		UpdateSavesList();
	}


	public override void _Process(double delta)
	{
		
	}

	private void OnItemSelected(long index)
	{
		_selectedItems = _savesList.GetSelectedItems();
		
		if (_selectedItems.Length > 0)
		{
			_selectedItem = _selectedItems[0];
			GD.Print($"Correct Index: {_selectedItem}");
		}
	}

	private void UpdateSavesList()
		{
			var savesList = GetNode<ItemList>("VBoxContainerRight/SaveListBg/SavesList");

			/*int count = savesList.ItemCount;
			for (int i = count - 1; i >= 0; i--)
			{
				savesList.RemoveItem(i);
			}*/
			savesList.Clear();

			var saves = DB.GetSaves();
			
			foreach (var save in saves)
			{
				int itemId = save.Key; 
				string itemText = $"{itemId}        {save.Value}";

				int index = savesList.AddItem(itemText);
				savesList.SetItemMetadata(index, itemId);				
				//savesList.AddItem($"{save.Key}        {save.Value}");
			}
			//_selectedItems = _savesList.GetSelectedItems();
		}
}
