using Godot;
using System;
using RPG.scripts;

public partial class FlowerLight : PointLight2D
{
	private int _previousHour = -1;

	// Called when the node enters the scene tree for the first time.

	// Called every frame. 'delta' is the elapsed time since the previous frame.

	public override void _Ready()
	{
		Visible = false;
		Global.Instance.GameTick += _on_time_tick;
	}
	
	public override void _Process(double delta)
	{
		
	}

	private void _on_time_tick(int day, int hour, int minute, float secondsPerInGameMinute)
	{
		var hour24 = hour % 24;
		var hour12 = hour % 12;

		if (_previousHour != hour24)
		{
			_previousHour = hour24;

			float targetEnergy;

			if (hour24 >= 16)
			{
				// 18:00 → 0f, 24:00 → 6f
				float t = (hour24 - 16f) / 8f;
				targetEnergy = Math.Clamp(6f * t, 0f, 6f);
			}
			else if (hour24 < 6)
			{
				// 0:00 → 6f, 6:00 → 0f
				float t = 1f - (hour24 / 6f);
				targetEnergy = Math.Clamp(6f * t, 0f, 6f);
			}
			else if (hour24 < 7 && minute <= 15)
			{
				// Daytime, lights off
				targetEnergy = 0f;
			}
			else
			{
				Energy =  0f;
				return;
			}

			// Smoothly transition to target over the duration of one in-game hour
			Tween tween = CreateTween();
			tween.TweenProperty(this, "energy", targetEnergy, secondsPerInGameMinute * 60f); // 60 seconds real time
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		Global.Instance.GameTick -= _on_time_tick;
	}
}
