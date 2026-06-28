using Godot;

namespace RPG.scripts.level_scripts;

public partial class TestLevel1 : Level
{
	public override void _Ready()
	{
		base._Ready();
		var skullFlower = GD.Load<PackedScene>("res://scenes/plants/skull_flower.tscn");
		var fireFlower = GD.Load<PackedScene>("res://scenes/plants/fire_flower.tscn");
		
		for (int i = 0; i < 2; i++)
		{
			var skullFlowerObject = skullFlower.Instantiate<Node2D>();
			skullFlowerObject.GlobalPosition = PlantLayer.MapToLocal(new Vector2I(4, 3+i));
			PlantLayer.AddChild(skullFlowerObject);
		}

		for (int i = 0; i < 2; i++)
		{
			var fireFlowerObject =  fireFlower.Instantiate<Node2D>();
			fireFlowerObject.GlobalPosition = PlantLayer.MapToLocal(new Vector2I(4, 5+i));
			PlantLayer.AddChild(fireFlowerObject);
		}
	}
}
