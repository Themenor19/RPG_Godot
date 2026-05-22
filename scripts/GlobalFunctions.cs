using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Schema;
using Godot;
using RPG.scripts.helper_classes;
using System.Text.Json;
using System.Threading.Tasks;

namespace RPG.scripts;

public partial class GlobalFunctions : Node
{
	private static readonly Vector2 BaseSize = new(480f, 270.0f);
	public static GlobalFunctions Instance { get; private set; }

	public static bool SaveLoaded = false;
	public static Vector2 SavedPlayerPosition;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UpdateSize();
		GetTree().GetRoot().SizeChanged += UpdateSize;
		LoadSave();
	}

	public void UpdateSize()
	{
		Vector2 sz = DisplayServer.WindowGetSize();
		float ratio = Math.Min(sz.X/BaseSize.X, sz.Y/BaseSize.Y);
		ratio = (float)Math.Max(1f, Math.Floor(ratio));
		GetWindow().ContentScaleFactor = ratio;
	}

	public static void Save(Vector2 pos)
	{
		
		PlayerSaveData playerSaveData = new PlayerSaveData
		{
			PlayerPosition = PlayerSaveData._vec2_to_dict(pos)
		};
		
		string json = JsonSerializer.Serialize(playerSaveData);
		Directory.CreateDirectory("saves");
		File.WriteAllTextAsync("saves/player_data.json", json);
		GD.Print(OS.GetExecutablePath());
		GD.Print(DirAccess.Open(".").GetCurrentDir());
		
	}

	public static void LoadSave()
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

}
