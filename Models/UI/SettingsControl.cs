using Godot;
using System;
using YobaGame.Models.Managers;
using YobaGame.Models.System;

public partial class SettingsControl : Control
{
	private Button _returnButton;
	private MarginContainer _marginContainer;
	
	private HSlider _musicSlider;
	private HSlider _soundSlider;

	private CheckBox _vibrationCheckBox;
	private CheckBox _shakeCheckBox;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_returnButton =GetNode<Button>("MarginContainer/VBoxContainer/ReturnButton");
		_marginContainer = GetNode<MarginContainer>("MarginContainer/VBoxContainer/VibrationContainer");
		_musicSlider = GetNode<HSlider>("MarginContainer/VBoxContainer/MusicContainer/VBoxContainer/HBoxContainer/MusicSlider");
		_soundSlider = GetNode<HSlider>("MarginContainer/VBoxContainer/SoundContainer/VBoxContainer/HBoxContainer2/SoundSlider");
		_vibrationCheckBox =
			GetNode<CheckBox>("MarginContainer/VBoxContainer/VibrationContainer/HBoxContainer/VibrationCheckBox");
		_shakeCheckBox =
			GetNode<CheckBox>("MarginContainer/VBoxContainer/ShakeContainer/HBoxContainer/ShakeCheckBox");
		
		if(OS.GetName() != "Android")
			_marginContainer.Visible = false;
		
		Setup();

		_musicSlider.ValueChanged += SetMusicVolume;
		_musicSlider.DragEnded += SaveMusicVolumeSetting;
		
		_soundSlider.ValueChanged += SetSoundVolume;
		_soundSlider.DragEnded += SaveSoundVolumeSetting;

		_vibrationCheckBox.Pressed += SetVibration;
		_shakeCheckBox.Pressed += SetScreenShake;
		

		_returnButton.Pressed += HideSettings;
	}

	private void SaveMusicVolumeSetting(bool valuechanged)
	{
		if (valuechanged)
			ConfigFileHandler.Instance.SetSetting("audio/music_volume", _musicSlider.Value);
	}

	private void SaveSoundVolumeSetting(bool valuechanged)
	{
		if(valuechanged)
			ConfigFileHandler.Instance.SetSetting("audio/sfx_volume", _soundSlider.Value);
	}

	private void Setup()
	{
		_musicSlider.Value = ConfigFileHandler.Instance.GetSetting("audio/music_volume").AsDouble();
		_soundSlider.Value = ConfigFileHandler.Instance.GetSetting("audio/sfx_volume").AsDouble();

		SetMusicVolume(_musicSlider.Value);
		SetSoundVolume(_soundSlider.Value);
		
		_vibrationCheckBox.ButtonPressed = ConfigFileHandler.Instance.GetSetting("system/vibration").AsBool();
		_shakeCheckBox.ButtonPressed = ConfigFileHandler.Instance.GetSetting("video/screen_shake").AsBool();
		
		VibrationHandler.Instance.EnableVibration(_vibrationCheckBox.ButtonPressed);
	}

	private void SetMusicVolume(double value)
	{
		AudioPlayer.Instance.SetMusicVolume((float)value/100.0f);
	}
	private void SetSoundVolume(double value)
	{
		AudioPlayer.Instance.SetSoundEffectsVolume((float)value/100.0f);
	}
	
	private void SetVibration()
	{
		ConfigFileHandler.Instance.SetSetting("system/vibration", _vibrationCheckBox.ButtonPressed);
		VibrationHandler.Instance.EnableVibration(_vibrationCheckBox.ButtonPressed);
	}

	private void SetScreenShake()
	{
		ConfigFileHandler.Instance.SetSetting("video/screen_shake", _shakeCheckBox.ButtonPressed);
		SignalManager.InvokeCameraShake(_shakeCheckBox.ButtonPressed);
	}
	
	public override void _ExitTree()
	{
		_musicSlider.ValueChanged -= SetMusicVolume;
		_musicSlider.DragEnded -= SaveMusicVolumeSetting;
		
		_soundSlider.ValueChanged -= SetSoundVolume;
		_soundSlider.DragEnded -= SaveSoundVolumeSetting;

		_vibrationCheckBox.Pressed -= SetVibration;
		_shakeCheckBox.Pressed -= SetScreenShake;
		

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
