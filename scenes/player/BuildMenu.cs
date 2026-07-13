using Godot;

public partial class BuildMenu : Control
{
	PlayerVariables vars = PlayerVariables.Instance;

	public static readonly float GridSpacing = 90.0f;

	private PackedScene _menuShipPartScene = GD.Load<PackedScene>("res://scenes/ship parts/MenuShipPart.tscn");
	private GridContainer _grid;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_grid = GetNode<GridContainer>("Grid");
		
		_grid.Columns = PlayerVariables.Stats.InvWidth;
		for (int i = 0; i < PlayerVariables.Stats.InvHeight * PlayerVariables.Stats.InvWidth; i++)
		{
			_grid.AddChild(ResourceLoader.Load<PackedScene>("res://scenes/ui/ItemSlot.tscn").Instantiate());
		}
	}
	
	public void OpenMainScene()
	{
		GetTree().ChangeSceneToFile("res://scenes/main/game.tscn");
	}

	public struct GridSpace
	{
		public GridSpace()
		{
			PartInstance = null;
			UID = 0;
			IsActive = false;
		}

		public ShipPart PartInstance { get; private set; }
		public int UID { get; private set; }
		public bool IsActive { get; private set; }

		public void Assign(ShipPart part, int uid)
		{
			PartInstance = part;
			UID = uid;
			IsActive = true;
		}
		public void Clear()
		{
			PartInstance = null;
			UID = 0;
			IsActive = false;
		}
	}
}
