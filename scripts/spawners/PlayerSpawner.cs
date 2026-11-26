using Godot;

namespace RPG.scripts.spawners;

public partial class PlayerSpawner : Node2D
{
	[Export] public Sprite2D SpawnerSprite;
	[Export] public Level ParentLevel;

	public MagicFarmer.scripts.Player Spawn(MagicFarmer.scripts.Player player)
	{
		var parent = player.GetParent();
		if (parent == null)
		{
			ParentLevel.AddChild(player);
			player.GlobalPosition = SpawnerSprite.GlobalPosition;
		}
		else
		{
			player.Reparent(ParentLevel);
		  
			Vector2 targetPos = SpawnerSprite.GlobalPosition;
		  
			Callable.From(() => player.GlobalPosition = targetPos).CallDeferred();
		}
	   
		return player;
	}
}
