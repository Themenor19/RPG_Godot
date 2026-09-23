using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace RPG.scripts.helper_classes.save_data;

public class LevelSaveData: AbcSave
{
	private const string LevelArrayKey = "LevelArray";
	private const string KeyCurrentLevel = "CurrentLevel";

	
	public Array<Dictionary> Levels;
	public string CurrentLvl { get; set; }
	
	protected override Dictionary DataToSave()
	{
		return new Dictionary
		{
			{ LevelArrayKey, Levels },
			{ KeyCurrentLevel, CurrentLvl }
		};
	}

	protected override Error DataFromSave(Dictionary data)
	{
		var error = VerifySave(data);
		if (error != Error.Ok) return error;

		try
		{
			Levels = (Array<Dictionary>)data[LevelArrayKey];
			CurrentLvl = (string)data[KeyCurrentLevel];

			return Error.Ok;
		}
		catch
		{
			return Error.ParseError;
		}
	}

	protected override Error VerifySave(Dictionary data)
	{
		if (!data.ContainsKey(LevelArrayKey) || !data.ContainsKey(KeyCurrentLevel))
		{
			return Error.InvalidData;
		}
		
		return Error.Ok;
	}
}
