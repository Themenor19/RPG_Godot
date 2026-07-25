using Godot;
using RPG.scenes.projectiles.spells;

namespace RPG.scripts.spell_action_components;

public partial class DiesOnImpact : Node2D
{
    [Export] public Projectile ParentProjectile;

    private void DieOnImpact(Node2D node)
    {
        if (node is )
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        Spell.AreaEntered += DieOnImpact;
    }
}