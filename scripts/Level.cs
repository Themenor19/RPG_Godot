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
	private readonly List<Vector2I> _scales = [new(1280, 720),  new(1920, 1080), new(640, 360)]; 
	private readonly List<float> _scaleFactors = [1f, 2f, 3f, 4f];

	private Player _player;
	private Global _global;

	private PackedScene _plotSelector;
	private Node2D _plotSelectorNode;
	private bool _isPlanting;
	
	
	private int _currentIndex;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (!CheckSpawners())
		{
			GD.PrintErr("Level: CheckSpawners failed");
		}
		_global = Global.Instance;
		
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

			_player.IsPlanting += PlayerPlant;
		}

		if (_player == null) return false;
		_player.IsPlanting -= PlayerPlant;
		return true;
	}

	private void PlayerPlant()
	{
		if (!_isPlanting)
		{
			_plotSelectorNode = _plotSelector.Instantiate<Node2D>();
			AddChild(_plotSelectorNode);
			_isPlanting = true;
		}
		else
		{
			RemoveChild(_plotSelectorNode);
			_plotSelectorNode.QueueFree();
			_plotSelectorNode = null;
			_isPlanting = false;
		}
		
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_isPlanting && _plotSelectorNode != null && PlantLayer != null)
		{
			Vector2 mouseLocal = GroundLayer.ToLocal(GetGlobalMousePosition());
			Vector2I tileCoords = GroundLayer.LocalToMap(mouseLocal);
			Vector2 snappedWorld = GroundLayer.ToGlobal(PlantLayer.MapToLocal(tileCoords));
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
}
