using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Godot;
using RPG.custom_resources.inventory;
using RPG.scripts.helper_classes;
using RPG.scripts.ui;
using Inventory = RPG.custom_resources.inventory.Inventory;

namespace RPG.scripts.globals;

public enum InventoryToAdd
{
	Either,
	Hotbar,
	Inventory
}

public partial class Global : Node
{
	private static readonly Vector2 BaseSize = new(480f, 270.0f);
	public static Global Instance { get; private set; }
	
	public Level CurrentLevel { get; set; }
	public TileMapLayer PlantingLayer;

	public bool SaveLoaded;
	public Vector2 SavedPlayerPosition;
	
	//Scene and node references
	public PackedScene InventorySlotScene;
	public PackedScene HotbarSlotScene;
	public PackedScene WorldInventoryItemScene;

	
	public Player PlayerNode { get; set; }
	private PackedScene _playerNodeReference;
	public Inventory PlayerInventory;
	//Hotbar Items
	public Inventory HotbarInventory;
	
	public int CoinAmount { get; set; }

	private string _playerSpawnLocation = "";

	[Signal]
	public delegate void GameTickEventHandler(int day, int hour, int minute, float secondsPerInGameMinute);

	[Signal]
	public delegate void PlayerInventoryUpdatedEventHandler(Inventory hotbar, Inventory playerInventory);
	[Signal]
	public delegate void CoinAmountChangedEventHandler(int coinAmount);
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_playerNodeReference = GD.Load<PackedScene>("uid://3t2b0fs1ct22");
		ItemDatabase.LoadItems();
		InventorySlotScene = GD.Load<PackedScene>("res://scenes/ui/inventory/inventory_slot.tscn");
		HotbarSlotScene = GD.Load<PackedScene>("res://scenes/ui/inventory/hotbar_slot.tscn");
		PlayerInventory = GD.Load<Inventory>("res://custom_resources/inventory/player_inventory.tres");
		HotbarInventory = GD.Load<Inventory>("res://custom_resources/inventory/hotbar_inventory.tres");
		WorldInventoryItemScene = GD.Load<PackedScene>("res://scenes/ui/inventory/world_inventory_item.tscn");
		UpdateSize();
		GetTree().GetRoot().SizeChanged += UpdateSize;
		LoadSave();
		Instance = this;
		ProcessMode = ProcessModeEnum.Always;
		ReloadHotbar();
		EmitSignalPlayerInventoryUpdated(HotbarInventory, PlayerInventory);
		PlayerMoveScenes("uid://bibtx3p5das13");
	}

	public void PlayerMoveScenes(string sceneUid, string spawnLocation = "MainSpawn")
	{
		if (DayNightCycle.Instance != null)
		{
			DayNightCycle.Instance.Paused = true;
		}
		_playerSpawnLocation = spawnLocation;
		SceneLoader.Instance.LoadFinished += SceneLoaded;
		SceneLoader.Instance.LoadScene(sceneUid);
	}
	
	public void SceneLoaded(Node newScene)
	{
		if (newScene is Level level)
		{
			PlayerNode ??= _playerNodeReference.Instantiate<Player>();
			var spawnName = string.IsNullOrEmpty(_playerSpawnLocation) ? "MainSpawn" : _playerSpawnLocation;
			level.AddPlayer(PlayerNode, $"{spawnName}");
			if (DayNightCycle.Instance != null)
			{
				DayNightCycle.Instance.Paused = false;
			}
		}
		SceneLoader.Instance.LoadFinished -= SceneLoaded;
	}

	public void UpdateSize()
	{
		Vector2 sz = DisplayServer.WindowGetSize();
		var ratio = Math.Min(sz.X/BaseSize.X, sz.Y/BaseSize.Y);
		ratio = (float)Math.Max(1f, Math.Floor(ratio));
		GetWindow().ContentScaleFactor = ratio;
	}

	public void Save(Vector2 pos)
	{
		PlayerSaveData saveData = new()
		{
			PlayerPosition = PlayerSaveData._vec2_to_dict(pos),
			Gold = CoinAmount,

			InventoryItems = PlayerInventory.Items
				.Where(i => i != null)
				.Select(i => new InventoryItemSaveData
				{
					ItemId = i.Item.Id,
					Quantity = i.Quantity,
				})
				.ToList()
		};

		string json = JsonSerializer.Serialize(saveData, new JsonSerializerOptions
		{
			WriteIndented = true
		});

		Directory.CreateDirectory("saves");
		File.WriteAllText("saves/player_data.json", json);
	}

	public void LoadSave()
	{
		try
		{
			if (!File.Exists("saves/player_data.json"))
			{
				throw new FileNotFoundException("saves/player_data.json file not found");
			}

			string json = File.ReadAllText("saves/player_data.json");

			PlayerSaveData? playerSaveData =
				JsonSerializer.Deserialize<PlayerSaveData>(json);

			if (playerSaveData == null)
			{
				throw new Exception("Failed to deserialize save file");
			}

			SavedPlayerPosition =
				PlayerSaveData._dic_to_vec2(playerSaveData.PlayerPosition);
			CoinAmount = playerSaveData.Gold;

			// Clear inventory first
			for (int i = 0; i < PlayerInventory.Items.Count; i++)
			{
				PlayerInventory.Items[i] = null;
			}

			// Rebuild inventory
			for (int i = 0;
				 i < playerSaveData.InventoryItems.Count &&
				 i < PlayerInventory.Items.Count;
				 i++)
			{
				InventoryItemSaveData savedItem =
					playerSaveData.InventoryItems[i];

				InventoryItem? item =
					ItemDatabase.GetItemById(savedItem.ItemId);

				if (item == null)
				{
					GD.PrintErr($"Could not find item ID {savedItem.ItemId}");
					continue;
				}

				InventoryItem loadedItem =
					(InventoryItem)item.Duplicate();

				InventoryItemSlot slot = new InventoryItemSlot
				{
					Item = loadedItem,
					Quantity = savedItem.Quantity
				};
					
				PlayerInventory.Items[i] = slot;
			}

			ReloadHotbar();
			SaveLoaded = true;
		}
		catch (Exception e)
		{
			GD.Print($"Failed to load save: {e}");

			SavedPlayerPosition = Vector2.Zero;
			SaveLoaded = false;
		}
	}

	
	public override void _ExitTree()
	{
		base._ExitTree();
		GetTree().GetRoot().SizeChanged -= UpdateSize;
		Instance = null;
	}

	public void _on_time_tick(int day, int hour, int minute, float secondsPerIngameMinute)
	{
		EmitSignal(SignalName.GameTick, day, hour, minute, secondsPerIngameMinute);
	}

	public bool AddItemToPlayer(InventoryItemSlot item, InventoryToAdd addToInventory = InventoryToAdd.Either)
	{
		var itemAdded = false;

		switch (addToInventory)
		{
			case InventoryToAdd.Hotbar:
				itemAdded = AddItemToInventory(HotbarInventory,  item);
				break;
			case  InventoryToAdd.Inventory:
				itemAdded = AddItemToInventory(PlayerInventory, item);
				break;
			default:
				itemAdded = AddItemToInventory(HotbarInventory,  item);
				if (!itemAdded)
				{
					itemAdded = AddItemToInventory(PlayerInventory, item);
				}
				break;
		}
		
		ReloadHotbar();
		if (itemAdded)
		{
			CallDeferred(nameof(EmitInventoryUpdated));
		}
		return itemAdded;
	}

	public bool AddItemToInventory(Inventory inventory, InventoryItemSlot item)
	{
		int emptySpace = -1;
		if (inventory == null) return false;
		if (item.Item.Type is ItemTypes.Coin)
		{
			CoinAmount  += item.Item.Value;
			CallDeferred(nameof(EmitCoinChanged));
			return true;
		}
		for (int i = 0; i < inventory.Items.Count; i++)
		{
			if (inventory.Items[i] == null)
			{
				if (emptySpace == -1) emptySpace = i;
				continue;
			}

			if (inventory.Items[i].Item.Name != item.Item.Name || 
				inventory.Items[i].Item.Effect != item.Item.Effect ||
				inventory.Items[i].Item.Type != item.Item.Type ||
				inventory.Items[i].Item.Value != item.Item.Value||
				inventory.Items[i].Item.ItemScene != item.Item.ItemScene) continue; // add if needed
			
			inventory.Items[i].Quantity += item.Quantity;
			
			return true;
		}

		if (emptySpace == -1) return false;
	
		inventory.Items[emptySpace] = new InventoryItemSlot
		{
			Item = item.Item,
			Quantity = item.Quantity,
		};
		return true;
	}

	public void SwapItems(int index1, int index2)
	{
		if (index1 == index2)
		{
			return;
		}
		(PlayerInventory.Items[index1], PlayerInventory.Items[index2]) = (PlayerInventory.Items[index2], PlayerInventory.Items[index1]);
		ReloadHotbar();
		EmitInventoryUpdated();
	}

	private void EmitInventoryUpdated()
	{
		EmitSignal(SignalName.PlayerInventoryUpdated, HotbarInventory, PlayerInventory);
	}

	private void EmitCoinChanged()
	{
		EmitSignal(SignalName.CoinAmountChanged, CoinAmount);
	}

	public void RemoveItem(InventoryItemSlot item, int slotIndex, int dropAmount)
	{
		if (item == null || PlayerInventory.Items.Count < slotIndex+1 || slotIndex < 0) return;
		if (item == PlayerInventory.Items[slotIndex])
		{
			if (item.Quantity <= 1)
			{
				PlayerInventory.Items[slotIndex] = null;
			}
			else
			{
				PlayerInventory.Items[slotIndex].Quantity -= dropAmount;
				if (PlayerInventory.Items[slotIndex].Quantity <= 0)
				{
					PlayerInventory.Items[slotIndex] = null;
				}
			}
		}
		ReloadHotbar();
		CallDeferred(nameof(EmitInventoryUpdated));
	}

	public void ReloadHotbar()
	{
		for (int i = 0; i < HotbarInventory.Items.Count; i++)
		{
			if (i < PlayerInventory.Items.Count)
			{
				HotbarInventory.Items[i] = PlayerInventory.Items[i];
			}
			else
			{
				HotbarInventory.Items[i] = null;
			}
		}
	}
	
	public void RemoveHotbarItem(InventoryItemSlot item, int slotIndex)
	{
		if (item == null || HotbarInventory.Items.Count < slotIndex+1 || slotIndex < 0) return;
		if (item == HotbarInventory.Items[slotIndex])
		{
			HotbarInventory.Items[slotIndex] = null;
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
				GD.RandRange(-radius, radius),
				GD.RandRange(-radius, radius)
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

	public void DropItem(InventoryItemSlot itemData, int slotIndex, Vector2 dropPosition, int dropAmount)
	{
		var itemInstance = WorldInventoryItemScene.Instantiate<WorldInventoryItem>();
		itemInstance.ItemResource = (InventoryItemSlot)itemData.Duplicate();
		itemInstance.ItemResource.Quantity = dropAmount;
		// Pass world position directly, don't add PlayerNode.GlobalPosition after adjusting
		var worldDropPosition = dropPosition + PlayerNode.GlobalPosition;
		itemInstance.GlobalPosition = AdjustDropPosition(worldDropPosition);
		GetTree().CurrentScene.AddChild(itemInstance);
		RemoveItem(itemData, slotIndex, dropAmount);
	}
	
	public Vector2 GetDropOffset(float offset)
	{
		var playerDirection = PlayerNode.Direction;
		if (playerDirection == LookDirection.North)
		{
			return Vector2.Up*offset;
		}

		if (playerDirection == LookDirection.South)
		{
			return Vector2.Down*offset;
		}

		if (playerDirection == LookDirection.East)
		{
			return Vector2.Right * offset;
		}

		if (playerDirection == LookDirection.West)
		{
			return Vector2.Left * offset;
		}
		
		return Vector2.Zero;
	}

	public void UseItem(InventoryItemSlot itemData, int slotIndex)
	{
		if (itemData == null || PlayerNode == null || slotIndex < 0)
			return;

		bool isUsable = itemData.Item.Effect != ItemEffects.None &&
						(itemData.Item.Type is ItemTypes.Consumable or ItemTypes.Spell);

		if (!isUsable) return;

		if (PlayerNode.ApplyItemEffect(itemData.Item))
		{
			if (itemData.Quantity > 1)
			{
				PlayerInventory.Items[slotIndex].Quantity--;
				CallDeferred(nameof(EmitInventoryUpdated));
			}
			else
			{
				RemoveItem(itemData, slotIndex, 1);
			}
		}
	}
}
