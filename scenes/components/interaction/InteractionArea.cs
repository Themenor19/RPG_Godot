using System;
using System.Threading.Tasks;
using Godot;

public partial class InteractionArea : Area2D
{

	[Export] public string ActionName = "interact";
	public bool CanInteract = true;

	public Func<Task> Interact = () => Task.CompletedTask;

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
