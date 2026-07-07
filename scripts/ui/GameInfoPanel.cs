using Godot;
using Global = RPG.scripts.globals.Global;

namespace RPG.scripts.ui;

public partial class GameInfoPanel : Control
{
	[Export] private RichTextLabel _coinAmountLabel;
	private Global _global;
	private RichTextLabel _timeLabel;
	private RichTextLabel _dayLabel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timeLabel = GetNode<RichTextLabel>("NinePatchRect/Time");
		_dayLabel = GetNode<RichTextLabel>("NinePatchRect/Day");
		SetCoinAmount(_global.CoinAmount);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_time_tick(int day, int hour, int minute, float secondsPerInGameMinute)
	{
		var hourString = hour < 10 ? $"0{hour}" : $"{hour}";
		var minuteString = minute < 10 ? $"0{minute}" : $"{minute}";
		var dayString = day < 10 ? $"0{day}" : $"{day}";
		_timeLabel.Text = $"{hourString}:{minuteString}";
		_dayLabel.Text = $"{dayString}";
	}
	
	private void SetCoinAmount(int coinAmount)
	{
		_coinAmountLabel.Text = coinAmount.ToString();
	}

	public override void _ExitTree()
	{
		_global.GameTick -= _on_time_tick;
		_global.CoinAmountChanged -= SetCoinAmount;
		base._ExitTree();
	}

	public override void _EnterTree()
	{
		_global = Global.Instance;
		_global.GameTick += _on_time_tick;
		_global.CoinAmountChanged += SetCoinAmount;
	}
}