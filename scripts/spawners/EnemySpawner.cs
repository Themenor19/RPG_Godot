using Godot;
using RPG.custom_resources.enemy_spawner_presets;

namespace RPG.scripts.spawners;

public partial class EnemySpawner : Node2D
{
	[Export] public BaseEnemySpawnerPreset Preset;
	[Export] public Node Container;
	public override void _Ready()
	{
		for (int i = 0; i < Preset.NumberOfEnemies; i++)
		{
			var temp = Preset.Enemy.Instantiate<Node2D>();
			temp.Visible = false;
			if (Container != null)
			{
				Container.AddChild(temp);
			}
			else
			{
				AddChild(temp);
			}
			temp.GlobalPosition = GlobalPosition;
			temp.ResetPhysicsInterpolation();
			temp.Visible = true;
		}
	}
}
