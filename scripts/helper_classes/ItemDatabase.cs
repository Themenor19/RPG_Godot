using System.Collections.Generic;
using Godot;

namespace RPG.scripts.helper_classes;

public static class ItemDatabase
{
	private static readonly Dictionary<int, string> ItemPaths = new();
	private static readonly Dictionary<int, InventoryItem> ItemCache = new();

	public static void LoadItems()
	{
		ItemPaths.Clear();
		ItemCache.Clear();
		IndexDirectory("res://custom_resources/items/");
	}

	private static void IndexDirectory(string path)
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
				IndexDirectory($"{fullPath}/");
			}
			else if (file.EndsWith(".tres") || file.EndsWith(".tres.remap"))
			{
				string loadPath = file.EndsWith(".remap")
					? fullPath.Substring(0, fullPath.Length - ".remap".Length)
					: fullPath;

				// Load once just to read the Id, then cache the path for later
				InventoryItem item = GD.Load<InventoryItem>(loadPath);
				if (item != null)
				{
					ItemPaths[item.Id] = loadPath;
				}
				else
				{
					GD.PrintErr($"Failed to load item at {loadPath}");
				}
			}

			file = dir.GetNext();
		}

		dir.ListDirEnd();
	}

	public static InventoryItem? GetItemById(int id)
	{
		if (ItemCache.TryGetValue(id, out var cached))
			return cached;

		if (!ItemPaths.TryGetValue(id, out var path))
			return null;

		var item = GD.Load<InventoryItem>(path);
		if (item != null)
			ItemCache[id] = item;

		return item;
	}
}
