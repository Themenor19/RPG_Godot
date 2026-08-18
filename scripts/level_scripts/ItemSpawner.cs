using System.Collections.Generic;
using System.Linq;
using Godot;
using RPG.custom_resources.inventory;
using RPG.scripts.globals;
using RPG.scripts.ui;

public partial class ItemSpawner : Node2D
{
	//Layer that contains a tilemap of tiles that have custom data called "can_spawn" which are just booleans of if an object can spawn there or not
	[Export] private TileMapLayer _spawningLocationLayer;
	//Inventory of possible drops
	[Export] private Inventory _drops;
	private GlobalHandler _global;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_global = GetTree().GetRoot().GetChildren().OfType<GlobalHandler>().FirstOrDefault();
		var spawnPositions = GetSpawnPositions();
		var numItemsToSpawn = (int)(GD.Randi() % spawnPositions.Count);

		for (int i = 0; i < numItemsToSpawn; i++)
		{
			if (spawnPositions.Count == 0) break;

			var positionIndex = (int)(GD.Randi() % spawnPositions.Count);
			var position = spawnPositions[positionIndex];

			var worldObject = _global.WorldInventoryItemScene.Instantiate<WorldInventoryItem>();
			var randIndex = (int)(GD.Randi() % _drops.Items.Count);
			var slotToSpawn = _drops.Items[randIndex];
			if (slotToSpawn != null)
			{
				worldObject.ItemResource = (InventoryItemSlot)slotToSpawn.Duplicate();
			}
			AddChild(worldObject);
			worldObject.GlobalPosition = position;
			spawnPositions.RemoveAt(positionIndex);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public List<Vector2> GetSpawnPositions()
	{
		var spawnPositions = new List<Vector2>();

		foreach (var cell in _spawningLocationLayer.GetUsedCells())
		{
			var tileData = _spawningLocationLayer.GetCellTileData(cell);
			if (tileData != null && tileData.GetCustomData("can_spawn").AsBool())
			{
				Vector2 globalPos = _spawningLocationLayer.ToGlobal(_spawningLocationLayer.MapToLocal(cell));
				spawnPositions.Add(globalPos);
			}
		}
		return spawnPositions;
	}
}
