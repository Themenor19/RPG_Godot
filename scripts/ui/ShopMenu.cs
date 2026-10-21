using System;
using Godot;
using Godot.Collections;
using RPG.custom_resources.inventory;

namespace RPG.scripts.ui;

public partial class ShopMenu : Control
{
	[Export] public PackedScene SlotPrefab;
	[Export] public GridContainer GridContainer;
	[Export] public int NumberOfSlotsPerRow;
	[Export] public Label Pages;

	public Action<ShopInventoryItemSlot> OnItemSelected;

	private ShopInventory _inventory;
	private int _numPages;
	
	private int _currentPage;

	public int CurrentPage
	{
		get => _currentPage;
		set
		{
			_currentPage = value;
			SetCurrentPage();
		}
	}

	private Array<ShopSlot> _slots = [];

	public void Init(ShopInventory inventory)
	{
		if (inventory == null)
		{
			GD.PrintErr("ShopMenu init failed: Inventory is null");
			return;
		}

		_inventory = inventory;
		SetPages();
	}

	public override void _Ready()
	{
		GridContainer.Columns = NumberOfSlotsPerRow;
		foreach (var child in GridContainer.GetChildren())
		{
			GridContainer.RemoveChild(child);
		}

		for (int i = 0; i < NumberOfSlotsPerRow; i++)
		{
			var slot = SlotPrefab.Instantiate<ShopSlot>();
			GridContainer.AddChild(slot);
			_slots.Add(slot);
			slot.ItemClicked += SlotClicked;
		}
	}

	private void SetPages()
	{
		if (_inventory == null) return;
		_numPages = _inventory.Items.Count / NumberOfSlotsPerRow;
		if (_inventory.Items.Count % NumberOfSlotsPerRow != 0) _numPages++;

		CurrentPage = 0;
	}

	private void SetCurrentPage()
	{
		
		for (int i = Math.Max(0, CurrentPage * NumberOfSlotsPerRow); i < NumberOfSlotsPerRow * (CurrentPage+1); i++)
		{
			if (_inventory.Items.Count < i + 1)
			{
				_slots[i%NumberOfSlotsPerRow].SetEmpty();
			}
			else
			{
				_slots[i%NumberOfSlotsPerRow].SetItem(_inventory.Items[i], i);
			}
		}
		
		Pages.Text = $"Page {CurrentPage+1} / {_numPages}";
	}

	public void Next()
	{
		CurrentPage = (CurrentPage + 1) % _numPages;
	}

	public void Prev()
	{
		var currentPage = (CurrentPage - 1) % _numPages;
		if (currentPage < 0) currentPage = _numPages -1;
		CurrentPage = currentPage;
	}

	private void SlotClicked(ShopInventoryItemSlot slot)
	{
		OnItemSelected?.Invoke(slot);
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		foreach (var slot in _slots)
		{
			slot.ItemClicked -= SlotClicked;
		}
	}
}
