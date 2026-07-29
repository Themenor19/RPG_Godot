using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace RPG.scripts.helper_classes;

/// <summary>
/// Stores all the needed player data and saves and loads them using Godot's binary system
/// </summary>
public class PlayerData
{
    private const string KeyPlayerPosition = "PlayerPosition";
    private const string KeyCurrentHealth = "CurrentHealth";
    private const string KeyMaxHealth = "MaxHealth";
    private const string KeyCurrentGold = "CurrentGold";
    private const string KeyInventory = "Inventory";
    private const string KeyStartingTime = "StartingTime";

    public Vector2 PlayerPosition { get; set; }
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }
    public int CurrentGold { get; set; }
    public List<InventoryItemSaveData> Inventory { get; set; } = new();
    public float StartingTime { get; set; }

    public bool Save(string savePath)
    {
        Dictionary saveData = DataToSave();

        var err = FileHandler.StoreBinaryFile(saveData, savePath, true);
        if (err != Error.Ok)
        {
            GD.PrintErr("Could not save player data binary: " + err);
        }

        return err == Error.Ok;
    }

    public bool Load(string savePath)
    {
        var saveData = new Dictionary();
        var err = FileHandler.OpenBinaryFile(savePath, saveData);
        if (err != Error.Ok)
        {
            GD.PrintErr("Could not load player data binary: " + err);
        }

        err = DataFromSave(saveData);
        if (err != Error.Ok)
        {
            GD.PrintErr("Invalid save data binary: " + err);
        }

        return err == Error.Ok;
    }

    private Dictionary DataToSave()
    {
        Array<Dictionary> inventory = InventoryToDictionary();

        return new Dictionary
        {
            { KeyPlayerPosition, PlayerPosition },
            { KeyCurrentHealth, CurrentHealth },
            { KeyMaxHealth, MaxHealth },
            { KeyCurrentGold, CurrentGold },
            { KeyInventory, inventory },
            { KeyStartingTime, StartingTime }
        };
    }

    private Error DataFromSave(Dictionary data)
    {
        var err = VerifySave(data);
        if (err != Error.Ok)
        {
            return err;
        }

        try
        {
            PlayerPosition = (Vector2)data[KeyPlayerPosition];
            CurrentHealth = (int)data[KeyCurrentHealth];
            MaxHealth = (int)data[KeyMaxHealth];
            CurrentGold = (int)data[KeyCurrentGold];
            StartingTime = (float)data[KeyStartingTime];
            Array<Dictionary> inventory = (Array<Dictionary>)data[KeyInventory];
            DictionaryToInventory(inventory);

            return Error.Ok;
        }
        catch
        {
            return Error.ParseError;
        }
    }

    private Array<Dictionary> InventoryToDictionary()
    {
        var inventory = new Array<Dictionary>();
        foreach (var item in Inventory)
        {
            Dictionary temp = new Dictionary
            {
                { "ItemId", item.ItemId },
                { "Quantity", item.Quantity }
            };
            inventory.Add(temp);
        }

        return inventory;
    }

    private void DictionaryToInventory(Array<Dictionary> data)
    {
        Inventory.Clear();
        foreach (var item in data)
        {
            var temp = new InventoryItemSaveData
            {
                ItemId = (int)item["ItemId"],
                Quantity = (int)item["Quantity"]
            };
            Inventory.Add(temp);
        }
    }

    private Error VerifySave(Dictionary data)
    {
        if (!data.ContainsKey("CurrentHealth") || !data.ContainsKey("MaxHealth") || !data.ContainsKey("CurrentGold") ||
            !data.ContainsKey("Inventory") || !data.ContainsKey("StartingTime") || !data.ContainsKey("PlayerPosition"))
        {
            return Error.InvalidData;
        }

        return Error.Ok;
    }
}

public class InventoryItemSaveData
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
}