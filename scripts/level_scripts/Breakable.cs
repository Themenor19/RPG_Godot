using System.Linq;
using Godot;
using RPG.custom_resources.inventory;
using RPG.scripts.globals;
using RPG.scripts.ui;

namespace RPG.scripts.level_scripts;

public partial class Breakable: Area2D
{
    [Export] public Inventory Drops;

    private GlobalHandler _global;
    private PackedScene _worldObject;

    public override void _Ready()
    {
        _global = GetTree().GetRoot().GetChildren().OfType<GlobalHandler>().FirstOrDefault();
        _worldObject = _global.WorldInventoryItemScene;
    }
    
    public void Break()
    {
        uint numItemsToSpawn = 1;
        int randomIndex = GD.RandRange(0, Drops.Items.Count - 1);
        var drop = Drops.Items[randomIndex];
        if (drop.Item.Name == "Gold Coin")
        {
            numItemsToSpawn = GD.Randi() % 4 + 1;
        }

        for (int i = 0; i < numItemsToSpawn; i++)
        {
            var tempObject = _worldObject.Instantiate<WorldInventoryItem>();
            tempObject.ItemResource = drop;
            GetTree().GetRoot().CallDeferred("add_child", tempObject);
            tempObject.Position = GlobalPosition + new Vector2(GD.RandRange(-5, 5)*2 , GD.RandRange(-5, 5)*2);
        }
    }
    
}