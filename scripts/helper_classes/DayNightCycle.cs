using System;
using Godot;
using RPG.scripts.globals;

namespace RPG.scripts.helper_classes;

public partial class DayNightCycle : Control
{
	public static DayNightCycle Instance { get; private set; }
	private const int MinutesPerDay = 1440;
	private const int MinutesPerHour = 60;
	private const float InGameToRealMinuteDuration = (float)(2 * Math.PI) / MinutesPerDay;

	private Global _global;
	
	[Signal] public delegate void TimeTickEventHandler(int day, int hour, int minute, float realSecondsPerInGameMinute);
	[Signal] public delegate void ColorChangedEventHandler(Color color);

	public GradientTexture1D Gradient;
	[Export] public float InGameSpeed = 1f;

	public bool Paused = true;
	
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
		Instance = this;
		_global = Global.Instance;
		Gradient = GD.Load<GradientTexture1D>("uid://dgoofvfiyvt35");
		Init();
		Connect(SignalName.TimeTick, new Callable(_global, nameof(_global._on_time_tick)));
	}

	public void Init()
	{
		if (_global.StartingTime >= 0f)
		{
			_time =  _global.StartingTime;
		}
		else
		{
			_time = InGameToRealMinuteDuration * InitialHour * MinutesPerHour;
		}
	}

	public override void _Process(double delta)
	{
		if (!Paused)
		{
			_time += (float)delta * InGameToRealMinuteDuration * InGameSpeed;
			var value = (Mathf.Sin(_time - Math.PI / 2f) + 1f) / 2f;
			var color = Gradient.Gradient.Sample((float)value);
			EmitSignal(SignalName.ColorChanged, color);
			RecalculateTime();
		}
	}
	
	public float GetRealSecondsPerInGameMinute()
	{
		return 1f / InGameSpeed;
	}

	public float GetCurrentTime()
	{
		return _time;
	}

	private void RecalculateTime()
	{
		var totalMinutes = (int)(_time / InGameToRealMinuteDuration);
		var day = totalMinutes / MinutesPerDay;
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
