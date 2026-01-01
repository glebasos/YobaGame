using Godot;
using System;
using YobaGame.Models.Managers;


public partial class HeartsControl : HBoxContainer
{
	[Export] public PackedScene HeartScene { get; set; }
	[Export] public int MaxLives { get; set; } = 5;
	private int _currentLives = 5;

	public override void _Ready()
	{
		UpdateHearts();
		_currentLives = MaxLives;

		SignalManager.LoseHealthSignal += LoseLife;
	}
	
	public override void _ExitTree()
	{
		SignalManager.LoseHealthSignal -= LoseLife;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void SetLives(int lives)
	{
		_currentLives = Mathf.Clamp(lives, 0, MaxLives);
		UpdateHearts();
	}
	
	private void UpdateHearts()
	{
		// Clear existing hearts
		foreach (Node child in GetChildren())
		{
			child.QueueFree();
		}

		// Add hearts dynamically
		for (int i = 0; i < MaxLives; i++)
		{
			var heart = HeartScene.Instantiate<HeartContainer>();
			AddChild(heart);

			// Set the state of the heart
			heart.SetState(i < _currentLives);
		}
	}
	
	public void LoseLife()
	{
		SetLives(_currentLives - 1);
		if (_currentLives == 0)
		{
			//GetTree().Quit();
			//GetTree().ChangeSceneToFile("res://Scenes/Main.tscn");
			SignalManager.InvokeGameOver();
		}
			
	}

	public void GainLife()
	{
		SetLives(_currentLives + 1);
	}
}
