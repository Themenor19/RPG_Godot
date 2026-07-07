using System.Collections.Generic;
using System.Linq;
using Godot;
using RPG.scripts;
using RPG.scripts.spawners;
using Global = RPG.scripts.globals.Global;

public partial class Level : Node2D
{
	[Export]
	public TileMapLayer PlantLayer { get; set; }
	[Export]
	public TileMapLayer GroundLayer { get; set; }
	[Export]
	public PlayerSpawner[] Spawners { get; set; }
	[Export] public bool CanPlant { get; set; }
	private readonly List<Vector2I> _scales = [new(1280, 720),  new(1920, 1080), new(640, 360)]; 
	private readonly List<float> _scaleFactors = [1f, 2f, 3f, 4f];

	private Player _player;
	private Global _global;

	private PackedScene _plotSelector;
	private Node2D _plotSelectorNode;
	private bool _isPlanting;
	private Vector2I _plotSelectorCoords;
	public List<Vector2I> PlantedSlots = new();
	
	
	private int _currentIndex;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (!CheckSpawners())
		{
			GD.PrintErr("Level: CheckSpawners failed");
		}
		_global = Global.Instance;

		_plotSelector = GD.Load<PackedScene>("res://scenes/plants/plot_selector.tscn");

		_global.CurrentLevel = this;
	}

	public bool AddPlayer(Player player, string spawnerName)
	{
		
		var spawner = Spawners.FirstOrDefault(s => s.Name== spawnerName);
		if (spawner == null)
		{
			player.Reparent(this);
			player.GlobalPosition = new Vector2(200, 300);
			_player = player;
		}
		else
		{
			_player = spawner.Spawn(player);
			if (_player == null) return false;
			if (Global.Instance.SaveLoaded)
			{
				_player.Position = Global.Instance.SavedPlayerPosition;
			}

		}

		if (_player == null) return false;
		_player.IsPlanting += PlayerPlanting;
		return true;
	}

	private void PlayerPlanting(bool isPlanting)
	{
		_isPlanting = isPlanting;
		if (_isPlanting)
		{
			_plotSelectorNode = _plotSelector.Instantiate<Node2D>();
			_plotSelectorNode.GlobalPosition = GetGlobalMousePosition();
			CallDeferred(Node.MethodName.AddChild, _plotSelectorNode);
		}
		else
		{
			if (_plotSelectorNode != null)
			{
				CallDeferred(Node.MethodName.RemoveChild, _plotSelectorNode);
				_plotSelectorNode.CallDeferred(Node.MethodName.QueueFree);
				_plotSelectorNode = null;
			}
		}
		
	}

	public bool Plant(InventoryItem item)
	{
		if (!CanPlant)  
		{
			GD.PrintErr("Level: Can't plant item");
			return false;
		}
		if (item == null)
		{
			GD.PrintErr("Level: inventory item is null");
			return false;
		}
		if (item.ItemScene == null)
		{
			GD.PrintErr("Level: Item scene is null");
			return false;
		}

		var tileCoords = _plotSelectorCoords;
		PlantedSlots.Add(tileCoords);
		var index = PlantedSlots.IndexOf(tileCoords);

		if (GroundLayer.GetUsedCells().Contains(tileCoords))
		{
			return false;
		}

		if (PlantedSlots.Contains(tileCoords))
		{
			return false;
		}
		
		var skullFlowerObject = item.ItemScene.Instantiate<BaseFlower>();
		skullFlowerObject.Init(this, index);
		skullFlowerObject.GlobalPosition = PlantLayer.MapToLocal(tileCoords);
		PlantLayer.AddChild(skullFlowerObject);
		return true;
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_isPlanting && _plotSelectorNode != null && PlantLayer != null)
		{
			Vector2 mouseLocal = GroundLayer.ToLocal(GetGlobalMousePosition());
			_plotSelectorCoords = GroundLayer.LocalToMap(mouseLocal);
			Vector2 snappedWorld = GroundLayer.ToGlobal(PlantLayer.MapToLocal(_plotSelectorCoords));
			_plotSelectorNode.GlobalPosition = snappedWorld;
		}
	}

	private bool CheckSpawners()
	{
		foreach (var item in Spawners)
		{
			if (item is null)
			{
				return false;
			}
		}

		return true;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_player.IsPlanting -= PlayerPlanting;
	}
}
