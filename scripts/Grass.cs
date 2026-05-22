using Godot;

public partial class Grass : TileMapLayer
{
	private bool _isClickable = true;
	private double _timer;

	public override void _Process(double delta)
	{
		if (!_isClickable)
		{
			if (_timer <= .2)
			{
				_timer += delta * 1.0;
			}
			else
			{
				_timer = 0.0;
				_isClickable = true;
			}
		}
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is not InputEventMouseButton mouseButton) return;
		if (mouseButton.ButtonIndex != MouseButton.Left && mouseButton.ButtonIndex != MouseButton.Right) return;
		if (!_isClickable) return;

		Vector2I tilePosition = LocalToMap(GetLocalMousePosition());

		var canEdit = CanEdit(tilePosition, mouseButton);

		if (!canEdit) return;
		
		switch (mouseButton.ButtonIndex)
		{
			case MouseButton.Left:
				SetCellsTerrainConnect([tilePosition], 0, 0, false);
				_isClickable = false;
				break;
			case MouseButton.Right:
				SetCellsTerrainConnect([tilePosition], 0, -1, false);
				_isClickable = false;
				break;
		}
	}

	private bool CanEdit(Vector2I tilePosition, InputEventMouseButton eventButton)
	{
		var currentTileData = GetCellTileData(tilePosition);
		
		if (eventButton.ButtonIndex == MouseButton.Left && currentTileData != null)
		{
			return false;
		}
		
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
