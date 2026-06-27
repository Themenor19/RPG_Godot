using Godot;

public partial class DestroysProjectiles : Area2D
{
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
		var groups = area.GetGroups();
		if (groups.Contains("projectiles"))
		{
			var projectile = area as Projectile;
			projectile?.Parent?.QueueFree();
		}
	}
}
