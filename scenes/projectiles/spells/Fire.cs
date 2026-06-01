using System;
using System.Threading.Tasks;
using Godot;

namespace RPG.scenes.projectiles.spells;

public partial class Fire : BaseSpellItem
{
	public override void _Ready()
	{
		Interact = (Area2D area) =>
		{
			try
			{
				if (area.GetGroups().Contains("terrain_items"))
				{ 
					area.QueueFree();
					QueueFree();
				}
				else if (area.GetGroups().Contains("enemies"))
				{
					area.GetParent().QueueFree();
					QueueFree();
				}

				return Task.CompletedTask;
			}
			catch (Exception exception)
			{
				return Task.FromException(exception);
			}
		};
		
		base._Ready();
	}
}
