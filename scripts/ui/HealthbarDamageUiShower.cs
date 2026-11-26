using Godot;
using System;
using MagicFarmer.scripts.ui;

public partial class HealthbarDamageUiShower : Node2D
{
	[Export] public ColorRect HealthIndicator;
	[Export] public HealthPool HealthPool;

	private float _originalSizeX;
	
	public override void _Ready()
	{
		_originalSizeX = HealthIndicator?.Size.X ?? 0;
	}

	private void AdjustIndicator(int currentHealth, int baseHealth)
	{
		float percentage = (float)currentHealth / baseHealth;
		var sizex = (int)Mathf.Round(_originalSizeX * percentage);
		HealthIndicator.Size = new Vector2(sizex, HealthIndicator.Size.Y);
	}
	
	public override void _EnterTree()
	{
		base._EnterTree();
		HealthPool.HealthChanged += AdjustIndicator;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		HealthPool.HealthChanged -= AdjustIndicator;
	}
}
