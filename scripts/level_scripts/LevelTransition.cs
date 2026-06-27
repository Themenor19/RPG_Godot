using Godot;
using RPG.scripts.globals;

namespace RPG.scripts.level_scripts;

public partial class LevelTransition : Area2D
{
	[Export] public string LevelReference;
	[Export] public string SpawnerName;

	private bool _canCollide = true;
	
	private Global _global;

	public override void _Ready()
	{
		_global = Global.Instance;
	}
	
	private void _on_body_entered(Node2D node)
	{
		if (node is Player && _canCollide)
		{
			_canCollide = false;
			_global.PlayerMoveScenes(LevelReference, SpawnerName);
		}
	}
}
