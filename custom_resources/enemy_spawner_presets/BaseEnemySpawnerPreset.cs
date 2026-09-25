using Godot;

namespace RPG.custom_resources.enemy_spawner_presets;


[GlobalClass]
public partial class BaseEnemySpawnerPreset: Resource
{
    [Export] public PackedScene Enemy;
    [Export] public int NumberOfEnemies;
}