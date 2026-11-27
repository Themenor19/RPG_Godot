using System;
using System.Threading.Tasks;
using Godot;

namespace MagicFarmer.scripts.components.entity;

public partial class HealthbarDamageUiShower : Node2D
{
	[Export] public ColorRect HealthIndicator;
	[Export] public HealthBar HealthBar;

	private float _originalSizeX;
	
	public override void _Ready()
	{
		_originalSizeX = HealthIndicator?.Size.X ?? 0;
		HealthBar?.UpdateHealthFunction = AdjustIndicator;
	}
	
	private Task AdjustIndicator(int currentHealth, int baseHealth)
	{
		try
		{
			float percentage = (float)currentHealth / baseHealth;
			var sizex = (int)Mathf.Round(_originalSizeX * percentage);
			HealthIndicator.Size = new Vector2(sizex, HealthIndicator.Size.Y);

			return Task.CompletedTask;
		}
		catch (Exception e)
		{
			return Task.FromException(e);
		}
	}
	
	
}
