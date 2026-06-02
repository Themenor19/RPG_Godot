using Godot;
using System;

public partial class GameInfoPanel : Control
{
	private RichTextLabel _timeLabel;
	private RichTextLabel _dayLabel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timeLabel = GetNode<RichTextLabel>("NinePatchRect/Time");
		_dayLabel = GetNode<RichTextLabel>("NinePatchRect/Day");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_time_tick(int day, int hour, int minute, float secondsPerIngameMinute)
	{
		var hourString = hour < 10 ? $"0{hour}" : $"{hour}";
		var minuteString = minute < 10 ? $"0{minute}" : $"{minute}";
		var dayString = day < 10 ? $"0{day}" : $"{day}";
		_timeLabel.Text = $"{hourString}:{minuteString}";
		_dayLabel.Text = $"{dayString}";
	}
}
