using System.IO;
using Godot;
using Godot.Collections;

public enum ItemTypes { Consumable, Weapon, Armor, Spell, Seed, Tool, Coin}
public enum ItemEffects { None, Heal, Damage, Hoes, Cast}
public enum ToolTypes {Default, Hoe, WateringCan, Pickaxe}

[Tool]
[GlobalClass]
public partial class InventoryItem : Resource
{
	private int _id;
	
	[Export] 
	public int Id 
	{ 
	   get => _id; 
	   set => _id = value;
	}
	
	[Export] public int Value;
	[Export] public string Name = "";

	private ItemTypes _type;
	[Export]
	public ItemTypes Type
	{
	   get => _type;
	   set
	   {
		  _type = value;
		  NotifyPropertyListChanged();
	   }
	}

	private ItemEffects _effect;
	[Export]
	public ItemEffects Effect
	{
	   get => _effect;
	   set
	   {
		  _effect = value;
		  NotifyPropertyListChanged();
	   }
	}

	//Either the spell scene or flower scene
	[Export] public PackedScene ItemScene;
	
	[Export] public Texture2D Icon;
	[Export] public ToolTypes ToolType { get; set; } = ToolTypes.Default;
	[Export] public int HealAmount;
	[Export] public int Damage {get; set;}
	[Export] public string Description { get; set; }

	// 1. Keep the constructor completely empty. No logic runs here during asset loading!
	public InventoryItem()
	{
	}

	// 2. Use the proper engine lifecycle hook to generate unique IDs safely
	public override void _Notification(int what)
	{
	   // 1 is the explicit engine constant value for NotificationInit
	   if (what == 1 && Engine.IsEditorHint() && _id == 0)
	   {
		  Callable.From(AssignNextId).CallDeferred();
	   }
	}

	private void AssignNextId()
	{
	   if (_id != 0) return; // Guard clause: already has an ID assigned, bypass completely!

	   var dir = DirAccess.Open("res://assets/items/");
	   if (dir == null)
	   {
		  GD.Print("Directory res://assets/items/ not found.");
		  return;
	   }

	   int maxId = 0;
	   dir.ListDirBegin();
	   string fileName = dir.GetNext();

	   while (!string.IsNullOrEmpty(fileName))
	   {
		  if (fileName.EndsWith(".tres"))
		  {
			 // 3. Optimization: Use Godot's built-in loader instead of raw string parsing lines.
			 // This uses the engine's internal cached memory, which is lightning fast.
			 var item = GD.Load<InventoryItem>("res://assets/items/" + fileName);
			 if (item != null && item.Id > maxId)
			 {
				maxId = item.Id;
			 }
		  }
		  fileName = dir.GetNext();
	   }

	   _id = maxId + 1;
	   EmitChanged(); // Tells the Inspector window to draw the new ID number automatically
	}

	public override void _ValidateProperty(Dictionary property)
	{
	   base._ValidateProperty(property);
	   var propertyName = property["name"].AsStringName();
	   
	   if (propertyName == PropertyName.Damage)
	   {
		  if ((Type != ItemTypes.Weapon && Type != ItemTypes.Spell) || Effect != ItemEffects.Damage)
		  {
			 property["usage"] = (int)PropertyUsageFlags.NoEditor;
		  }
	   }
	   
	   if (propertyName == PropertyName.HealAmount)
	   {
		  if (Type is not ItemTypes.Spell and not ItemTypes.Consumable || Effect != ItemEffects.Heal)
		  {
			 property["usage"] = (int)PropertyUsageFlags.NoEditor;
		  }
	   }

	   if (propertyName == PropertyName.ToolType || propertyName == PropertyName.Effect)
	   {
		  if (Type is not ItemTypes.Tool)
		  {
			 property["usage"] = (int)PropertyUsageFlags.NoEditor;
		  }
	   }

	   if (propertyName == PropertyName.ItemScene)
	   {
		  if (Type is not ItemTypes.Spell and not ItemTypes.Seed)
		  {
			 property["usage"] = (int)PropertyUsageFlags.NoEditor;
		  }
	   }
	}
}

public abstract class ItemData
{
	public int Quantity { get; set; }
	public string Name { get; set; }
	public ItemTypes Type { get; set; }
	public ItemEffects Effect { get; set; }
	public Texture2D Texture { get; set; }
	public string ScenePath { get; set; }
}
