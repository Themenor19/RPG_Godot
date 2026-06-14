using Godot;
using System;
using System.Threading.Tasks;

public partial class Projectile : Area2D
{
	public bool CanInteract = true;
	[Export] public Node Parent;
	[Export] public int Damage;
	public Func<Area2D, Task> Interact = (Area2D area) => Task.CompletedTask;
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
		if (CanInteract)
		{
			Interact(area);
		}
	}
}
