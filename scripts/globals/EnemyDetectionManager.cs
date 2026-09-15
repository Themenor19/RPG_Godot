using System;
using System.Collections.Generic;
using Godot;
using RPG.scripts.enemy_components;

namespace RPG.scripts.globals;

public partial class EnemyDetectionManager: Node2D
{
    public static EnemyDetectionManager Instance { get; private set; }

    private readonly List<DetectionArea> _detectionAreas = [];
    private float _detectionScale;

    public override void _Ready()
    {
        Instance = this;
    }
    // I want to make it so that this class is in the global handler and that enemies can change their scale using the manager (if they want to double their radius, they call a function in the manager) and I want to make it so that
    // from the global handler, you can select whether or not enemies can see the player or not (this will be used later for other stuff such as invisibility).
    ;skdjflkj
    public void RegisterArea(DetectionArea area)
    {
        if (Math.Abs(area.RadiusScale - _detectionScale) > .001)
        {
            area.RadiusScale = _detectionScale;
        }
        _detectionAreas.Add(area);
    }

    public void UnregisterArea(DetectionArea area)
    {
        _detectionAreas.Remove(area);
    }
    
    public void ChangeDetectionRadiusScale(float newScale)
    {
        _detectionScale = newScale;
        foreach (DetectionArea area in _detectionAreas)
        {
            area.RadiusScale = newScale;
        }
    }
}