using Godot;
using System;
using YobaGame.Models.UI;

public partial class PauseMenu : Control
{
	private Button _backButton;
	private Button _settingsButton;
	private Button _mainMenuButton;
	
	private SettingsControl _settingsControl;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		
		_backButton = GetNode<Button>("SettingsContainer/VBoxContainer/BackButton");
		_settingsButton = GetNode<Button>("SettingsContainer/VBoxContainer/SettingsButton");
		_mainMenuButton = GetNode<Button>("SettingsContainer/VBoxContainer/MainMenuButton");
		
		_settingsControl = GetNode<SettingsControl>("SettingsControl");
		
		_backButton.Pressed += ContinueGame;
		_settingsButton.Pressed += ShowSettings;
		_mainMenuButton.Pressed += ReturnToMainMenu;
	}
	
	public void ContinueGame()
	{
		this.Visible = false;
		GetTree().Paused = false;
	}
	
	public void PauseGame()
	{
		GetTree().Paused = true;
		Visible = true;
	}
	
	private void ShowSettings()
	{
		_settingsControl.Visible = true;
	}
	
	private void ReturnToMainMenu()
	{
		GetTree().CallGroup("Yobas", Node.MethodName.QueueFree);
		// GetTree().ChangeSceneToFile("res://Scenes/UI/MainMenu.tscn");
		SceneTransition.Instance.GoToScene("res://Scenes/UI/MainMenu.tscn");
	}
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (GetTree().Paused && @event is InputEventKey eventKey && eventKey.Pressed && eventKey.Keycode == Key.Escape)
		{
			ContinueGame(); // Unpause when Escape is pressed
			GetViewport().SetInputAsHandled();
		}
	}
	
	// public override void _Input(InputEvent inputEvent)
	// {
	// 	if(Input.IsActionJustPressed("Escape"))
	// 	{
	// 		if (GetTree().Paused)
	// 			ContinueGame();
	// 	}
	// }
	//
	// public override void _UnhandledInput(InputEvent @event)
	// {
	// 	if (GetTree().Paused)
	// 	{
	// 		if (@event is InputEventKey eventKey && eventKey.Pressed && eventKey.Keycode == Key.Escape)
	// 		{
	// 			ContinueGame();
	// 		}
	// 	}
	// }
}
