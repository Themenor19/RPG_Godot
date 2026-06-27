using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Godot;
using FileAccess = Godot.FileAccess;

namespace RPG.scenes.dialouge;

public partial class DialoguePlayer : Control
{
	[Signal] public delegate void DialogueEndedEventHandler();
	
	[Export(PropertyHint.File, "*.json")] public string DFile { get; set; } = "";

	private List<DialogueItem> _dialogue = [];
	private int _currentDialogueIndex;
	private bool _dialogueActive;

	private NinePatchRect _rect;
	
	public override void _Ready()
	{
		_rect = GetNode<NinePatchRect>("NinePatchRect");
		_rect.Visible = false;
	}

	public void Start()
	{
		if (_dialogueActive)
		{
			return;
		}
		_dialogueActive = true;
		_dialogue = LoadDialogue();
		if (_dialogue.Count == 0)
		{
			GD.PrintErr("Could not load dialogue items: " + DFile);
		}
		_currentDialogueIndex = -1;
		NextScript();
		_rect.Visible = true;
	}

	public void NextScript()
	{
		_currentDialogueIndex++;
		if (_currentDialogueIndex >= _dialogue.Count)
		{
			Stop();
			return;
		}

		_rect.GetNode<RichTextLabel>("Name").Text = _dialogue[_currentDialogueIndex].Name;
		_rect.GetNode<RichTextLabel>("Text").Text = _dialogue[_currentDialogueIndex].Text;
	}

	private List<DialogueItem> LoadDialogue()
	{
		if (!File.Exists(DFile))
		{
			try
			{
				var json = FileAccess.GetFileAsString(DFile);
				var deserialize = JsonSerializer.Deserialize<Dialogue>(json);
				return deserialize.DialogueItems;
			}
			catch (Exception e)
			{
				GD.PrintErr("Could not load dialogue items: " + e);
			}
		}

		return [];
	}

	public void InputPressed()
	{
		if (!_dialogueActive) return;
		NextScript();
	}

	public void Stop()
	{
		_dialogueActive = false;
		_rect.Visible = false;
		EmitSignal(SignalName.DialogueEnded);
	}
}

class DialogueItem
{
	public string Name { get; set; } = "";
	public string Text { get; set; } = "";
}

class Dialogue
{
	public List<DialogueItem> DialogueItems { get; init; }
}
