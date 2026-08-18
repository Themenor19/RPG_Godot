using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using RPG.custom_resources.inventory;
using RPG.scripts.globals;

namespace RPG.scripts;

public partial class BaseFlower : Node2D
{
	public const string UidKey = "Uid";
	private const string PlantedCoordinatesKey = "PlantedCoordinates";
	private const string DayStartKey = "DayStart";
	private const string HourStartKey = "HourStart";
	private const string MinuteStartKey = "MinuteStart";
	private const string GrowStageDurationKey = "GrowStageDuration";
	private const string CurrentStageKey = "CurrentStage";
	private const string PreviousStageKey = "PreviousStage";
	private const string IsGrowingKey = "IsGrowing";
	
	[Export] public PackedScene SpellObject;
	private scenes.projectiles.spells.BaseSpellItem _spellItem;
	[Export] public float SpellItemFloatingSpeed = 100;
	[Export] public InventoryItemSlot Item;
	[Export] public int NumGrowPhases;
	[Export] public int NumGrowMinutes;

	private Level _parentLevel;
	private Vector2I _plantedCoordinates;

	private AnimatedSprite2D _sprite;
	private AnimationPlayer _animation;
	private InteractionArea _interactionArea;

	private int _dayStart = -1;
	private int _hourStart = -1;
	private int _minuteStart = -1;

	private int _growStageDuration;
	private int _currentStage = 1;
	private int _previousStage = 1;

	private GlobalHandler _global;

	private bool _isGrowing = true;

	// Called when the node enters the scene tree for the first time.

	public void Init(Level level, Vector2I plantedCoordinates)
	{
		_parentLevel = level;
		_plantedCoordinates = plantedCoordinates;
	}

	public InitFromSaveReturn InitFromSave(Level level, Dictionary data)
	{
		_parentLevel = level;

		try
		{
			_plantedCoordinates = (Vector2I)data[PlantedCoordinatesKey];
			_dayStart =  (int)data[DayStartKey];
			_hourStart =  (int)data[HourStartKey];
			_minuteStart =  (int)data[MinuteStartKey];
			_growStageDuration =  (int)data[GrowStageDurationKey];
			_currentStage = (int)data[CurrentStageKey];
			_previousStage = (int)data[PreviousStageKey];

			return new InitFromSaveReturn
			{
				Error = Error.Ok,
				PlantedCoordinates = _plantedCoordinates,
			};
		}
		catch
		{
			return new InitFromSaveReturn
			{
				Error = Error.ParseError
			};
		}
	}

	public override void _Ready()
	{
		_global = GetTree().GetRoot().GetChildren().OfType<GlobalHandler>().FirstOrDefault();
		_growStageDuration = NumGrowMinutes / NumGrowPhases;

		if (_global != null) _global.GameTick += CheckGrowStatus;

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
			}

			if (_currentStage == NumGrowPhases)
			{
				_isGrowing = false;
			}

			if (_currentStage > 1)
			{
				ZIndex = 0;
			}

			_sprite.Frame = _currentStage - 1;
		}
	}

	private int TotalTime(int day1, int hour1, int minute1, int day2, int hour2, int minute2)
	{
		var dayDifference = day1 - day2;
		var hourDifference = hour1 - hour2;
		var minuteDifference = minute1 - minute2;
		var timeDifference =
			Math.Abs(dayDifference * 1440) + Math.Abs(hourDifference * 60) + Math.Abs(minuteDifference);
		return timeDifference;
	}

	public void OnInteract()
	{
		GetPicked();
	}

	public void GetPicked()
	{
		_spellItem = SpellObject.Instantiate<scenes.projectiles.spells.BaseSpellItem>();
		_spellItem.SpellSpeed = SpellItemFloatingSpeed;
		_spellItem.Velocity = Vector2.Up;
		GetParent().AddChild(_spellItem);
		_spellItem.GlobalPosition = GlobalPosition;
		_spellItem.MakeFade(canInteract: false);
		_global.AddItemToPlayer(Item, InventoryToAdd.Inventory);
		try
		{
			_parentLevel.PlantedSlots.Remove(_plantedCoordinates);
		}
		catch (Exception e)
		{
			GD.PrintErr("Plant couldn't be picked: " + e.Message);
		}

		QueueFree();
	}

	public Dictionary GetSaveData()
	{
		long uid = ResourceLoader.GetResourceUid(SceneFilePath);
		var uidString = ResourceUid.IdToText(uid);
		
		var saveData = new Dictionary
		{
			{UidKey, uidString},
			{PlantedCoordinatesKey, _plantedCoordinates},
			{ DayStartKey, _dayStart },
			{ HourStartKey, _hourStart },
			{ MinuteStartKey, _minuteStart },
			{ GrowStageDurationKey, _growStageDuration },
			{ CurrentStageKey, _currentStage },
			{ PreviousStageKey, _previousStage },
			{ IsGrowingKey, _isGrowing },
		};
		
		return saveData;
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

public class InitFromSaveReturn
{
	public Error Error { get; set; }
	public Vector2I PlantedCoordinates { get; set; } = Vector2I.Zero;
}
