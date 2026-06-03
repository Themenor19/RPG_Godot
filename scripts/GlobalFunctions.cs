using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Schema;
using Godot;
using RPG.scripts.helper_classes;
using System.Text.Json;
using System.Threading.Tasks;
using RPG.scenes.ui.inventory;

namespace RPG.scripts;

public partial class GlobalFunctions : Node
{
	private static readonly Vector2 BaseSize = new(480f, 270.0f);
	public static GlobalFunctions Instance { get; private set; }
	public static Dictionary<string, PackedScene> Spells = new();

	public bool SaveLoaded;
	public Vector2 SavedPlayerPosition;

	public PackedScene InventorySlotScene;
	
	public Node PlayerNode { get; set; }
	public Inventory PlayerInventory;

	[Signal]
	public delegate void GameTickEventHandler(int day, int hour, int minute, float secondsPerIngameMinute);
	[Signal]
	public delegate void PlayerInventoryUpdatedEventHandler(Inventory inventory);
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InventorySlotScene = GD.Load<PackedScene>("res://scenes/ui/inventory/inventory_slot.tscn");
		PlayerInventory = GD.Load<Inventory>("res://scenes/ui/inventory/inventories/player_inventory.tres");
		UpdateSize();
		GetTree().GetRoot().SizeChanged += UpdateSize;
		LoadSave();
		InstantiateSpells();
		Instance = this;
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
					Name = item.Name,
					Effect = item.Effect,
					Type = item.Type,
					Icon = item.Icon,
					Quantity = item.Quantity,
				};
				EmitSignal(SignalName.PlayerInventoryUpdated, PlayerInventory);
				return true;
			}
			
				if (PlayerInventory.Items[i].Name == item.Name && PlayerInventory.Items[i].Effect == item.Effect && PlayerInventory.Items[i].Type == item.Type)
			{
				PlayerInventory.Items[i].Quantity += item.Quantity;
				EmitSignal(SignalName.PlayerInventoryUpdated, PlayerInventory);
				return true;
			}
		}
		return false;
	}

	public void RemoveItem(ItemData item)
	{
		EmitSignal(SignalName.PlayerInventoryUpdated, PlayerInventory);
	}

	public void IncreaseInventorySize()
	{
		EmitSignal(SignalName.PlayerInventoryUpdated, PlayerInventory);
	}
}
