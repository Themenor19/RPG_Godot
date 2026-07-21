using Godot;
using RPG.scripts.globals;

namespace RPG.scripts.character_components;

public partial class DeathHandler : Node2D
{
	[Export] public HealthBar HealthBar;
	[Export] public Node2D Parent;
	
	private Global _global;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		HealthBar.Dead += Die;
		_global = Global.Instance;
	}

	public void Die()
	{
		Parent.Visible = false;
		_global.PlayerMoveScenes("uid://bibtx3p5das13");;
	}
}
