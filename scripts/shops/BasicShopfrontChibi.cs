using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using RPG.scripts.globals;

namespace RPG.scripts.shops;

public partial class BasicShopfrontChibi : StaticBody2D
{
	[Export] public string ShopUid;
	[Export] public InteractionArea InteractionArea;

	private GlobalHandler _global;

	public override void _Ready()
	{
		InteractionArea.Interact = () =>
		{
			try
			{
				Interact();
				return Task.CompletedTask;
			} catch(Exception e)
			{
				Console.WriteLine(e);
				return Task.FromException(e);
			}
		};
	}

	private void Interact()
	{
		_global.PlayerMoveScenes(ShopUid);
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		_global = GetTree().GetRoot().GetChildren().OfType<GlobalHandler>().FirstOrDefault();
	}
}
