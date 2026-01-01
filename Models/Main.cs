using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Quic;
using System.Reflection;
using System.Xml.Linq;
using YobaGame.Models;
using YobaGame.Models.Managers;
using YobaGame.Models.System;
using static Godot.WebSocketPeer;

public partial class Main : Node
{
    private Dictionary<string, PackedScene> yobaScenes = new Dictionary<string, PackedScene>();
    private Dictionary<string, int> yobaValues = new Dictionary<string, int>();
    private LinkedList<string> yobaTypes = new LinkedList<string>();
    
    private Queue<Yoba> yobaQueue = new Queue<Yoba>();

    private int biggestYoba = 0;
    
    private YobaFactory _yobaFactory;

    private TextureButton _pauseButton;
    
    private PauseMenu _pauseMenu;
    
    private ScoreControl _scoreControl;
    private StatsControl _statsControl;
    private ComboMeter _comboMeter;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetTree().Paused = false;
        _pauseButton = GetNode<TextureButton>("PauseButton");
        _pauseMenu = GetNode<PauseMenu>("PauseMenu");
        _scoreControl = GetNode<ScoreControl>("ScoreControl");
        _statsControl = GetNode<StatsControl>("StatsControl");
        _comboMeter = GetNode<ComboMeter>("ComboMeter");
        
        var yobaNames = new string[]
        {
            "YobaClassic", "YobaAnime", "YobaBatya", "YobaSpurdo", "YobaButthurt",
            "YobaDerp", "YobaSmile", "YobaNya", "YobaFat", "YobaLmao", "YobaGreen"
        };

        foreach (var name in yobaNames)
        {
            yobaScenes[name] = GD.Load<PackedScene>($"res://Scenes/Yobas/{name}.tscn");
            yobaValues[name] = Array.IndexOf(yobaNames, name);
            yobaTypes.AddLast(name);
        }
        
        _yobaFactory = new YobaFactory(yobaScenes);
        
        //SignalManager.YobasCollidedSignal += SpawnNextYoba;

        GD.Randomize();

        //DisplayServer.MouseSetMode(DisplayServer.MouseMode.Hidden);
        //DisplayServer.MouseSetMode(DisplayServer.MouseMode.ConfinedHidden);
        GetTree().GetRoot().Connect("go_back_requested", new Callable(this, "_OnBackButtonPressed"));

        SignalManager.GameOverSignal += ShowStats;
        SignalManager.YobaFellSignal += ShakeRigidBodies;

        _pauseButton.Pressed += PauseGame;
        
        AudioPlayer.Instance.ResumeMusic();
    }

    public override void _ExitTree()
    {
        SignalManager.GameOverSignal -= ShowStats;
        SignalManager.YobaFellSignal -= ShakeRigidBodies;
        _pauseButton.Pressed -= PauseGame;
    }
    
    private void _OnBackButtonPressed()
    {
        if (!GetTree().Paused)
        {
            _pauseMenu.PauseGame(); // Pause when Escape is pressed
            GetViewport().SetInputAsHandled();
        }
    }
    
    private void ShowStats()
    {
        AudioPlayer.Instance.PauseMusic();
        _statsControl.SetScore(_scoreControl.GetScore());
        _statsControl.SetMaxCombo(_comboMeter.GetMaxCombo());
        _statsControl.Visible = true;
    }

    private void PauseGame()
    {
        GetTree().Paused = true;
        _pauseMenu.Visible = true;
    }
    
    private void ResumeGame()
    {
        GetTree().Paused = false;
        _pauseMenu.Visible = false;
    }

    

    private void QuitGame()
    {
        GetTree().Quit();
    }
    
    private void ReloadScene()
    {
        GetTree().CallGroup("Yobas", Node.MethodName.QueueFree);
        GetTree().ChangeSceneToFile("res://Scenes/Main.tscn");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Add any per-frame logic if needed.
    }

    // public override void _Input(InputEvent inputEvent)
    // {
    //     if(Input.IsActionJustPressed("Escape"))
    //     {
    //         if (!GetTree().Paused)
    //             PauseGame();
    //         else
    //             ResumeGame();
    //     }
    // }
    
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey eventKey && eventKey.Pressed && eventKey.Keycode == Key.Escape)
        {
            if (!GetTree().Paused)
            {
                _pauseMenu.PauseGame(); // Pause when Escape is pressed
                GetViewport().SetInputAsHandled();
            }
        }
    }
    
    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            if (!GetTree().Paused)
            {
                _pauseMenu.PauseGame(); // Pause when Escape is pressed
                GetViewport().SetInputAsHandled();
            }
        }
    }
    
    private void ShakeRigidBodies()
    {
        VibrationHandler.Vibrator.OnYobaFell();
        float force = 200f; 
        foreach (Node node in GetTree().GetNodesInGroup("Yobas"))
        {
            if (node is RigidBody2D rb && rb.Mass > 0 && rb.Freeze == false)
            {
                Vector2 randomForce = new Vector2(
                    (float)GD.RandRange(-1, 1), 
                    (float)GD.RandRange(-1, 0.5)
                ).Normalized() * force * rb.Mass; 

                rb.Sleeping = false;
            
                if (!rb.IsSleeping())
                {
                    // // Apply impulse slightly off-center to create visible movement
                    // Vector2 randomOffset = new Vector2(
                    //     (float)GD.RandRange(-10, 10), 
                    //     (float)GD.RandRange(-10, 10)
                    // );

                    //rb.ApplyImpulse(randomOffset, randomForce); // Use off-center impulse
                    // OR if still not working, try:
                    rb.ApplyCentralImpulse(randomForce);
                }
            }
        }
    }
}
