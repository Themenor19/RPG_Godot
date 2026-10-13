using System;
using System.Threading.Tasks;
using Godot;

namespace RPG.scripts.building.doors;

[Tool]
public partial class DoorBase : Building
{
	
	[Export] public AnimatedSprite2D Sprite;
	[Export] public CollisionShape2D CollisionShape2D;
	[Export] public InteractionArea InteractionArea;
	[Export] public AnimationPlayer AnimationPlayer;

	private bool _doorOpened;
	[Export]
	public bool DoorOpened
	{
		get => _doorOpened;
		set
		{
			_doorOpened = value;
			HandleRotation();
		}
	}

	public override void HandleRotation()
	{
		if (AnimationPlayer == null || !IsNodeReady()) return;
		var openedString = DoorOpened ? "opened" : "closed";
		AnimationPlayer.Play($"{openedString}_{Rotation.ToString()}");
	}

	public override void _Ready()
	{
		base._Ready();
		HandleRotation();
		InitInteraction();
	}

	private void InitInteraction()
	{
		InteractionArea.Interact = () =>
		{
			try
			{
				DoorOpened = !DoorOpened;
				return Task.CompletedTask;
			}
			catch (Exception e)
			{
				return Task.FromException(e);
			}
		};
	}
	
	
}
