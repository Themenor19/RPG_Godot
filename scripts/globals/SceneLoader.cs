using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;

namespace RPG.scripts.globals;

public partial class SceneLoader : Node2D
{
	public static SceneLoader Instance { get; private set; }
	[Signal]
	public delegate void ProgressChangedEventHandler(float progress);

	[Signal]
	public delegate void LoadFinishedEventHandler();

	public PackedScene LoadingScreen;
	public PackedScene LoadedResource;
	public string ScenePath;
	public bool UseSubThreads = true;

	private Godot.Collections.Array _progress = new();

	public override void _Ready()
	{
		Instance = this;
		LoadingScreen = GD.Load<PackedScene>("uid://bjuwbq07tri2f");
		SetProcess(false);
	}

	public void LoadScene(string scenePath)
	{
		ScenePath = scenePath;

		var newLoadScreen = LoadingScreen.Instantiate<LoadingScreen>();
		AddChild(newLoadScreen);

		ProgressChanged += newLoadScreen.OnProgressChanged;
		LoadFinished += newLoadScreen.OnLoadFinished;

		newLoadScreen.LoadingScreenReady += StartLoad;
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
				break;
			case ResourceLoader.ThreadLoadStatus.Loaded:
				SetProcess(false); // <-- also add this so _Process stops
				LoadedResource = ResourceLoader.LoadThreadedGet(ScenePath) as PackedScene;
				GetTree().ChangeSceneToPacked(LoadedResource);
				EmitSignal(SignalName.LoadFinished);
				break;
		}
	}
}
