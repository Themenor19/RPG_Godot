using Godot;

public partial class LoadingScreen : CanvasLayer
{
	[Signal]
	public delegate void LoadingScreenReadyEventHandler();
	
	[Export] public AnimationPlayer LoadingScreenAnimation;

	public override void _Ready()
	{
		// Setup initial fade-in hook
		LoadingScreenAnimation.AnimationFinished += OnFadeInFinished;
	}

	private void OnFadeInFinished(StringName animName)
	{
		LoadingScreenAnimation.AnimationFinished -= OnFadeInFinished;
		EmitSignal(SignalName.LoadingScreenReady);
	}

	public void OnProgressChanged(float progress)
	{
		// Progress bar logic here if needed
	}

	public void OnLoadFinished()
	{
		// 1. Defensively remove the method first, preventing duplicate connection errors
		LoadingScreenAnimation.AnimationFinished -= OnFadeOutFinished;
		LoadingScreenAnimation.AnimationFinished += OnFadeOutFinished;
	   
		// 2. Play the fade out
		LoadingScreenAnimation.PlayBackwards("transition");
	}

	// 3. Extracted lambda out to a named method so Godot can cleanly track/disconnect it
	private void OnFadeOutFinished(StringName animName)
	{
		LoadingScreenAnimation.AnimationFinished -= OnFadeOutFinished;
		QueueFree();
	}
}
