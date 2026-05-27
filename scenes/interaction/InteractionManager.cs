using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class InteractionManager : Node2D
{
	public static InteractionManager Instance { get; private set; }

	private CharacterBody2D _player;
	private Label _label;
	private static string BASE_TEXT = "[E] to ";

	public List<InteractionArea> ActiveAreas = [];
	public bool CanInteract = true;

	public override void _Ready()
	{
		Instance = this;
		_player = GetTree().GetFirstNodeInGroup("player") as CharacterBody2D;
		_label = GetNode<Label>("Label");	
	}

	public void RegisterArea(InteractionArea area)
	{
		ActiveAreas.Add(area);
	}

	public void UnregisterArea(InteractionArea area)
	{
		try
		{
			var index =  ActiveAreas.IndexOf(area);
			ActiveAreas.RemoveAt(index);
		}
		catch (Exception e)
		{
			// ignored
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	
		if (ActiveAreas.Count > 0 && CanInteract)
		{
			ActiveAreas.Sort(SortByDistanceToPlayer);
			_label.Text = BASE_TEXT + ActiveAreas[0].ActionName;
			_label.GlobalPosition = ActiveAreas[0].GlobalPosition - new Vector2(_label.Size.X / 2, _label.Size.Y + 8);			_label.Show();
		}
		else
		{
			_label.Hide();
		}
	}

	private int SortByDistanceToPlayer(InteractionArea area1, InteractionArea area2)
	{
		var area1ToPlayer = _player.GlobalPosition.DistanceTo(area1.GlobalPosition);
		var area2ToPlayer = _player.GlobalPosition.DistanceTo(area2.GlobalPosition);
		return area1ToPlayer.CompareTo(area2ToPlayer);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("interact"))
		{
			if (ActiveAreas.Count > 0)
			{
				_ = HandleInteraction(); // fire and forget
			}
		}
	}

	private async Task HandleInteraction()
	{
		CanInteract = false;
		_label.Hide();

		if (ActiveAreas[0].Interact != null)
			await ActiveAreas[0].Interact();

		CanInteract = true;
	}
}
