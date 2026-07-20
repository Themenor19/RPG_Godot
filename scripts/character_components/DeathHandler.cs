using Godot;

namespace RPG.scripts.character_components;

public partial class DeathHandler : Node2D
{
	[Export] public HealthBar HealthBar;
	[Export] public Node Parent;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		HealthBar.Dead += Die;
	}

	public void Die()
	{
		CallDeferred(nameof(KillParent));
	}

	public void KillParent()
	{
		if (Parent != null)
		{
			Parent.QueueFree();
		}
	}
}
