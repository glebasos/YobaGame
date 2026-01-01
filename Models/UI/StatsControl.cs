using Godot;
using System;
using YobaGame.Models.System;
using YobaGame.Models.UI;

public partial class StatsControl : Control
{
	private Label _scoreLabel;
	private Label _highScoreLabel;
	private Label _maxComboLabel;

	private Button _restartButton;
	private Button _menuButton;
	
	private int _highScore;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_scoreLabel = GetNode<Label>("MarginContainer/VBoxContainer/MarginContainer/HBoxContainer/ScoreLabel");
		_highScoreLabel = GetNode<Label>("MarginContainer/VBoxContainer/MarginContainer2/HBoxContainer/HighScorelabel");
		_maxComboLabel = GetNode<Label>("MarginContainer/VBoxContainer/MarginContainer4/HBoxContainer/MaxComboLabel");
		
		_restartButton = GetNode<Button>("MarginContainer/VBoxContainer/MarginContainer3/HBoxContainer/RestartButton");
		_menuButton = GetNode<Button>("MarginContainer/VBoxContainer/MarginContainer3/HBoxContainer/MenuButton");
		
		_highScore = ConfigFileHandler.Instance.GetSetting("game_data/high_score").AsInt32();
		_highScoreLabel.Text = _highScore.ToString().PadZeros(8);

		_restartButton.Pressed += RestartGame;
		_menuButton.Pressed += GoToMainMenu;
	}
	
	public override void _ExitTree()
	{
		_restartButton.Pressed -= RestartGame;
		_menuButton.Pressed -= GoToMainMenu;
	}

	private void RestartGame()
	{
		GetTree().CallGroup("Yobas", Node.MethodName.QueueFree);
		//GetTree().ChangeSceneToFile("res://Scenes/Main.tscn");
		SceneTransition.Instance.GoToScene("res://Scenes/Main.tscn");
	}

	private void GoToMainMenu()
	{
		GetTree().CallGroup("Yobas", Node.MethodName.QueueFree);
		SceneTransition.Instance.GoToScene("res://Scenes/UI/MainMenu.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetScore(int score)
	{
		_scoreLabel.Text = score.ToString().PadZeros(8);
		if (score > _highScore)
		{
			_highScore = score;
			_highScoreLabel.Text = _highScore.ToString().PadZeros(8);
			ConfigFileHandler.Instance.SetSetting("game_data/high_score", score);
		}
	}

	public void SetMaxCombo(int combo)
	{
		_maxComboLabel.Text = combo.ToString();
	}

	public void Show()
	{
		this.Visible = true;
	}

	public void Hide()
	{
		this.Visible = false;
	}
}
