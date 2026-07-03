using Godot;
using Microgravity.util;

public partial class HighScoresScreen : Control
{
	[Signal]
	public delegate void CloseEventHandler();

	private Node _soundManager;
	
	public override void _Ready()
	{
		_soundManager = GetNode("/root/SoundManager");

		GetNode<TextureButton>("./VBoxContainerLeft/BackButton").Pressed += () => {
			_soundManager.Call("PlaySound", 0, 0);
			CloseScreen();
		};
		
		var resultList = GetNode<VBoxContainer>("VBoxContainerRight/TextureRect/ItemList");
		
		var results = DB.GetHighScores();

		int position = 1;
		
		foreach (var result in results)
		{
			// populate an HBox with data, then add it to the list
			var hBox = new HBoxContainer();
			hBox.AddChild(new Control() { SizeFlagsHorizontal = SizeFlags.ExpandFill }); // left spacer
			hBox.AddChild(new Label() {
				Text = $"#{position}",
				HorizontalAlignment = HorizontalAlignment.Left,
				SizeFlagsHorizontal = SizeFlags.ExpandFill
			});
			hBox.AddChild(new Label() {
				Text = result.PlayerName,
				HorizontalAlignment = HorizontalAlignment.Center,
				SizeFlagsHorizontal = SizeFlags.ExpandFill
			});
			hBox.AddChild(new Label() {
				Text = result.Score.ToString(),
				HorizontalAlignment = HorizontalAlignment.Right,
				SizeFlagsHorizontal = SizeFlags.ExpandFill
			});
			hBox.AddChild(new Control() { SizeFlagsHorizontal = SizeFlags.ExpandFill }); // right spacer
			resultList.AddChild(hBox);
			
			position++;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey)
		{
			GetViewport().SetInputAsHandled();
		}
		
		if (@event.IsActionPressed("ui_close_dialog"))
		{
			CloseScreen();
		}
	}
	
	private void CloseScreen()
	{
		QueueFree();
		EmitSignal(SignalName.Close);
	}
}
