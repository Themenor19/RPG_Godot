using System;
using Godot;
using RPG.scripts.ui;

namespace RPG.scripts;

public enum LookDirection
{
	North,
	South,
	East,
	West
}

public partial class Player : CharacterBody2D
{
	[Signal]
	public delegate void IsPlantingEventHandler();
	
	[Export] private InventoryUi _inventory;
	[Export] private ui.InventoryHotbar _inventoryHotbar;
	[Export] public HealthBar HealthBar;
	[Export] public float SpellSpeed = 100f;
	[Export] public int StartingHealth = 100;
	[Export] public int BaseHealth = 100;

	public const float Speed = 100.0f;
	public AnimatedSprite2D Sprite;
	public LookDirection Direction = LookDirection.South;

	private bool _isPlanting;


	public override void _Ready()
	{ 
		Sprite = GetNode<AnimatedSprite2D>("PlayerSprite");
		Sprite.Play("front_standing_idle");
		Global.Instance.PlayerNode = this;
		_inventory.Visible = false;
		HealthBar.SetHealthBar(StartingHealth, BaseHealth);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (GetTree().Paused)  return;
		Vector2 velocity = Velocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("left", "right", "up", "down");
		if (direction != Vector2.Zero)
		{

			if (Math.Abs(direction.X) > Math.Abs(direction.Y))
			{
				bool goingRight = direction.X > 0;
				Sprite.Play(goingRight? "walk_right" : "walk_left");
				Direction = goingRight ? LookDirection.East:  LookDirection.West;
			}
			else
			{
				bool goingUp = direction.Y < 0;
				Sprite.Play(goingUp ? "walk_up" : "walk_down");
				Direction = goingUp ? LookDirection.North : LookDirection.South;
			}
			velocity.X = direction.X * Speed;
			velocity.Y = direction.Y * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);

			switch (Direction)
			{
				case LookDirection.South:
					Sprite.Play("front_standing_idle");
					break;
				case LookDirection.North:
					Sprite.Play("back_standing_idle");
					break;
				case LookDirection.West:
					Sprite.Play("left_standing_idle");
					break;
				case LookDirection.East:
					Sprite.Play("right_standing_idle");
					break;
				default:
					Sprite.Play("front_standing_idle");
					break;
			}
			
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private int IsHotbarPressed(InputEvent @event)
	{
		for (int i = 0; i < 5; i++)
		{
			if (@event.IsActionPressed($"hotbar_{i + 1}"))
				return i;
		}
		return -1;
	}
	
	public override void _Input(InputEvent @event)
	{
		var hotbarSlotNum = IsHotbarPressed(@event);
		if (hotbarSlotNum != -1) 
		{
			/*_isPlanting = !_isPlanting;
			EmitSignal(SignalName.IsPlanting);*/
			
			_inventoryHotbar.CheckHotbarSelected(hotbarSlotNum);
		}
		
		if (@event is InputEventMouseButton eventButton && eventButton.ButtonIndex == MouseButton.Left && eventButton.Pressed && !GetTree().Paused && !_isPlanting)
		{
			var spell = Global.Spells["fire"].Instantiate<BaseSpellItem>();

			spell.SpellSpeed = SpellSpeed;
			GetParent().AddChild(spell);
			spell.GlobalPosition = GlobalPosition;
			spell.Velocity = spell.GlobalPosition.DirectionTo(GetGlobalMousePosition()).Normalized();
			spell.GlobalRotation = spell.Velocity.Angle() - MathF.PI / 2f;
			
		}

		if (@event.IsActionPressed("ui_inventory"))
		{
			_inventory.Visible = !_inventory.Visible;
			_inventoryHotbar.Visible = !_inventoryHotbar.Visible;
			GetTree().Paused = !GetTree().Paused;
		}
		
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

	private void _on_color_changed(Color color)
	{
		// Luminance of the current day/night color
		float luminance = 0.299f * color.R + 0.587f * color.G + 0.114f * color.B;
	
		// Invert it — dark night = high intensity, bright day = low intensity
		float glowIntensity = 1f - luminance;
	
		// Scale between a min and max glow strength
		float minGlow = 1.0f;
		float maxGlow = 4.0f;
		float intensity = minGlow + glowIntensity * (maxGlow - minGlow);
	
		Sprite.SetInstanceShaderParameter("GlowColor", new Color(
			1.317f * intensity,
			1.306f * intensity,
			0.375f * intensity
		));
	}

	public bool CheckItemType(InventoryItem item)
	{
		switch (item.Type)
		{
			default:
				return false;
		}
	}
	
	public bool ApplyItemEffect(InventoryItem item)
	{
		switch (item.Effect)
		{
			case ItemEffects.Heal:
				Heal(item.HealAmount);
				return true;
			case ItemEffects.Damage:
				Damage(item.Damage);
				return true;
			default:
				return false;
		}
	}

	private void Heal(int health)
	{
		HealthBar.AddCurrentHealth(health);
	}
	
	private void Damage(int damage)
	{
		HealthBar.AddCurrentHealth(damage);
	}
}
