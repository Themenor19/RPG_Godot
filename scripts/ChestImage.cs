using Godot;

public partial class ChestImage : AnimatedSprite2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_chest_opened()
	{
		GD.Print("animation open");
		Play("open");
	}

	private void _on_chest_closed()
	{
		GD.Print("animation close");
		Play("close");
	}
}
