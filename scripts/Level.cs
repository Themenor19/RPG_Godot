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

	
	
	private int _currentIndex;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_player = GetNode<CharacterBody2D>("Player");
		if (GlobalFunctions.SaveLoaded)
		{
			_player.Position = GlobalFunctions.SavedPlayerPosition;
		}
		
		var packedScene = GD.Load<PackedScene>("res://scenes/plants/skull_flower.tscn");
		var fireFlower = GD.Load<PackedScene>("res://scenes/plants/fire_flower.tscn");
		
		for (int i = 0; i < 2; i++)
		{
			var skullFlower = packedScene.Instantiate<Node2D>();
			skullFlower.GlobalPosition = TileMapLayer.MapToLocal(new Vector2I(4, 3+i));
			TileMapLayer.AddChild(skullFlower);
		}

		for (int i = 0; i < 2; i++)
		{
			var fireFlowerObject =  fireFlower.Instantiate<Node2D>();
			fireFlowerObject.GlobalPosition = TileMapLayer.MapToLocal(new Vector2I(4, 5+i));
			TileMapLayer.AddChild(fireFlowerObject);
		}
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public override void _Input(InputEvent @event)
	{
		//Changes Scale
		if (Input.IsActionJustPressed("jump"))
		{

			/*GetWindow().GetWindow().ContentScaleSize = _scales[_currentIndex];
			GD.Print(GetWindow().ContentScaleFactor);
			_currentIndex = (_currentIndex + 1) % _scales.Count;*/

			GetWindow().ContentScaleFactor = _scaleFactors[_currentIndex];
			_currentIndex =  (_currentIndex + 1) % _scaleFactors.Count;
		}
		
		//Quits the Game
		if (Input.IsActionJustPressed("exit"))
		{
			GetTree().Quit();
		}
		
		//Saves the Game
		if (Input.IsKeyPressed(Key.F1))
		{
			GlobalFunctions.Save(GetNode<CharacterBody2D>("%Player").Position);
		}
	}
}
