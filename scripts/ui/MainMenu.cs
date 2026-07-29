using Godot;
using RPG.scripts.globals;

namespace RPG.scripts.ui;

public partial class MainMenu : Control
{
	[Export] public Button StartButton;
	[Export] public Button ExitButton;
	[Export] public string StarterScene;

	private Global _global;

	public override void _Ready()
	{
		_global = Global.Instance;
		_global.PlayerNode?.GetParent().RemoveChild(_global.PlayerNode);
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
