using System.Collections.Generic;
using Godot;

namespace RPG.scripts.helper_classes;

public static class ItemDatabase
{
    private static readonly Dictionary<int, InventoryItem> Items = new();

    public static void LoadItems()
    {
        LoadItemsFromDirectory("res://custom_resources/items/");
    }

    private static void LoadItemsFromDirectory(string path)
    {
        var dir = DirAccess.Open(path);
        if (dir == null) return;

        dir.ListDirBegin();
        string file = dir.GetNext();

        while (!string.IsNullOrEmpty(file))
        {
            if (file == "." || file == "..")
            {
                file = dir.GetNext();
                continue;
            }

            string fullPath = $"{path}{file}";

            if (dir.CurrentIsDir())
            {
                LoadItemsFromDirectory($"{fullPath}/");
            }
            else if (file.EndsWith(".tres"))
            {
                InventoryItem item = GD.Load<InventoryItem>(fullPath);
                Items[item.Id] = item;
            }

            file = dir.GetNext();
        }

        dir.ListDirEnd();
    }

    public static InventoryItem? GetItemById(int id)
    {
        return Items.TryGetValue(id, out var item)
            ? item
            : null;
    }
}