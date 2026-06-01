using Godot;
using System;

public partial class Item : Control
{
	private TextureRect _textureRect;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_textureRect = GetNode<TextureRect>("TextureRect");
		_textureRect.Texture = GD.Load<Texture2D>("res://assets/Sprites/items/fire.png");
	}

	
}
