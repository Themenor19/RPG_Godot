using Godot;

namespace RPG.scripts.spawners;

public partial class PlayerSpawner : Node2D
{
    [Export] public Sprite2D SpawnerSprite;
    [Export] public Level ParentLevel;

    public Player Spawn(Player player)
    {
        var parent = player.GetParent();
        if (parent == null)
        {
            ParentLevel.AddChild(player);
            // If it's a completely fresh spawn, setting it immediately is safe
            player.GlobalPosition = SpawnerSprite.GlobalPosition;
        }
        else
        {
            // 1. Initiate the reparent process
            player.Reparent(ParentLevel);
          
            // 2. Capture the target position
            Vector2 targetPos = SpawnerSprite.GlobalPosition;
          
            // 3. Defer setting the position until the tree update settles
            Callable.From(() => player.GlobalPosition = targetPos).CallDeferred();
        }
       
        return player;
    }
}