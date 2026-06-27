using System.Collections.Generic;
using Godot;

namespace RPG.scripts.helper_classes;

public class PlayerSaveData
{
	public Dictionary<string, float> PlayerPosition { get; set; } = new();
	public int Gold { get; set; }
	public List<InventoryItemSaveData> InventoryItems { get; set; } = [];

	public static Vector2 _dic_to_vec2(Dictionary<string, float> dict)
	{
		return new Vector2(dict["x"], dict["y"]);
	}
	
	public static Dictionary<string, float> _vec2_to_dict(Vector2 vec)
	{
		return new Dictionary<string, float>
		{
			{ "x", vec.X },
			{ "y", vec.Y }
		};
	}
}

public class InventoryItemSaveData
{
	public int ItemId { get; set; }
	public int Quantity { get; set; }
}
