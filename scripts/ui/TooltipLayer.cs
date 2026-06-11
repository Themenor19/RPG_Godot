using Godot;

namespace RPG.scripts.ui;

public partial class TooltipLayer : CanvasLayer
{
	private Control _tooltip;
	private Node _tooltipParent;
	private Vector2 _originalPosition;
	
	private Control _itemTooltip;
	private Node _itemTooltipParent;
	private Vector2 _itemTooltipOriginalPosition;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		

	}

	public bool IsShowingTooltip(Control tooltip)
	{
		return _tooltip == tooltip;
	}

	public void AddTooltip(Control tooltip, Node originalParent)
	{
		ClearTooltip();
		_tooltip = tooltip;
		_tooltipParent = originalParent;
		_originalPosition = tooltip.Position; 
		_tooltip.Reparent(this);
		_tooltip.Visible = true;
	}

	public void AddItemTooltip(Control tooltip, Node originalParent)
	{
		ClearTooltip();
		ClearItemTooltip();
		_itemTooltip = tooltip;
		_itemTooltipParent = originalParent;
		_originalPosition = tooltip.Position; 
		_tooltip.Reparent(this);
		_tooltip.Visible = true;
	}
	
	public void ClearTooltip()
	{
		if (_tooltip == null) return;
		_tooltip.Visible = false;

		if (_tooltipParent != null && IsInstanceValid(_tooltipParent))
		{
			_tooltip.Reparent(_tooltipParent);
			_tooltip.Position = _originalPosition;
		}
		else if (_tooltip.GetParent() != null)
		{
			// Parent is gone but panel is still in tree — just remove it
			_tooltip.GetParent().RemoveChild(_tooltip);
			// Don't QueueFree — the slot still owns this panel
		}

		_tooltipParent = null;
		_tooltip = null;
		_originalPosition = Vector2.Zero;
	}
	
	public void ClearItemTooltip()
	{
		if (_itemTooltip == null) return;
		_itemTooltip.Visible = false;

		if (_itemTooltip != null && IsInstanceValid(_itemTooltipParent))
		{
			_itemTooltip.Reparent(_itemTooltipParent);
			_itemTooltip.Position = _itemTooltipOriginalPosition;
		}
		else
		{
			_itemTooltip.QueueFree();
		}

		_itemTooltipParent = null;
		_itemTooltip = null;
		_itemTooltipOriginalPosition = Vector2.Zero;
	}

	public bool HasTooltip()
	{
		return _tooltip != null || _itemTooltip != null;
	}
}
