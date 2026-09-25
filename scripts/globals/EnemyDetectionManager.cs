using System;
using System.Collections.Generic;
using Godot;
using RPG.scripts.enemy_components;

namespace RPG.scripts.globals;

public partial class EnemyDetectionManager : Node2D
{

	private readonly List<DetectionAreaListItem> _detectionAreas = [];
	private float _detectionScale;

	[Export]
	public float DetectionScale
	{
		get => _detectionScale;
		set
		{
			_detectionScale = value;
			ChangeDetectionRadiusScale(value);
		}
	}

	//Adjust scale of an individual detection area, while still being bound by the overall detection scale (so if an individual wants to be 2x it's normal radius, but the overall detection scale is .5, the items scale will be 1.0)
	public void AdjustIndividualScale(float newScale, DetectionArea area)
	{
		var item = _detectionAreas.Find(x => x.DetectionArea == area);
		if (item == null) return;
		item.AdjustedIndividualScale = newScale;
		item.DetectionArea.RadiusScale = newScale * DetectionScale;
	}

	public void RegisterArea(DetectionArea area)
	{
		var item = new DetectionAreaListItem
		{
			DetectionArea = area,
			BaseScale = area.RadiusScale,
			AdjustedIndividualScale = area.RadiusScale
		};

		if (Math.Abs(item.DetectionArea.RadiusScale - _detectionScale * item.AdjustedIndividualScale) > .001)
		{
			area.RadiusScale = _detectionScale * item.AdjustedIndividualScale;
		}

		_detectionAreas.Add(item);
	}

	public void UnregisterArea(DetectionArea area)
	{
		var item = _detectionAreas.Find(x => x.DetectionArea == area);
		item.DetectionArea.RadiusScale = item.BaseScale;
		_detectionAreas.Remove(item);
	}

	//changes the overall scale of every enemy detection area, meaning that if I change it to 2.0, every detection area will be 2x larger than what their individual detection scale/radius is normally
	private void ChangeDetectionRadiusScale(float newScale)
	{
		GD.Print("Change detection radius scale");
		foreach (DetectionAreaListItem item in _detectionAreas)
		{
			GD.Print(
				$"radius scale before: {item.DetectionArea.RadiusScale} and radius before: {item.DetectionArea.Radius}");
			item.DetectionArea.RadiusScale = newScale * item.AdjustedIndividualScale;
			GD.Print(
				$"radius scale after: {item.DetectionArea.RadiusScale} and radius after: {item.DetectionArea.Radius}");
		}
	}
}

//stores the area, it's original scale used to revert when releasing/unregistering the item, as well as its own individual scale that is used while registered along with the overarching scale. 
public class DetectionAreaListItem
{
	public DetectionArea DetectionArea { get; set; }
	public float BaseScale { get; set; }
	public float AdjustedIndividualScale { get; set; }
}
