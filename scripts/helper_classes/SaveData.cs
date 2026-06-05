using System.Collections.Generic;
using System.Runtime.InteropServices;
using Godot;
using RPG.scenes.ui.inventory;
using Inventory = RPG.custom_resources.inventory.Inventory;

namespace RPG.scripts.helper_classes;

public class PlayerSaveData
{
	public Dictionary<string, float> PlayerPosition { get; set; } = new();
	public Inventory PlayerInventory { get; set; } = new();

	public static Dictionary<string, float> _vec2_to_dict(Vector2 vec2)
	{
		return new Dictionary<string, float>()
		{
			{ "x", vec2.X },
			{ "y", vec2.Y }
		};
	}

	public static Vector2 _dic_to_vec2(Dictionary<string, float> dict)
	{
		return new Vector2(dict["x"], dict["y"]);
	}

}
