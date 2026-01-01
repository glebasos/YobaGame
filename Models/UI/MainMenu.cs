using System;
using System.Collections.Generic;
using System.IO;
using Godot;
using YobaGame.Models.UI;
using static System.DateTime;
using FileAccess = Godot.FileAccess;

public partial class MainMenu : Control
{
	private Button _playButton;
	private Button _settingsButton;
	private Button _exitButton;
	private Button _aboutButton;
	//private Button _saveButton;
    
    private SettingsControl _settingsControl;
    private AboutControl _aboutControl;
    
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		_playButton = GetNode<Button>("MarginContainer/VBoxContainer/PlayButton");
		_settingsButton = GetNode<Button>("MarginContainer/VBoxContainer/SettingsButton");
		_exitButton = GetNode<Button>("MarginContainer/VBoxContainer/ExitButton");
		_aboutButton = GetNode<Button>("AboutButton");
		//_saveButton = GetNode<Button>("MarginContainer/VBoxContainer/SaveButton");
		
		//var children = this.GetChildren();
		_settingsControl = GetNode<SettingsControl>("SettingsControl");
		_aboutControl = GetNode<AboutControl>("AboutControl");
		
		AudioPlayer.Instance.PauseMusic();
		
		_playButton.Pressed += StartGame;
		_settingsButton.Pressed += ShowSettings;
		_exitButton.Pressed += ExitGame;
		_aboutButton.Pressed += ShowAbout;
		//_saveButton.Pressed += TestSave;
	}
	
	public override void _ExitTree()
	{
		_playButton.Pressed -= StartGame;
		_settingsButton.Pressed -= ShowSettings;
		_exitButton.Pressed -= ExitGame;
		_aboutButton.Pressed -= ShowAbout;
		//_vibrateButton.Pressed -= VibrateMainMenu;
	}

	private void ShowSettings()
	{
		_settingsControl.Visible = true;
	}
	
	private void ShowAbout()
	{
		_aboutControl.Visible = true;
	}

	private void ExitGame()
	{
		GetTree().Quit();
	}

	// private void StartGame()
	// {
	// 	GetTree().ChangeSceneToFile("res://Scenes/Main.tscn");
	// }
	private async void StartGame()
	{
		// var transition = GetNode<SceneTransition>("SceneTransition");
		// await transition.ChangeSceneWithFade("res://Scenes/Main.tscn");
		// var sT = new SceneTransition();
		SceneTransition.Instance.GoToScene("res://Scenes/Main.tscn");
	}
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
