using System;
using System.Threading.Tasks;
using Godot;

namespace RPG.scripts;

public partial class SkullFlower : Node2D
{
	[Export] public PackedScene SpellItem;
	private BaseSpellItem _spellItem;
	[Export] public float SpellItemFloatingSpeed = 100;
	private AnimatedSprite2D _sprite;
	private InteractionArea _interactionArea;


	private bool _isGrowing = true;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		
		_sprite.Play("default");

		_interactionArea = GetNode<InteractionArea>("InteractionArea");
		_interactionArea.CanInteract = false;
		_interactionArea.Interact = () =>
		{
			try
			{
				OnInteract();
				return Task.CompletedTask;
			}
			catch (Exception exception)
			{
				return Task.FromException(exception);
			}
		};
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!_sprite.IsPlaying())
		{
			_isGrowing = false;
			_interactionArea.CanInteract = true;
		}
	}
	
	public void OnInteract()
	{
		GetPicked();
	}

	public void GetPicked()
	{
		_spellItem = SpellItem.Instantiate<BaseSpellItem>();
		_spellItem.SpellSpeed = SpellItemFloatingSpeed;
		_spellItem.Velocity = Vector2.Up;
		GetParent().AddChild(_spellItem);
		_spellItem.GlobalPosition = GlobalPosition;
		_spellItem.MakeFade(canInteract: false); 
		QueueFree();
	}
	
	public override void _ExitTree()
	{
		// If somehow _spellItem was pre-instantiated and never added to tree, free it
		if (_spellItem != null && !IsInstanceValid(_spellItem.GetParent()))
		{
			_spellItem.QueueFree();
			_spellItem = null;
		}
	}
}
