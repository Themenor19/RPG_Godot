using System;
using System.Threading.Tasks;
using Godot;
using RPG.scripts.level_scripts;

namespace RPG.scenes.projectiles.spells;

[Tool]
public partial class BaseSpellItem : Node2D
{
	private bool _isReady;
	
	public Area2D Area;
	public Projectile Projectile;
	public Sprite2D Sprite;
	private float _fadeTimer;
	private float _fadeDuration = .5f; // seconds to fade out
	private bool _isFading;
	
	public Vector2 Velocity = Vector2.Zero;
	public float SpellSpeed = 100f;
	public Func<Area2D, Task> Interact;

	public scripts.character_components.HitBox ParentHitbox;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_isReady = true;
		Sprite = GetNode<Sprite2D>("Sprite2D");
		Sprite = GetNode<Sprite2D>("Sprite2D");
		Projectile = GetNode<Projectile>("Projectile");
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
				else if (area is scripts.character_components.HitBox hitbox && hitbox != ParentHitbox)
				{
					hitbox.AddCurrentHealth(-Projectile.Damage);
					QueueFree();
				}
				return Task.CompletedTask;
			}
			catch (Exception exception)
			{
				return Task.FromException(exception);
			}
		};
		Projectile.Interact = Interact;
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
			Projectile.CanInteract = canInteract;
		}
		catch (Exception e)
		{
			GD.PrintErr(e.Message);
		}
	}

	
	
	public void Cast(Vector2 velocity, float angle, int damage, scripts.character_components.HitBox parentHitBox)
	{
		ParentHitbox = parentHitBox;
		Velocity = velocity;
		GlobalRotation = angle;
		Projectile.Damage = damage;
	}
}
