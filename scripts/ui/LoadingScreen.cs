using Godot;
using System;
using System.Threading.Tasks;

public partial class LoadingScreen : CanvasLayer
{
	[Signal]
	public delegate void LoadingScreenReadyEventHandler();
	
	[Export] AnimationPlayer LoadingScreenAnimation;

	private bool _deleteSelf = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LoadingScreenAnimation.AnimationFinished += EmitLoadingScreenReady;
	}

	public void EmitLoadingScreenReady(StringName name)
	{
		if (_deleteSelf)
		{
			QueueFree();
			return;
		}
		EmitSignal(SignalName.LoadingScreenReady);
	}


	public void OnProgressChanged(float progress)
	{
		
	}

	public void OnLoadFinished()
	{
		_deleteSelf = true;
		LoadingScreenAnimation.PlayBackwards("transition");
	}
}
