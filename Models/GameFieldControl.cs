using Godot;
using System;
using System.Collections.Generic;
using YobaGame.Models;
using YobaGame.Models.Managers;

public partial class GameFieldControl : Control
{
	private Area2D _area2D;
	
	private List<Yoba> _enteredYobas;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_enteredYobas = new List<Yoba>();
		_area2D = GetNode<Area2D>("DeathArea");

		_area2D.BodyEntered += OnDeathAreaEntered;
		_area2D.BodyExited += OnDeathAreaExited;
	}
	
	public override void _ExitTree()
	{
		_area2D.BodyEntered -= OnDeathAreaEntered;
		_area2D.BodyExited -= OnDeathAreaExited;
	}

	private void OnDeathAreaExited(Node2D body)
	{
		if (body is Yoba _yoba)
		{
			if (_yoba.HasCollided)
				_enteredYobas.Remove(_yoba);
		}
	}

	private void OnDeathAreaEntered(Node2D body)
	{
		if (body is Yoba _yoba)
		{
			if(_yoba.HasCollided)
				_enteredYobas.Add(_yoba);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		foreach (var body in _area2D.GetOverlappingBodies())
		{
			if (body is Yoba _yoba)
			{
				if (_yoba.IsSleeping() ||
				    _yoba.HasCollided && _yoba.GetLinearVelocity() == Vector2.Zero)
				{
					if (!_yoba.CanDepleteHealth)
					{
						_yoba.DidDepleteHealth = true;
						SignalManager.InvokeLoseHealth();
					}
						
					_yoba.Burn();
				}
			}
		}
		
		// foreach (var yoba in _enteredYobas)
		// {
		// 	if(yoba.HasCollided && yoba.IsSleeping()  || yoba.HasCollided && yoba.GetLinearVelocity() == Vector2.Zero)
		// 		SignalManager.InvokeGameOver();
		// }
	}
}
