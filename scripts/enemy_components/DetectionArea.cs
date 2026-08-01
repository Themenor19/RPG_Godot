using Godot;

namespace RPG.scripts.enemy_components;

[Tool]
public partial class DetectionArea : Area2D
{
	private float _radius = 10f;
	private CollisionShape2D _shape;

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

	public override void _Ready()
	{
		if (Radius > 0)
		{
			if (Shape.Shape is CircleShape2D circle)
			{
				circle.Radius = Radius;
			}
		}
	}

	private void UpdateShape()
	{
		if (Radius <= 0) return;
		if (Shape.Shape is CircleShape2D circle)
		{
			circle.Radius = Radius;
		}
	}
}
