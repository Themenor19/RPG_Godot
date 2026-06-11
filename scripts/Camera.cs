using Godot;

namespace RPG.scripts;

public partial class Camera : Camera2D
{
	/*private Node2D _target;

	public override void _Ready()
	{
		_target = GetParent<Node2D>();
		// Detach from parent's transform so we control it manually
		TopLevel = true;
	}

	public override void _Process(double delta)
	{
		GlobalPosition = _target.GlobalPosition.Round();
	}*/
}
