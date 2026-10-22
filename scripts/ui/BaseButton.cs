using Godot;

namespace RPG.scripts.ui;

[Tool]
public partial class BaseButton : Button
{
	private Texture2D _buttonTexture;
	private Texture2D _buttonPressedTexture;

	[Export]
	public Texture2D ButtonTexture
	{
		get => _buttonTexture;
		set
		{
			_buttonTexture = value;
			SetButtonTexture();
		}
	}

	[Export]
	public Texture2D ButtonPressedTexture
	{
		get => _buttonPressedTexture;
		set
		{
			_buttonPressedTexture = value;
			SetButtonTexture();
		}
	}
	private void SetButtonTexture()
	{
		StyleBox styleBoxPressed = IsInstanceValid(ButtonPressedTexture) 
			? new StyleBoxTexture { Texture = ButtonPressedTexture } 
			: new StyleBoxEmpty();
		StyleBox styleBox = IsInstanceValid(ButtonTexture)
			? new StyleBoxTexture { Texture = ButtonTexture }
			: new StyleBoxEmpty();
		
		AddThemeStyleboxOverride("normal", styleBox);
		AddThemeStyleboxOverride("pressed", styleBox);
		AddThemeStyleboxOverride("hover", styleBoxPressed);
		AddThemeStyleboxOverride("hover_pressed", styleBox);

		GD.Print(styleBox);
		GD.Print(this.GetThemeStylebox("normal"));
	}
}
