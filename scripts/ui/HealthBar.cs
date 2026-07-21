using Godot;
using Global = RPG.scripts.globals.Global;

[GlobalClass]
[Tool]
public partial class HealthBar : Control
{
	[Signal] public delegate void DeadEventHandler();
	
	
	private Label _label; 
	private Global _global;

	private int _baseHealth;
	private int _currentHealth;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		_global = Global.Instance;
		_label = GetNode<Label>("MarginContainer/Label");
	}
	public void SetHealthBar(int currentHealth, int baseHealth)
	{
		_baseHealth = baseHealth;
		_currentHealth = currentHealth;
		_label.Text = $"{_currentHealth}/{_baseHealth}";
	}

	public void AddBaseHealth(int baseHealthAddition)
	{
		if (baseHealthAddition >= 0)
		{
			_currentHealth += baseHealthAddition;
		}
		_baseHealth += baseHealthAddition;
		if (_baseHealth < 0)
		{
			_baseHealth = 0;
			CallDeferred(nameof(EmitDead));
		}
		if (_currentHealth > _baseHealth)
		{
			_currentHealth = _baseHealth;
		}

		SetHealthBar(_currentHealth, _baseHealth);
	}

	public void AddCurrentHealth(int currentHealth)
	{
		_currentHealth += currentHealth;
		if (_currentHealth > _baseHealth)
		{
			_currentHealth = _baseHealth;
		}

		if (_currentHealth <= 0)
		{
			_currentHealth = 0;
			CallDeferred(nameof(EmitDead));
		}
		SetHealthBar(_currentHealth, _baseHealth);
	}
	
	public int GetBaseHealth()
	{
		return _baseHealth;
	}
	public int GetCurrentHealth()
	{
		return _currentHealth;
	}
	
	public void EmitDead()
	{
		EmitSignal(SignalName.Dead);
	}
}
