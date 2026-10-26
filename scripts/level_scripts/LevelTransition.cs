using System.Linq;
using Godot;
using RPG.scripts.globals;

namespace RPG.scripts.level_scripts;

[Tool]
public partial class LevelTransition : Area2D
{
	[Export] public string LevelReference;
	[Export] public string SpawnerName;

	private CollisionShape2D _body;
	private bool _canCollide = true;
	
	private GlobalHandler _global;

	public override void _Ready()
	{
		_global = GetTree().GetRoot().GetChildren().OfType<GlobalHandler>().FirstOrDefault();
		_body = GetNode<CollisionShape2D>("CollisionShape2D");
	}
	
	private void _on_body_entered(Node2D node)
	{
		if (node is Player && _canCollide)
		{ 
			_canCollide = false;
			_body.SetDeferred("disabled", true);
			_global.PlayerMoveScenes(LevelReference, SpawnerName);
		}
	}
}
