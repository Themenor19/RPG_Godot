using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using RPG.scripts;

public partial class Level : Node2D
{
	[Export]
	public TileMapLayer TileMapLayer { get; set; }
	private readonly List<Vector2I> _scales = [new(1280, 720),  new(1920, 1080), new(640, 360)]; 
	private readonly List<float> _scaleFactors = [1f, 2f, 3f, 4f];

	private CharacterBody2D _player;
	private Global _global;

	
	
	private int _currentIndex;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_global = Global.Instance;
		_player = GetNode<CharacterBody2D>("Player");
		if (Global.Instance.SaveLoaded)
		{
			_player.Position = Global.Instance.SavedPlayerPosition;
		}
		
		var skullFlower = GD.Load<PackedScene>("res://scenes/plants/skull_flower.tscn");
		var fireFlower = GD.Load<PackedScene>("res://scenes/plants/fire_flower.tscn");
		
		for (int i = 0; i < 2; i++)
		{
			var skullFlowerObject = skullFlower.Instantiate<Node2D>();
			skullFlowerObject.GlobalPosition = TileMapLayer.MapToLocal(new Vector2I(4, 3+i));
			TileMapLayer.AddChild(skullFlowerObject);
		}

		for (int i = 0; i < 2; i++)
		{
			var fireFlowerObject =  fireFlower.Instantiate<Node2D>();
			fireFlowerObject.GlobalPosition = TileMapLayer.MapToLocal(new Vector2I(4, 5+i));
			TileMapLayer.AddChild(fireFlowerObject);
		}

		_global.CurrentLevel = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public override void _Input(InputEvent @event)
	{
		
		//Quits the Game
		if (Input.IsActionJustPressed("exit"))
		{
			GetTree().Quit();
		}
		
		//Saves the Game
		if (Input.IsKeyPressed(Key.F1))
		{
			Global.Instance.Save(GetNode<CharacterBody2D>("%Player").Position);
		}
	}
}
