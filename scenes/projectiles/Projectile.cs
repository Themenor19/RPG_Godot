using System;
using System.Threading.Tasks;
using Godot;

public partial class Projectile : Area2D
{
	public bool CanInteract = true;
	[Export] public Node Parent;
	[Export] public int Damage;
	public Func<Area2D, Task> Interact = area => Task.CompletedTask;
	
	[Signal] public delegate void ProjectileBodyShapeEnteredEventHandler (Rid bodyRid, Node2D body, int bodyShapeIndex, int localShapeIndex);
	[Signal] public delegate void ProjectileBodyEnteredEventHandler (Node2D body);

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
	
	public void _on_body_shape_entered(Rid bodyRid, Node2D body, int bodyShapeIndex, int localShapeIndex)
	{
		if (CanInteract)
		{
			EmitSignal(SignalName.ProjectileBodyShapeEntered, bodyRid, body, bodyShapeIndex, localShapeIndex);
		}
	}
	
	public void _on_body_entered(Node2D body)
	{
		if (CanInteract)
		{
			EmitSignal(SignalName.ProjectileBodyEntered, body);
		}
	}
}
