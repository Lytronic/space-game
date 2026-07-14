using System.Linq;
using Godot;

namespace Microgravity.util
{
	public partial class DBTestScene : Control
	{
		public override void _Ready()
		{
			var addButton = GetNode<Button>("DBTestContainer/ContainerLeft/AddButton");
			addButton.Pressed += Add;
			var queryAllButton = GetNode<Button>("DBTestContainer/ContainerRight/QueryContainer/QueryAllButton");
			queryAllButton.Pressed += QueryAll;
			var queryPlayerButton = GetNode<Button>("DBTestContainer/ContainerRight/QueryContainer/QueryPlayerButton");
			queryPlayerButton.Pressed += QueryPlayer;

			GetNode<Button>("DBTestContainer/SaveTestContainer/SaveButton").Pressed += () => {
				var vars = new Stats
				{
					// change something so we can identify this object when we load a game with it
					Score = 1290,
					CurrentHealth = 67.0f,
					Round = 123,
				};

				var data = new SaveData
				{
					Stats = vars,
					ActiveParts = [ new SavedPart("WeaponPartArc", 1.0f) ]
				};

				DB.SaveGame("test name", data);

				UpdateSavesList();
			};
			
			GetNode<Button>("DBTestContainer/SaveTestContainer/LoadButton").Pressed += () => {
				var saves = DB.GetSaves();
				PlayerVariables.LoadFromSave(saves.Keys.Max());

				GetTree().ChangeSceneToFile("res://scenes/main/game.tscn");
			};

			UpdateSavesList();
		}

		private void Add()
		{
			var name = GetNode<LineEdit>("DBTestContainer/ContainerLeft/NameLine").GetText();
			var score = int.Parse(GetNode<LineEdit>("DBTestContainer/ContainerLeft/ScoreLine").GetText());
			DB.AddHighScore(name, score);
		}

		private void QueryAll()
		{
			UpdateHighScoresList(DB.GetHighScores());
		}

		private void QueryPlayer()
		{
			string name = GetNode<LineEdit>("DBTestContainer/ContainerRight/QueryContainer/NameLine").GetText();
			if (name == null)
			{
				UpdateHighScoresList([]);
				return;
			}

			int limit = (int) GetNode<SpinBox>("DBTestContainer/ContainerRight/QueryContainer/LimitBox").Value;
			UpdateHighScoresList(DB.GetHighScores(name, limit));
		}

		private void UpdateHighScoresList(System.Collections.Generic.List<HighScore> results)
		{
			var resultList = GetNode<ItemList>("DBTestContainer/ContainerRight/ItemList");

			// remove existing Items
			int count = resultList.ItemCount;
			for (int i = count - 1; i >= 0; i--)
			{
				resultList.RemoveItem(i);
			}

			foreach (var result in results)
			{
				resultList.AddItem($"{result.Id}        {result.PlayerName}        {result.Score}");
			}
		}

		private void UpdateSavesList()
		{
			var savesList = GetNode<ItemList>("DBTestContainer/SaveTestContainer/ItemList");

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
}
