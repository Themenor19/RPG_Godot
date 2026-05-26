using System;
using System.Threading.Tasks;
using Godot;

namespace RPG.scripts;

public partial class SkullFlower : Node2D
{
	[Export] public Node2D SpellItem = new();
	private AnimatedSprite2D _sprite;
	private InteractionArea _interactionArea;
	

	private bool _isGrowing;

	private float _fadeTimer = 0f;
	private float _fadeDuration = 2f; // seconds to fade out
	private bool _isFading = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		
		_sprite.Play("default");

		_interactionArea = GetNode<InteractionArea>("InteractionArea");
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
		_fadeTimer += (float)delta;
		float t = Mathf.Clamp(_fadeTimer / _fadeDuration, 0f, 1f);
		float alpha = 1f - t; // 1 when t=0, 0 when t=1
		SpellItem.SelfModulate = new Color(1, 1, 1, alpha);

		if (_fadeTimer >= _fadeDuration)
		{
			_isFading = false;
		}
	}
	
	public void OnInteract()
	{
		GetPicked();
	}

	public void GetPicked()
	{
		_isFading = true;
		_fadeTimer = 0f;
		SpellItem.Visible = true;
		_interactionArea.Visible = false;
		_sprite.Visible = false;
	}
}
