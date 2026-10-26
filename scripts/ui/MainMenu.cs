using System.Linq;
using Godot;
using RPG.scripts.globals;

namespace RPG.scripts.ui;

public partial class MainMenu : Control
{
	[Export] public Button StartButton;
	[Export] public Button ExitButton;
	[Export] public BaseButton ControlButton;
	[Export] public string ControlButtonScene;
	[Export] public string StarterScene;
	[Export] public string MainMenuScene;

	private GlobalHandler _global;

	public override void _Ready()
	{
		_global = GetTree().GetRoot().GetChildren().OfType<GlobalHandler>().FirstOrDefault();
		if (_global == null) return;
		var parent = _global.PlayerNode?.GetParent();
		parent?.RemoveChild(_global.PlayerNode);
	}

	private void _on_start_pressed()
	{
		_global.PlayerMoveScenes(string.IsNullOrEmpty(_global.CurrentLevelUid) ? StarterScene : _global.CurrentLevelUid, position: _global.SavedPlayerPosition);
	}

	private void _on_exit_pressed()
	{
		GetTree().Quit();
	}

	private void _on_control_button_pressed()
	{
		_global.QueueLevelAndMove(ControlButtonScene, levelSceneOverride: MainMenuScene, transition:false);
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		ControlButton.Pressed += _on_control_button_pressed;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		ControlButton.Pressed -= _on_control_button_pressed;
	}
}
