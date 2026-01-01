using Godot;
using System;
using YobaGame.Models.Managers;
using YobaGame.Models.System;

public partial class AboutControl : Control
{
	private Button _returnButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_returnButton = GetNode<Button>("MarginContainer/VBoxContainer/ReturnButton");
		_returnButton.Pressed += HideSettings;
	}
	public override void _ExitTree()
	{
		_returnButton.Pressed -= HideSettings;
	}

	private void HideSettings()
	{
		this.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
