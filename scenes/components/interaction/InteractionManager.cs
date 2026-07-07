using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class InteractionManager : Node2D
{
	public static InteractionManager Instance { get; private set; }

	private CharacterBody2D _player;
	private Label _label;
	private static readonly string BaseText = "[E] to ";

	private List<InteractionArea> _activeAreas = [];
	private bool _canInteract = true;

	public override void _Ready()
	{
		Instance = this;
		_player = GetTree().GetFirstNodeInGroup("player") as CharacterBody2D;
		_label = GetNode<Label>("Label");	
		_label.Visible = false;
	}

	public void RegisterArea(InteractionArea area)
	{
		_activeAreas.Add(area);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (!IsInstanceValid(_player))
		{
			_player = GetTree().GetFirstNodeInGroup("player") as CharacterBody2D;
			if (_player == null) return;
		}
		
		// Build a clean snapshot — only valid areas, no mutation during sort
		var validAreas = _activeAreas
			.Where(IsInstanceValid)
			.OrderBy(a => _player.GlobalPosition.DistanceSquaredTo(a.GlobalPosition))
			.ToList();

		// Sync back so stale entries get cleaned up
		_activeAreas = validAreas;

		if (validAreas.Count > 0 && _canInteract)
		{
			var closest = validAreas[0];
			_label.Text = BaseText + closest.ActionName;
			_label.GlobalPosition = closest.GlobalPosition - new Vector2(_label.Size.X / 2, _label.Size.Y + 8);
			_label.Show();
		}
		else
		{
			_label.Hide();
		}
	}

	private int SortByDistanceToPlayer(InteractionArea a, InteractionArea b)
	{
		if (!IsInstanceValid(a)) return 1;
		if (!IsInstanceValid(b)) return -1;

		return _player.GlobalPosition.DistanceSquaredTo(a.GlobalPosition)
			.CompareTo(_player.GlobalPosition.DistanceSquaredTo(b.GlobalPosition));
	}

	public void UnregisterArea(InteractionArea area)
	{
		_activeAreas.Remove(area); // Remove returns false if not found, no exception needed
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("interact"))
		{
			if (_activeAreas.Count > 0)
			{
				_ = HandleInteraction(); // fire and forget
			}
		}
	}

	private async Task HandleInteraction()
	{
		if (_activeAreas.Count == 0) return;
	
		var area = _activeAreas[0]; // capture before await
	
		_canInteract = false;
		_label.Hide();

		if (IsInstanceValid(area) && area.Interact != null)
			await area.Interact();

		_canInteract = true;
	}
}
