using System;
using Godot;

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

	public const float Speed = 100.0f;
	public AnimatedSprite2D Sprite;
	private scenes.ui.inventory.InventoryUi _inventory;
	public LookDirection Direction = LookDirection.South;
	private bool _isPlanting = false;
	[Export] public int BaseHealth = 100;
	[Export] public int CurrentHealth = 10;
	[Export]
	public float SpellSpeed = 100f;

	public override void _Ready()
	{ 
		Sprite = GetNode<AnimatedSprite2D>("PlayerSprite");
		Sprite.Play("front_standing_idle");
		Global.Instance.PlayerNode = this;
		_inventory = GetNode<scenes.ui.inventory.InventoryUi>("CanvasLayer/Inventory_Ui");
		_inventory.Visible = false;

		
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

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("hotbar_1"))
		{
			_isPlanting = !_isPlanting;
			EmitSignal(SignalName.IsPlanting);

		}
		if (@event.IsActionPressed("action_fire") && !GetTree().Paused && !_isPlanting)
		{
			var spell = Global.Spells["fire"].Instantiate<BaseSpellItem>();
			
			switch (Direction)
			{
				case LookDirection.North: 
					spell.GlobalRotationDegrees = 180f;
					spell.Velocity = Vector2.Up;
					break;
				case LookDirection.South: 
					spell.GlobalRotationDegrees = 0f; 
					spell.Velocity = Vector2.Down;
					break;
				case LookDirection.West: 
					spell.GlobalRotationDegrees = 90f;
					spell.Velocity = Vector2.Left;
					break;
				case LookDirection.East: 
					spell.GlobalRotationDegrees = -90f;
					spell.Velocity = Vector2.Right;
					break;
			}

			spell.SpellSpeed = SpellSpeed;
			
			GetParent().AddChild(spell);
			spell.GlobalPosition = GlobalPosition;

		}
		else if (@event is InputEventMouseButton eventButton && eventButton.ButtonIndex == MouseButton.Left && eventButton.Pressed && !GetTree().Paused && !_isPlanting)
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
			GetTree().Paused = !GetTree().Paused;
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

	public void ApplyItemEffect(InventoryItem item)
	{
		switch (item.Effect)
		{
			case ItemEffects.Heal:
				Heal(item.HealAmount);
				break;
		}
	}

	private void Heal(int health)
	{
		CurrentHealth += health;
		if (CurrentHealth > BaseHealth)
		{
			CurrentHealth = BaseHealth;
		}
	}
}
