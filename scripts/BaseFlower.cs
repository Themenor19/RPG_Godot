using System;
using System.Threading.Tasks;
using Godot;
using RPG.custom_resources.inventory;
using RPG.scripts.globals;

namespace RPG.scripts;

public partial class BaseFlower : Node2D
{
	[Export] public PackedScene SpellObject;
	private BaseSpellItem _spellItem;
	[Export] public float SpellItemFloatingSpeed = 100;
	[Export] public InventoryItemSlot Item;
	[Export] public int NumGrowPhases;
	[Export] public int NumGrowMinutes;
	private AnimatedSprite2D _sprite;
	private AnimationPlayer _animation;
	private InteractionArea _interactionArea;

	private int _dayStart = -1;
	private int _hourStart = -1;
	private int _minuteStart = -1;

	private int _growStageDuration;
	private int _currentStage = 1;
	private int _previousStage = 1;
	
	private Global _global;

	private bool _isGrowing = true;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_global = Global.Instance;
		_growStageDuration = NumGrowMinutes / NumGrowPhases;
		
		_global.GameTick += CheckGrowStatus;
		
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animation = GetNode<AnimationPlayer>("AnimationPlayer");
		_sprite.Animation = "default";
		_sprite.Frame = 0;

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
		ZIndex = -1;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!_isGrowing)
		{
			_interactionArea.CanInteract = true;
		}
	}

	private void CheckGrowStatus(int day, int hour, int minute, float secondsPerInGameMinute)
	{
		if (_dayStart == -1 || _hourStart == -1 || _minuteStart == -1)
		{
			_dayStart = day;
			_hourStart = hour;
			_minuteStart = minute;
		}
		
		if (_isGrowing)
		{
			var difference = TotalTime(day, hour, minute, _dayStart, _hourStart, _minuteStart);
			if (difference / (_currentStage * _growStageDuration) >= 1)
			{
				_currentStage++;
			}

			if (_currentStage > _previousStage)
			{
				_previousStage = _currentStage;
				_sprite.Frame = _currentStage-1;
			}

			if (_currentStage == NumGrowPhases)
			{
				_isGrowing = false;
			}

			if (_currentStage > 1)
			{
				ZIndex = 0;
			}
		}
	}

	private int TotalTime(int day1, int hour1, int minute1, int day2, int  hour2, int minute2)
	{
		var dayDifference = day1 - day2;
		var hourDifference = hour1 - hour2;
		var minuteDifference = minute1 - minute2;
		var timeDifference = Math.Abs(dayDifference*1440) + Math.Abs(hourDifference*60) +  Math.Abs(minuteDifference);
		return timeDifference;
	}
	
	public void OnInteract()
	{
		GetPicked();
	}

	public void GetPicked()
	{
		_spellItem = SpellObject.Instantiate<BaseSpellItem>();
		_spellItem.SpellSpeed = SpellItemFloatingSpeed;
		_spellItem.Velocity = Vector2.Up;
		GetParent().AddChild(_spellItem);
		_spellItem.GlobalPosition = GlobalPosition;
		_spellItem.MakeFade(canInteract: false);
		_global.AddItemToPlayer(Item, InventoryToAdd.Inventory);
		QueueFree();
	}
	
	public override void _ExitTree()
	{
		base._ExitTree();
		// If somehow _spellItem was pre-instantiated and never added to tree, free it
		if (_spellItem != null && !IsInstanceValid(_spellItem.GetParent()))
		{
			_spellItem.QueueFree();
			_spellItem = null;
		}
		_global.GameTick -= CheckGrowStatus;
	}


}
