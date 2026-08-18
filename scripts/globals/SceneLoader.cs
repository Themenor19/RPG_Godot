using System.Linq;
using Godot;
using Godot.Collections;

namespace RPG.scripts.globals;

public partial class SceneLoader : Node2D
{
	private bool _isCurrentlyLoading;

	[Signal]
	public delegate void ProgressChangedEventHandler(float progress);

	[Signal]
	public delegate void LoadFinishedEventHandler(Node newSceneRoot); // Added parameter here

	[Export] public Node2D LevelContainer;

	public PackedScene LoadingScreen;
	public PackedScene LoadedResource;
	public string ScenePath;
	public bool UseSubThreads = true;

	private Array _progress = new();
	private LoadingScreen _currentLoadingScreen; // Store a reference to clean it up
	private GlobalHandler _global;
	
	public override void _Ready()
	{
		_global = GetTree().GetRoot().GetChildren().OfType<GlobalHandler>().FirstOrDefault();
		LoadingScreen = GD.Load<PackedScene>("uid://bjuwbq07tri2f");
		SetProcess(false);
	}

	public void LoadScene(string scenePath, bool transition = true)
	{
		// 2. Guard Clause: If we are already running a load tracking operation, drop this request!
		if (_isCurrentlyLoading)
		{
			GD.Print("Bypassing duplicate LoadScene request. Already loading a scene!");
			return;
		}

		// 3. Set the flag to true to lock out incoming inputs
		_isCurrentlyLoading = true;
		ScenePath = scenePath;

		if (transition)
		{
			_currentLoadingScreen = LoadingScreen.Instantiate<LoadingScreen>();
			AddChild(_currentLoadingScreen);

			ProgressChanged += _currentLoadingScreen.OnProgressChanged;
			_currentLoadingScreen.LoadingScreenReady += StartLoad;
		}
		else
		{
			StartLoad();
		}
	}

	public void StartLoad()
	{
		var state = ResourceLoader.LoadThreadedRequest(ScenePath, "", UseSubThreads);
		if (state == Error.Ok)
		{
			SetProcess(true);
		}
	}

	public override void _Process(double delta)
	{
		var loadStatus = ResourceLoader.LoadThreadedGetStatus(ScenePath, _progress);

		if (_progress.Count > 0)
			EmitSignal(SignalName.ProgressChanged, _progress[0].AsSingle());

		switch (loadStatus)
		{
			case ResourceLoader.ThreadLoadStatus.InvalidResource:
			case ResourceLoader.ThreadLoadStatus.Failed:
				SetProcess(false);
				_currentLoadingScreen.QueueFree(); // Clean up if failed
				break;

			case ResourceLoader.ThreadLoadStatus.Loaded:
				SetProcess(false);
				LoadedResource = ResourceLoader.LoadThreadedGet(ScenePath) as PackedScene;

				if (LoadedResource != null)
				{
					var levelChildren = LevelContainer.GetChildren();
					var newSceneInstance = LoadedResource.Instantiate();

					if (newSceneInstance is Level newLevel)
					{
						_global.LoadLevelSave(newLevel);
					}
					LevelContainer.AddChild(newSceneInstance);
					
					foreach (var child in levelChildren)
					{
						if (child is Level level)
						{
							_global.SaveLevel(level);
						}
						child.QueueFree();
					}

					if (_currentLoadingScreen != null)
					{
						ProgressChanged -= _currentLoadingScreen.OnProgressChanged;
						_currentLoadingScreen.OnLoadFinished();
					}

					// 4. Reset the state tracker lock once everything finishes cleanly
					_isCurrentlyLoading = false;

					EmitSignal(SignalName.LoadFinished, newSceneInstance);
				}

				break;
		}
	}
}
