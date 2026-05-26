using Godot;
using System;
using System.Threading.Tasks;

public partial class Projectile : Area2D
{
	[Export] public Node Parent;
	public Func<Area2D, Task> Interact = async (Area2D area) =>
	{
		
	};
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_area_entered(Area2D area)
	{
		Interact(area);
	}
}
