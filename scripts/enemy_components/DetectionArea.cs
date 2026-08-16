using Godot;
using RPG.scripts.globals;

namespace RPG.scripts.enemy_components;

[Tool]
[GlobalClass]
public partial class DetectionArea : Area2D
{
	private float _radius = 10f;
	private float _radiusScale = 1f;
	private CollisionShape2D _shape;
	
	private EnemyDetectionManager _manager;

	[Export]
	public CollisionShape2D Shape
	{
		get => _shape;
		set
		{
			_shape = value;
			UpdateShape();
		}
	}

	[Export]
	public float Radius
	{
		get => _radius;
		set
		{
			_radius = value;
			UpdateShape();
		}
	}

	[Export]
	public float RadiusScale
	{
		get => _radiusScale;
		set
		{
			_radiusScale = value;
			UpdateShape();
		}
	}

	public override void _Ready()
	{
		if (Radius > 0)
		{
			if (Shape.Shape is CircleShape2D circle)
			{
				circle.Radius = Radius;
			}
		}
		_manager = EnemyDetectionManager.Instance;
		_manager.RegisterArea(this);
	}

	private void UpdateShape()
	{
		if (Radius <= 0) return;
		if (Shape is {Shape: CircleShape2D circle})
		{
			circle.Radius = Radius;
			if (RadiusScale > 0)
			{
				circle.Radius *= RadiusScale;
			}
		}
	}

	public override void _ExitTree()
	{
		_manager.UnregisterArea(this);
		base._ExitTree();
	}
}
