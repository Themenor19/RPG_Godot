using System;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
[Tool]
public partial class InteractionArea : Area2D
{

	[Export] public string ActionName = "interact";
	[Export] public Sprite2D Marker;
	[Export] public bool UseMarker;
	private float _markerX;
	private float _markerY;

	[Export]
	public float MarkerX
	{
		get => _markerX;
		set
		{
			_markerX = value;
			Marker?.Position = new Vector2(_markerX, _markerY);
		}
	}

	[Export]
	public float MarkerY
	{
		get => _markerY;
		set
		{
			_markerY = value;
			Marker?.Position = new Vector2(_markerX, _markerY);
		}
	}
	public bool CanInteract = true;

	public Func<Task> Interact = () => Task.CompletedTask;

	public override void _Ready()
	{
		base._Ready();
		Marker.Visible = Engine.IsEditorHint();
	}
	
	public void _on_body_entered(Node2D body)
	{
		if (CanInteract)
		{
			InteractionManager.Instance.RegisterArea(this);
		}
	}

	public void _on_body_exited(Node2D body)
	{
		if (CanInteract)
		{
			InteractionManager.Instance.UnregisterArea(this);
		}
	}
}
