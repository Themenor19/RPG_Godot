using Godot;
using RPG.scripts.level_scripts;

namespace RPG.scripts.terrain_items.portals;

[Tool]
public partial class LevelTransitionPortal : Node2D
{
	
	[Export] public LevelTransition Transition;

	[Export] public string LevelUid;

	[Export] public string ConnectingSpawnerName;

	public override void _Ready()
	{
		Transition.LevelReference = LevelUid;
		Transition.SpawnerName = ConnectingSpawnerName;
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
