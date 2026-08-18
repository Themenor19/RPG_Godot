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
			var plantPosition = new Vector2I(4, 3 + i);
			var skullFlowerObject = skullFlower.Instantiate<BaseFlower>();
			skullFlowerObject.GlobalPosition = PlantLayer.MapToLocal(plantPosition);
			PlantedSlots.Add(plantPosition);
			skullFlowerObject.Init(this, plantPosition);
			PlantLayer.AddChild(skullFlowerObject);
		}

		for (int i = 0; i < 2; i++)
		{
			var plantPosition = new Vector2I(4, 5 + i);
			var fireFlowerObject =  fireFlower.Instantiate<BaseFlower>();
			fireFlowerObject.GlobalPosition = PlantLayer.MapToLocal(plantPosition);
			PlantedSlots.Add(plantPosition);
			fireFlowerObject.Init(this, plantPosition);
			PlantLayer.AddChild(fireFlowerObject);
		}
		skullFlower.Dispose();
		fireFlower.Dispose();
	}
}
