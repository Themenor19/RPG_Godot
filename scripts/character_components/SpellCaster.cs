using System;
using Godot;

namespace RPG.scripts.character_components;

public partial class SpellCaster: Node2D
{
    public bool CastSpell(InventoryItem item, float speed, Vector2 target, Node spellHolder, Vector2 spellCastPosition, HitBox parentHitBox)
    {
        var spell = item.ItemScene.Instantiate<BaseSpellItem>();

        spell.SpellSpeed = speed;
        spell.GlobalPosition = spellCastPosition;

        spellHolder.AddChild(spell);

        var velocity = spell.GlobalPosition.DirectionTo(target).Normalized();
        var angle = velocity.Angle() - MathF.PI / 2f; // <-- use 'velocity', not 'spell.Velocity'

        spell.Cast(velocity, angle, item.Damage, parentHitBox);
        return true;
    }
}