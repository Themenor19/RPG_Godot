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