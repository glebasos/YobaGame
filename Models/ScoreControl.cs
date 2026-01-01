using Godot;
using System;
using YobaGame.Models.Managers;

public partial class ScoreControl : Control
{
	private Label _scoreLabel;
	
	private int _score = 0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_scoreLabel = GetNode<Label>("ScoreLabel");
		_scoreLabel.Text = _score.ToString().PadLeft(8, '0');;
		
		SignalManager.ComboScoredSignal += ComboScored;
	}
	
	public override void _ExitTree()
	{
		SignalManager.ComboScoredSignal -= ComboScored;
	}

	private void ComboScored(int comboScore)
	{
		// _score += comboScore;
		// _scoreLabel.Text = _score.ToString().PadLeft(8, '0');
		int startScore = _score;
		int endScore = _score + comboScore;
		_score = endScore; // Store the actual score

		Tween scoreTween = GetTree().CreateTween();
		scoreTween.TweenMethod(Callable.From((float value) =>
			{
				_scoreLabel.Text = ((int)value).ToString().PadLeft(8, '0');
			}), startScore, endScore, 0.5f) // Smoothly animate over 0.5 seconds
			.SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.Out);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public int GetScore() => _score;
}
