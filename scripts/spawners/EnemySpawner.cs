using Godot;

namespace RPG.scripts.spawners;

public partial class EnemySpawner : Node2D
{
	[Export] public PackedScene Enemy;
	[Export] public int NumberOfEnemies;

	public override void _Ready()
	{
		for (int i = 0; i < NumberOfEnemies; i++)
		{
			var temp = Enemy.Instantiate<Node2D>();
			temp.GlobalPosition = GlobalPosition;
			AddChild(temp);lkjkwerf
		}
	}
}
