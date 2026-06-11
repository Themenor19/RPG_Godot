using Godot;
using System;
using System.Threading.Tasks;
using RPG.scripts.level_scripts;

[Tool]
public partial class BaseSpellItem : Node2D
{
	private bool _isReady;
	
	private Area2D _area;
	private Projectile _projectile;
	private Sprite2D _sprite;
	private float _fadeTimer;
	private float _fadeDuration = .5f; // seconds to fade out
	private bool _isFading;
	
	public Vector2 Velocity = Vector2.Zero;
	public float SpellSpeed = 100f;
	public Func<Area2D, Task> Interact;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_isReady = true;
		_sprite = GetNode<Sprite2D>("Sprite2D");
		_sprite = GetNode<Sprite2D>("Sprite2D");
		_projectile = GetNode<Projectile>("Projectile");
		Interact = area =>
		{
			try
			{
				if (area.GetGroups().Contains("terrain_items"))
				{
					if (area is Breakable breakable)
					{
						breakable.Break();
					}
					area.QueueFree();
					QueueFree();
				}
				else if (area.GetGroups().Contains("enemies"))
				{
					area.GetParent().QueueFree();
					QueueFree();
				}

				return Task.CompletedTask;
			}
			catch (Exception exception)
			{
				return Task.FromException(exception);
			}
		};
		_projectile.Interact = Interact;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition += Velocity * SpellSpeed * (float)delta;
		if (_isFading)
		{
			_fadeTimer += (float)delta;
			float t = Mathf.Clamp(_fadeTimer / _fadeDuration, 0f, 1f);
			float alpha = 1f - t; // 1 when t=0, 0 when t=1
			Modulate = new Color(1, 1, 1, alpha);
			if (_fadeTimer >= _fadeDuration)
			{
				_isFading = false;
				QueueFree();
			}
		}
	}
	
	public void MakeFade(float fadeDuration = .5f, bool canInteract = false)
	{
		try
		{
			_isFading = true;
			_fadeTimer = 0f;
			_fadeDuration = fadeDuration;
			_projectile.CanInteract = canInteract;
		}
		catch (Exception e)
		{
			GD.PrintErr(e.Message);
		}
	}
}
