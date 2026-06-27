using Godot;
using Godot.Collections;

namespace RPG.scripts.globals;

public partial class SceneLoader : Node2D
{
    public static SceneLoader Instance { get; private set; }
    
    [Signal]
    public delegate void ProgressChangedEventHandler(float progress);

    [Signal]
    public delegate void LoadFinishedEventHandler(Node newSceneRoot); // Added parameter here

    public PackedScene LoadingScreen;
    public PackedScene LoadedResource;
    public string ScenePath;
    public bool UseSubThreads = true;

    private Array _progress = new();
    private LoadingScreen _currentLoadingScreen; // Store a reference to clean it up

    public override void _Ready()
    {
       Instance = this;
       LoadingScreen = GD.Load<PackedScene>("uid://bjuwbq07tri2f");
       SetProcess(false);
    }

    public void LoadScene(string scenePath)
    {
       ScenePath = scenePath;

       _currentLoadingScreen = LoadingScreen.Instantiate<LoadingScreen>();
       AddChild(_currentLoadingScreen);

       ProgressChanged += _currentLoadingScreen.OnProgressChanged;
       
       // Note: If OnLoadFinished doesn't expect a parameter, you can change this connection
       // or update OnLoadFinished in your LoadingScreen script to accept a Node.
       LoadFinished += scene => _currentLoadingScreen.OnLoadFinished();

       _currentLoadingScreen.LoadingScreenReady += StartLoad;
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
                // 1. Capture a reference to the OLD scene before changing anything
                Node oldScene = GetTree().CurrentScene;
       
                // 2. Instantiate the level manually
                Node newSceneInstance = LoadedResource.Instantiate();
       
                // 3. Add new scene to tree and set it as current
                GetTree().Root.AddChild(newSceneInstance);
                GetTree().CurrentScene = newSceneInstance;
       
                // 4. Safely free the OLD scene
                if (oldScene != null)
                {
                   oldScene.QueueFree();
                }
                
                if (_currentLoadingScreen != null)
                {
                   ProgressChanged -= _currentLoadingScreen.OnProgressChanged;
                }

                // 5. Tell the level to setup its player
                EmitSignal(SignalName.LoadFinished, newSceneInstance);
             }
             break;
       }
    }
}