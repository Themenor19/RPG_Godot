using Godot;
using System;
using System.Threading.Tasks;

public partial class InteractionArea : Area2D
{

	[Export] public string ActionName = "interact";
	public bool CanInteract = true;

	public Func<Task> Interact = () => Task.CompletedTask;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
