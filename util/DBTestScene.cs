using Godot;

namespace SpaceGame.util
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
		}

		private void Add()
		{
			var name = GetNode<LineEdit>("DBTestContainer/ContainerLeft/NameLine").GetText();
			var score = int.Parse(GetNode<LineEdit>("DBTestContainer/ContainerLeft/ScoreLine").GetText());
			DB.AddHighScore(name, score);
		}

		private void QueryAll()
		{
			UpdateList(DB.GetHighScores());
		}

		private void QueryPlayer()
		{
			string name = GetNode<LineEdit>("DBTestContainer/ContainerRight/QueryContainer/NameLine").GetText();
			if (name == null)
			{
				UpdateList([]);
				return;
			}
			UpdateList(DB.GetHighScores(name));
		}

		private void UpdateList(System.Collections.Generic.List<HighScore> results)
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
	}
}
