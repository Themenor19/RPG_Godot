using Godot;

public partial class Grass : TileMapLayer
{
	public override void _Input(InputEvent @event)
	{
		if (@event is not InputEventMouseButton mouseButton) return;
		if (mouseButton.ButtonIndex != MouseButton.Left && mouseButton.ButtonIndex != MouseButton.Right) return;

		Vector2I tilePosition = LocalToMap(GetLocalMousePosition());

		var canEdit = CanEdit(tilePosition);

		if (!canEdit) return;
		
		switch (mouseButton.ButtonIndex)
		{
			case MouseButton.Left:
				SetCellsTerrainConnect([tilePosition], 0, 0, false);
				break;
			case MouseButton.Right:
				SetCellsTerrainConnect([tilePosition], 0, -1, false);
				break;
		}
	}

	private bool CanEdit(Vector2I tilePosition)
	{
		bool canEdit = true;

		Vector2I[] neighbors =
		[
			tilePosition + Vector2I.Up,
			tilePosition + Vector2I.Down,
			tilePosition + Vector2I.Left,
			tilePosition + Vector2I.Right,
			tilePosition + Vector2I.Up + Vector2I.Right,
			tilePosition + Vector2I.Down + Vector2I.Left,
			tilePosition + Vector2I.Up + Vector2I.Left,
			tilePosition + Vector2I.Down + Vector2I.Right
		];

		foreach (var neighbor in neighbors)
		{
			var tileData = GetCellTileData(neighbor);
			if (tileData == null) continue;

			string type = (string)tileData.GetCustomData("TerrainType");
			if (type == "Path")
			{
				canEdit = false;
				break;
			}
		}

		return canEdit;
	}
}
