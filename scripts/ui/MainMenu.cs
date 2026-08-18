using System.Linq;
using Godot;
using RPG.scripts.globals;

namespace RPG.scripts.ui;

public partial class MainMenu : Control
{
	[Export] public Button StartButton;
	[Export] public Button ExitButton;
	[Export] public string StarterScene;

	private GlobalHandler _global;

	public override void _Ready()
	{
		_global = GetTree().GetRoot().GetChildren().OfType<GlobalHandler>().FirstOrDefault();
		if (_global != null) _global.PlayerNode?.GetParent().RemoveChild(_global.PlayerNode);
	}

	private void _on_start_pressed()
	{
		_global.PlayerMoveScenes(StarterScene);
	}

	private void _on_exit_pressed()
	{
		GetTree().Quit();
	}
}
