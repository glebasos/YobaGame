using Godot;
using System;
using System.Collections.Generic;
using YobaGame.Models.Managers;

public partial class AudioPlayer : Node
{
	[Export] private int numPlayers = 8;
	[Export] private string soundEffectsBus = "sound_effects";
	[Export] private string musicBus = "music";
	[Export] private float soundEffectsVolume = 1.0f;
	[Export] private float musicVolume = 1.0f;
	
	public static AudioPlayer Instance { get; private set; }
	
	//available players
	private List<AudioStreamPlayer2D> available = new List<AudioStreamPlayer2D>();
	private Queue<AudioStream> soundQueue = new Queue<AudioStream>();
	
	AudioStreamPlayer musicPlayer;
	private Queue<string> musicQueue = new Queue<string>();
	
	//AudioStream musicStream;
	List<AudioStream> soundStreams = new List<AudioStream>();
	List<string> musicList = new List<string>();
	
	private bool _isMusicPaused = false; // Track music pause state
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		
		for (int i = 0; i < numPlayers; i++)
		{
			var player = new AudioStreamPlayer2D();
			AddChild(player);
			available.Add(player);
			player.Bus = soundEffectsBus;
			player.VolumeDb = (float)Mathf.LinearToDb(soundEffectsVolume);
			player.Finished += () => OnStreamFinished(player);
		}
		
		musicPlayer = new AudioStreamPlayer();
		AddChild(musicPlayer);
		musicPlayer.Bus = musicBus;
		musicPlayer.VolumeDb = (float)Mathf.LinearToDb(musicVolume);
		musicPlayer.Finished += OnMusicFinished;
		
		soundStreams.Add(GD.Load<AudioStream>("res://Resources/Audio/Sounds/pop_001.mp3"));
		soundStreams.Add(GD.Load<AudioStream>("res://Resources/Audio/Sounds/pop_002.mp3"));
		soundStreams.Add(GD.Load<AudioStream>("res://Resources/Audio/Sounds/pop_003.mp3"));
		
		soundStreams.Add(GD.Load<AudioStream>("res://Resources/Audio/Sounds/burn1.mp3"));
		soundStreams.Add(GD.Load<AudioStream>("res://Resources/Audio/Sounds/burn2.mp3"));
		soundStreams.Add(GD.Load<AudioStream>("res://Resources/Audio/Sounds/burn3.mp3"));

		musicList = new()
		{
			"res://Resources/Audio/Music/crab.mp3",
			"res://Resources/Audio/Music/dikiy.mp3",
			"res://Resources/Audio/Music/govno.mp3",
			"res://Resources/Audio/Music/intel.mp3",
			"res://Resources/Audio/Music/korporat.mp3",
			"res://Resources/Audio/Music/revol.mp3",
			"res://Resources/Audio/Music/sumatoha.mp3",
			"res://Resources/Audio/Music/vdul.mp3",
			"res://Resources/Audio/Music/wife.mp3",
			"res://Resources/Audio/Music/woman.mp3",
		};

		SignalManager.PopSignal += PlayPop;
		SignalManager.BurnSignal += PlayBurn;
		
		GD.Randomize();
	}
	
	private void OnStreamFinished(AudioStreamPlayer2D stream)
	{
		// When finished playing a stream, make the player available again.
		available.Add(stream);
	}
	
	private void OnMusicFinished()
	{
		// Play the next track in the music queue if available.
		if (musicQueue.Count > 0)
		{
			PlayNextMusicTrack();
		}
	}
	
	public void PlayPop()
	{
		soundQueue.Enqueue(soundStreams[GD.RandRange(0, 2)]);
	}
	
	public void PlayBurn()
	{
		soundQueue.Enqueue(soundStreams[GD.RandRange(3, 5)]);
	}
	
	public void PlayMusic(string musicPath)
	{
		musicQueue.Enqueue(musicPath);

		// If no music is currently playing, start playing immediately.
		if (!musicPlayer.Playing)
		{
			PlayNextMusicTrack();
		}
	}
	
	private void PlayNextMusicTrack()
	{
		if (musicQueue.Count > 0)
		{
			musicPlayer.Stream = GD.Load<AudioStream>(musicQueue.Dequeue());
			musicPlayer.Play();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Play a queued sound if any players are available.
		if (soundQueue.Count > 0 && available.Count > 0)
		{
			var player = available[0];
			player.Stream = soundQueue.Dequeue();
			player.Play();
			available.RemoveAt(0);
		}
		
		if (musicPlayer is not null && !musicPlayer.Playing && !_isMusicPaused)
		{
			musicQueue.Enqueue(musicList[GD.RandRange(0, musicList.Count-1)]);
			PlayNextMusicTrack();
		}
		
	}
	
	public void SetSoundEffectsVolume(float volume)
	{
		soundEffectsVolume = volume;
		foreach (var player in available)
		{
			
			player.VolumeDb = (float)Mathf.LinearToDb(volume);
		}
	}

	public void SetMusicVolume(float volume)
	{
		musicVolume = volume;
		musicPlayer.VolumeDb = (float)Mathf.LinearToDb(volume);
	}
	
	// Pause music
	public void PauseMusic()
	{
		if (musicPlayer.Playing)
			musicPlayer.StreamPaused = true;
		_isMusicPaused = true;

	}

	// Resume music
	public void ResumeMusic()
	{
		if (_isMusicPaused)
		{
			musicPlayer.StreamPaused = false;
			PlayNextMusicTrack();
			_isMusicPaused = false;
		}
	}
}
