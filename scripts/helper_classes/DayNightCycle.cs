using System;
using Godot;

namespace RPG.scripts.helper_classes;

public partial class DayNightCycle : Control
{
	private const int MinutesPerDay = 1440;
	private const int MinutesPerHour = 60;
	private const float InGameToRealMinuteDuration = (float)(2 * Math.PI) / MinutesPerDay;

	[Signal] public delegate void TimeTickEventHandler(int day, int hour, int minute, float realSecondsPerInGameMinute);
	[Signal] public delegate void ColorChangedEventHandler(Color color);

	[Export] public GradientTexture1D Gradient;
	[Export] public float InGameSpeed = 1f;
	[Export] public ColorRect Overlay; // assign in editor
	
	
	private int _initialHour = 12;

	[Export]
	public int InitialHour
	{
		get => _initialHour;
		set
		{
			_initialHour = value;
			_time = InGameToRealMinuteDuration * _initialHour * MinutesPerHour;
		}
	}

	private float _time;
	private int _pastMinute = -1;

	public override void _Ready()
	{
		_time = InGameToRealMinuteDuration * InitialHour * MinutesPerHour;
		var global = GetNode<Global>("/root/Global");
		Connect(SignalName.TimeTick, new Callable(global, nameof(global._on_time_tick)));
	}

	public override void _Process(double delta)
	{
		_time += (float)delta * InGameToRealMinuteDuration * InGameSpeed;
		var value = (Mathf.Sin(_time - Math.PI / 2f) + 1f) / 2f;
		var color = Gradient.Gradient.Sample((float)value);

		if (Overlay != null)
		{
			Overlay.Material.Set("shader_parameter/tint", color);
			
		}
		else
		{
			GD.Print("Overlay is null!");
		}
		EmitSignal(SignalName.ColorChanged, color);
		RecalculateTime();
	}
	
	public float GetRealSecondsPerInGameMinute()
	{
		return 1f / InGameSpeed;
	}

	private void RecalculateTime()
	{
		var totalMinutes = (int)(_time / InGameToRealMinuteDuration);
		var day = (int)(totalMinutes / MinutesPerDay);
		var currentDayMinutes = totalMinutes % MinutesPerDay;
		var hour = currentDayMinutes / MinutesPerHour;
		var minute = currentDayMinutes % MinutesPerHour;

		if (_pastMinute != minute)
		{
			_pastMinute = minute;
			EmitSignal(SignalName.TimeTick, day, hour, minute, GetRealSecondsPerInGameMinute());
		}
	}
}
