using System;
using System.Linq;
using Godot;
using RPG.custom_resources.inventory;
using RPG.scripts.character_components;
using RPG.scripts.globals;
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
	private GlobalHandler _global;
	[Signal]
	public delegate void IsPlantingEventHandler(bool isPlanting);
	
	[Export] private SpellCaster _spellCaster;
	[Export] private InventoryUi _inventory;
	[Export] private InventoryHotbar _inventoryHotbar;
	[Export] private character_components.HitBox _hitBox;
	[Export] public HealthBar HealthBar;
	[Export] public Camera2D Camera;
	[Export] public float SpellSpeed = 100f;
	[Export] public int StartingHealth = 100;
	[Export] public int BaseHealth = 100;
	[Export] public AnimatedSprite2D Hoe;

	public const float Speed = 120.0f;
	public AnimatedSprite2D Sprite;
	public LookDirection Direction = LookDirection.South;

	private bool _isPlanting;


	public override void _Ready()
	{ 
		Sprite = GetNode<AnimatedSprite2D>("PlayerSprite");
		Sprite.Play("front_standing_idle");
		_global.PlayerNode = this;
		_inventory.Visible = false;
		HealthBar.SetHealthBar(StartingHealth, BaseHealth);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (GetTree().Paused || HealthBar.GetCurrentHealth()<=0)  return;
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
				Hoe.FlipH = !goingRight;
				Hoe.Position = !goingRight ? new Vector2(-15, -20) : new Vector2(15, -20);
			}
			else
			{
				bool goingUp = direction.Y < 0;
				Sprite.Play(goingUp ? "walk_up" : "walk_down");
				Direction = goingUp ? LookDirection.North : LookDirection.South;
			}
			velocity = direction.Normalized() * Speed;
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

	private void HotbarUpdated(Inventory hotbar, Inventory playerInventory)
	{
		var selectedSlot = _inventoryHotbar.SlotSelected;
		if (selectedSlot > -1 && hotbar.Items[selectedSlot] == null)
		{
			_isPlanting = false;
			EmitSignal(SignalName.IsPlanting, _isPlanting);
		}
	}
	
	public override void _Input(InputEvent @event)
	{
		var hotbarSlotNum = IsHotbarPressed(@event);
		if (hotbarSlotNum != -1) 
		{
			_inventoryHotbar.CheckHotbarSelected(hotbarSlotNum);
			if (_global.PlayerInventory.Items[hotbarSlotNum]?.Item is {Type: ItemTypes.Seed} && _global.CurrentLevel is {CanPlant: true})
			{
				_isPlanting = !_isPlanting;
				EmitSignal(SignalName.IsPlanting, _isPlanting);
			}
			else
			{
				_isPlanting = false;
				EmitSignal(SignalName.IsPlanting, _isPlanting);
			}
		}

		if (@event is InputEventMouseButton eventButton)
		{
			if (eventButton.ButtonIndex == MouseButton.Left && eventButton.Pressed && !GetTree().Paused)
			{
				var itemIndex = _inventoryHotbar.GetSelectedItemIndex();
				if (itemIndex == -1) return;
				var item = _global.PlayerInventory.Items[itemIndex];
				if (item == null) return;
				var effectApplied = CheckItemType(item.Item);
				if (effectApplied)
				{
					_global.RemoveItem(item, itemIndex, 1);
				}
			}

			if (eventButton.ButtonIndex == MouseButton.WheelUp && eventButton.Pressed && !GetTree().Paused)
			{
				Camera.Zoom += new Vector2(0.25f, 0.25f);
			}

			if (eventButton.ButtonIndex == MouseButton.WheelDown && eventButton.Pressed && !GetTree().Paused) 
			{
				Camera.Zoom -= new Vector2(0.25f, 0.25f);
			}
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
			var nodes = GetTree().GetNodesInGroup("player");
			if (nodes.Count > 0 && nodes[0] is Player player)
			{
				_global.BinarySave(player.Position);
			}
		}

		if (Input.IsKeyPressed(Key.F2))
		{
			GetWindow().Mode = Window.ModeEnum.Maximized;
		}

		if (Input.IsKeyPressed(Key.F3))
		{
			GetWindow().Mode = Window.ModeEnum.Fullscreen;
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
		if (item == null) return false;
		switch (item.Type)
		{
			case ItemTypes.Consumable:
				return ApplyItemEffect(item);
			case ItemTypes.Spell:
				return _spellCaster.CastSpell(item, SpellSpeed, GetGlobalMousePosition(), GetParent(), GlobalPosition, _hitBox);
			case ItemTypes.Seed:
				return _global.CurrentLevel.Plant(item);
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

	public override void _ExitTree()
	{
		base._ExitTree();
		_global.PlayerInventoryUpdated -= HotbarUpdated;
		_global = null;
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		_global = GetTree().GetRoot().GetChildren().OfType<GlobalHandler>().FirstOrDefault();
		if (_global != null) _global.PlayerInventoryUpdated += HotbarUpdated;
	}
}
