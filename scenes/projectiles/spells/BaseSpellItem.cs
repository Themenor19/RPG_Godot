using Godot;
using System;
using System.Threading.Tasks;

public partial class BaseSpellItem : Node2D
{
	private Area2D _area;
	private Projectile _projectile;
	private float _fadeTimer = 0f;
	private float _fadeDuration = .5f; // seconds to fade out
	private bool _isFading = false;
	
	public Vector2 Velocity = Vector2.Zero;
	public float SpellSpeed = 100f;
	public Func<Area2D, Task> Interact = null;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_projectile = GetNode<Projectile>("Projectile");
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
	
	public async void MakeFade(float fadeDuration = .5f, bool canInteract = false)
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
