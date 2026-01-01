using Godot;
using System;
using System.Collections.Generic;
using YobaGame.Models;
using YobaGame.Models.Managers;

public partial class ComboMeter : Node2D
{
    private int _currentCombo = 0;
    private int _comboScore = 0;
    private int _maxCombo = 0;
    private Timer _comboTimer;
    private List<Sprite2D> _comboSprites = new();
    private Sprite2D _currentSprite;

    private string[] _comboNames = new[]
    {
        "D", "C", "B", "A", "S", "SS", "SSS"
    };
    
    private Dictionary<string, int> _scoreTable = new Dictionary<string, int>
    {
        { "YobaClassic", 1 },    // Cherry
        { "YobaAnime", 3 },      // Strawberry
        { "YobaBatya", 6 },      // Grapes
        { "YobaSpurdo", 10 },    // Dekopon
        { "YobaButthurt", 15 },  // Persimmon
        { "YobaDerp", 21 },      // Apple
        { "YobaSmile", 28 },     // Pear
        { "YobaNya", 36 },       // Peach
        { "YobaFat", 45 },       // Pineapple
        { "YobaLmao", 55 },      // Melon
        { "YobaGreen", 66 }      // Watermelon
    };
    
    public override void _Ready()
    {
        //init all combo sprites
        foreach (var comboName in _comboNames)
        {
            Sprite2D comboSprite = new Sprite2D
            {
                Texture = (Texture2D)GD.Load($"res://Resources/Misc/Combos/{comboName}.png"),
                Modulate = new Color(1, 1, 1, 0), // Start fully transparent
                Visible = true // Initially hidden
            };
            _comboSprites.Add(comboSprite);
            AddChild(comboSprite);
        }
        SignalManager.PlayerSpawnedYobaSignal += ResetCombo;
        SignalManager.YobasCollidedSignal += IncrementCombo;
        
        _comboTimer = GetNode<Timer>("Timer");
        _comboTimer.Timeout += ResetCombo;
    }
    
    public override void _ExitTree()
    {
        _comboTimer.Timeout -= ResetCombo;
        SignalManager.PlayerSpawnedYobaSignal -= ResetCombo;
        SignalManager.YobasCollidedSignal -= IncrementCombo;
    }

    private void IncrementCombo(Vector2 _playerpos, Yoba _yoba, Yoba _otheryoba)
    {
        _comboTimer.Start();
        _currentCombo++;
        CalculateComboScore(_currentCombo, _yoba);
        ShowComboSprite();
    }

    private void CalculateComboScore(int currentCombo, Yoba yoba)
    {
        _comboScore += currentCombo * _scoreTable[yoba.YobaName] * 2;
    }

    private void ShowComboSprite()
    {
        int index = Mathf.Min(_currentCombo, _comboNames.Length) - 1;
        if (index < 0) return;

        // If the new combo level is the same as the last one, don't fade it out
        if (_currentSprite != null && _currentSprite != _comboSprites[index])
        {
            Node2D previousSprite = _currentSprite;

            // Stop any running tweens on the previous sprite
            previousSprite.SetProcess(false); // Ensures tween doesn't interfere
            previousSprite.Modulate = new Color(1, 1, 1, 0); // Fully transparent
            previousSprite.Visible = false;
        }

        _currentSprite = _comboSprites[index];
        _currentSprite.Visible = true;
        _currentSprite.Modulate = new Color(1, 1, 1, 0); // Start invisible

        Tween tween = GetTree().CreateTween();

        // Fade in quickly with a strong ease
        tween.TweenProperty(_currentSprite, "modulate:a", 1, 0.1f)
            .SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);

        // Overshoot scale effect for impact
        tween.TweenProperty(_currentSprite, "scale", new Vector2(1.2f, 1.2f), 0.15f)
            .SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);

        // Scale back to normal
        tween.TweenProperty(_currentSprite, "scale", new Vector2(1f, 1f), 0.1f)
            .SetTrans(Tween.TransitionType.Linear)
            .SetEase(Tween.EaseType.In);
    }

    private void ResetCombo()
    {
        // send _comboScore to score control
        SignalManager.InvokeComboScored(_comboScore);
        
        if(_currentCombo > _maxCombo)
            _maxCombo = _currentCombo;
        
        _comboScore = 0;
        _currentCombo = 0;
        _comboTimer.Stop();
        if (_currentSprite is not null)
        {
            Node2D previousSprite = _currentSprite;

            Tween fadeOutTween = GetTree().CreateTween();
            fadeOutTween.TweenProperty(previousSprite, "modulate:a", 0, 0.5f);
            fadeOutTween.TweenCallback(Callable.From(() => previousSprite.Visible = false));
            //_currentSprite.Visible = false;
            _currentSprite = null;
        }
    }
    
    public int GetMaxCombo() => _maxCombo;
}
