using Godot;
using System;
using YobaGame.Models.Managers;
using YobaGame.Models.System;

public partial class ShakeCamera : Camera2D
{
	[Export] public float ShakeStrength { get; set; } = 10f;
	[Export] public float ShakeFade { get; set; } = 10f;

	private float _shakeStrength = 0f;
	private Vector2 _originalOffset = Vector2.Zero;

	private bool _shakeEnabled = true;

	public override void _Ready()
	{
		GD.Randomize();
		_originalOffset = Offset; // Store the original offset
		_shakeEnabled = ConfigFileHandler.Instance.GetSetting("video/screen_shake").AsBool();
		SignalManager.YobaFellSignal += Shake;
		SignalManager.CameraShakeSignal += EnableShake;
	}

	private void EnableShake(bool _enable)
	{
		_shakeEnabled = _enable;
	}

	public override void _ExitTree()
	{
		SignalManager.YobaFellSignal -= Shake;
	}

	public override void _Process(double delta)
	{
		if (_shakeEnabled)
		{
			if (_shakeStrength > 0)
			{
				// Apply randomized shake
				Offset = _originalOffset + RandomOffset();
            
				// Reduce shake strength over time (exponential decay)
				_shakeStrength = Mathf.MoveToward(_shakeStrength, 0, ShakeFade * (float)delta);
			}
			else
			{
				// Reset to original position
				Offset = _originalOffset;
			}
		}
	}

	private void Shake()
	{
		if(_shakeEnabled)
			_shakeStrength = ShakeStrength;
	}

	private Vector2 RandomOffset()
	{
		return new Vector2(
			(float)GD.RandRange(-_shakeStrength, _shakeStrength),
			(float)GD.RandRange(-_shakeStrength, _shakeStrength)
		) * 0.1f; // Reduce jitter scale to avoid excessive movement
	}
}
