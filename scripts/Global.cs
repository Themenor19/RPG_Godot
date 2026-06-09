using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Schema;
using Godot;
using RPG.scripts.helper_classes;
using System.Text.Json;
using RPG.scenes.ui.inventory;
using RPG.scripts.ui;
using Inventory = RPG.custom_resources.inventory.Inventory;

namespace RPG.scripts;

public partial class Global : Node
{
	private static readonly Vector2 BaseSize = new(480f, 270.0f);
	public static Global Instance { get; private set; }
	public static Dictionary<string, PackedScene> Spells = new();
	
	public Level CurrentLevel { get; set; }

	public bool SaveLoaded;
	public Vector2 SavedPlayerPosition;

	public PackedScene InventorySlotScene;
	public PackedScene WorldInventoryItemScene;

	
	public Player PlayerNode { get; set; }
	public Inventory PlayerInventory;

	[Signal]
	public delegate void GameTickEventHandler(int day, int hour, int minute, float secondsPerIngameMinute);
	[Signal]
	public delegate void PlayerInventoryUpdatedEventHandler(Inventory inventory);
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InventorySlotScene = GD.Load<PackedScene>("res://scenes/ui/inventory/inventory_slot.tscn");
		PlayerInventory = GD.Load<Inventory>("res://custom_resources/inventory/player_inventory.tres");
		WorldInventoryItemScene = GD.Load<PackedScene>("res://scenes/ui/inventory/world_inventory_item.tscn");
		UpdateSize();
		GetTree().GetRoot().SizeChanged += UpdateSize;
		LoadSave();
		InstantiateSpells();
		Instance = this;
		ProcessMode = ProcessModeEnum.Always;
	}

	public void UpdateSize()
	{
		Vector2 sz = DisplayServer.WindowGetSize();
		float ratio = Math.Min(sz.X/BaseSize.X, sz.Y/BaseSize.Y);
		ratio = (float)Math.Max(1f, Math.Floor(ratio));
		GetWindow().ContentScaleFactor = ratio;
	}

	public void Save(Vector2 pos)
	{
		
		PlayerSaveData playerSaveData = new PlayerSaveData
		{
			PlayerPosition = PlayerSaveData._vec2_to_dict(pos)
		};
		
		string json = JsonSerializer.Serialize(playerSaveData);
		Directory.CreateDirectory("saves");
		File.WriteAllTextAsync("saves/player_data.json", json);
	}

	public void LoadSave()
	{
		try
		{
			if (File.Exists("saves/player_data.json"))
			{
				var json = File.ReadAllText("saves/player_data.json");
				PlayerSaveData playerSaveData = JsonSerializer.Deserialize<PlayerSaveData>(json);
				SavedPlayerPosition = PlayerSaveData._dic_to_vec2(playerSaveData.PlayerPosition);
				SaveLoaded = true;
			}
			else
			{
				throw new FileNotFoundException("saves/player_data.json file not found");
			}

		}
		catch (Exception e)
		{
			GD.Print(e.Message);	
			SavedPlayerPosition = new Vector2();
		}
	}

	public void InstantiateSpells()
	{
		List<PackedScene> spells =
		[
			GD.Load<PackedScene>("res://scenes/projectiles/spells/fire.tscn"),
			GD.Load<PackedScene>("res://scenes/projectiles/spells/necro.tscn")
		];

		foreach (PackedScene spell in spells)
		{
			string name = Path.GetFileNameWithoutExtension(spell.ResourcePath);
			Spells.Add(name ?? Spells.Count.ToString(), spell);
		}
	}
	public override void _ExitTree()
	{
		base._ExitTree();
		GetTree().GetRoot().SizeChanged -= UpdateSize;
		Spells.Clear(); // Just clear the dict, don't Dispose
		Instance = null;
	}

	public override void _Process(double delta)
	{
	}

	public void _on_time_tick(int day, int hour, int minute, float secondsPerIngameMinute)
	{
		EmitSignal(SignalName.GameTick, day, hour, minute, secondsPerIngameMinute);
	}

	public bool AddItem(InventoryItem item)
	{
		if (PlayerInventory == null) return false;
		for (int i = 0; i < PlayerInventory.Items.Length; i++)
		{
			if (PlayerInventory.Items[i] == null)
			{
				PlayerInventory.Items[i] = new InventoryItem
				{
					Id = item.Id,
					Name = item.Name,
					Effect = item.Effect,
					Types = item.Types,
					Icon = item.Icon,
					Quantity = item.Quantity,
					HealAmount = item.HealAmount,
					Damage = item.Damage,
					ToolType = item.ToolType
				};
				CallDeferred(nameof(EmitInventoryUpdated));
				return true;
			}

			if (PlayerInventory.Items[i].Name == item.Name && PlayerInventory.Items[i].Effect == item.Effect &&
				PlayerInventory.Items[i].Types == item.Types)
			{
				PlayerInventory.Items[i].Quantity += item.Quantity;
				CallDeferred(nameof(EmitInventoryUpdated));
				return true;
			}
		}

		return false;
	}

	private void EmitInventoryUpdated()
	{
		EmitSignal(SignalName.PlayerInventoryUpdated, PlayerInventory);
	}

	public void RemoveItem(InventoryItem item, int slotIndex)
	{
		if (item == null || PlayerInventory.Items.Length < slotIndex+1 || slotIndex < 0) return;
		if (item == PlayerInventory.Items[slotIndex])
		{
			PlayerInventory.Items[slotIndex] = null;
		}
		CallDeferred(nameof(EmitInventoryUpdated));
	}
	public void IncreaseInventorySize()

	{
		// ... resize logic ...
		CallDeferred(nameof(EmitInventoryUpdated));
	}

	public Vector2 AdjustDropPosition(Vector2 position)
	{
		var radius = 15;
		var items = GetTree().GetNodesInGroup("items");
		var finalPosition = position;
		int maxAttempts = 10;

		for (int attempt = 0; attempt < maxAttempts; attempt++)
		{
			var randomOffset = new Vector2(
				(float)GD.RandRange(-radius, radius),
				(float)GD.RandRange(-radius, radius)
			);
			finalPosition = position + randomOffset;

			bool tooClose = false;
			foreach (var item in items)
			{
				if (item is Node2D item2D && item2D.GlobalPosition.DistanceTo(finalPosition) < 20f)
				{
					tooClose = true;
					break;
				}
			}

			if (!tooClose) break;
		}

		return finalPosition;
	}

	public void DropItem(InventoryItem itemData, int slotIndex, Vector2 dropPosition)
	{
		var itemInstance = WorldInventoryItemScene.Instantiate<WorldInventoryItem>();
		itemInstance.ItemResource = itemData;
		// Pass world position directly, don't add PlayerNode.GlobalPosition after adjusting
		var worldDropPosition = dropPosition + PlayerNode.GlobalPosition;
		itemInstance.GlobalPosition = AdjustDropPosition(worldDropPosition);
		GetTree().CurrentScene.AddChild(itemInstance);
		RemoveItem(itemData, slotIndex);
	}

	public void UseItem(InventoryItem itemData, int slotIndex)
	{
		if (PlayerNode == null || slotIndex < 0 || itemData == null || itemData.Effect == ItemEffects.None || !itemData.Types.HasFlag(InventoryItem.ItemTypes.Spell) || !itemData.Types.HasFlag(ItemTypes.Consumable)) return;
		if (PlayerNode.ApplyItemEffect(itemData))
		{
			RemoveItem(itemData, slotIndex);
		}
		
	}
}
