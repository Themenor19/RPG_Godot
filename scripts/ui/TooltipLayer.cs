using Godot;

namespace RPG.scripts.ui;

public partial class TooltipLayer : CanvasLayer
{
	private Control _tooltip;
	private Node _tooltipParent;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void AddTooltip(Control tooltip, Node originalParent)
	{
		ClearTooltip();
		_tooltip = tooltip;
		_tooltipParent = originalParent;
		_tooltip.Reparent(this);
		_tooltip.Visible = true;
	}

	public void ClearTooltip()
	{
		if (_tooltip == null) return;
		_tooltip.Visible = false;
		_tooltip.Reparent(_tooltipParent);
		_tooltipParent = null;
		_tooltip = null;
	}

	public bool HasTooltip()
	{
		return _tooltip != null;
	}
}
