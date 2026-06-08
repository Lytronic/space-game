using Godot;
using System;

public partial class DeathScreen : Control
{
    private void _on_retry_button_pressed()
    {
        GetTree().ReloadCurrentScene();
    }
}
