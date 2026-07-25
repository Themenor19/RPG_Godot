using Godot;

namespace RPG.scripts.projectile_action_components;

public partial class DiesOnImpact : Node2D
{
	[Export] public Projectile Projectile;
	[Export] public Node2D Parent;

	private void DieOnImpact(Rid bodyRid, Node2D body, int bodyShapeIndex, int localShapeIndex)
	{
		if (body is TileMapLayer tileMapLayer)
		{
			if (tileMapLayer.GetGroups().Contains("walls"))
			{
				if (IsInstanceValid(Parent) && !Parent.IsQueuedForDeletion())
				{
					Parent.QueueFree();
				}
			}
		}
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		Projectile.ProjectileBodyShapeEntered += DieOnImpact;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		Projectile.ProjectileBodyShapeEntered -= DieOnImpact;
	}
}
