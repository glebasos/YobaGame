using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using YobaGame.Models;
using YobaGame.Models.Managers;

public partial class Player : CharacterBody2D
{
    public const float Speed = 300.0f;
    public const float JumpVelocity = -400.0f;

    private Sprite2D playerSprite;
    private Sprite2D handsSprite;
    private Node2D yobaHolder;
    private AnimationPlayer animationPlayer;
    private Timer throwTimer;

    private bool isThrowAllowed = true;

    private bool _isPhone;
    
    private TextureProgressBar powerBar;
    
    private float throwPower = 0f;
    private const float maxThrowPower = 1000f;
    private const float chargeSpeed = 1000f; // units per second
    private bool isCharging = false;
    
    public override void _Ready()
    {
        playerSprite = GetNode<Sprite2D>("Sprite2D");
        handsSprite = GetNode<Sprite2D>("YobaHolder/SpriteHands");
        yobaHolder = GetNode<Node2D>("YobaHolder");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        throwTimer = GetNode<Timer>("ThrowTimer");
        powerBar = GetNode<TextureProgressBar>("ProgressBarHolder/TextureProgressBar");
        
        powerBar.MaxValue = maxThrowPower;
        powerBar.Value = 0;
        powerBar.Visible = false;
        
        string platform = OS.GetName();
        _isPhone = platform == "Android" || platform == "iOS";

        animationPlayer.Play("HandIdle");
        SignalManager.GetPlayerYobaSignal += TakeYoba;

        SignalManager.InvokePlayerSpawnedYoba();

        throwTimer.Timeout += OnAllowTimer;

        SignalManager.InvokePlayerReady();

        //this.ZIndex = 100;
    }
    
    public override void _ExitTree()
    {
        throwTimer.Timeout -= OnAllowTimer;
        SignalManager.GetPlayerYobaSignal -= TakeYoba;
    }

    private void TakeYoba(Yoba _yoba)
    {
        try
        {
            if (_yoba is null)
                return;

            if (_yoba.GetParent() is null)
                yobaHolder.AddChild(_yoba);
            else
                _yoba.Reparent(yobaHolder, true);

            _yoba.GlobalPosition = yobaHolder.GlobalPosition;
            _yoba.DisableCollisionShape();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return;
        }
        
    }

    public void OnAllowTimer()
    {
        GD.Print("1");
        isThrowAllowed = true;
        GD.Print("2");
    }

    public override void _Process(double delta)
    {
        if (isCharging)
        {
            throwPower += (float)(chargeSpeed * delta);
            throwPower = Mathf.Min(throwPower, maxThrowPower);
            powerBar.Value = throwPower;
            
            // Show bar only if more than 10% charged
            if (!powerBar.Visible && throwPower >= maxThrowPower * 0.15f)
            {
                powerBar.Visible = true;
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        // Handle Jump.
        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 direction = Input.GetVector("Left", "Right", "ui_up", "ui_down");
        if (direction != Vector2.Zero)
        {
            velocity.X = direction.X * Speed;

            // Flip character based on direction
            if (direction.X < 0)
            {
                playerSprite.FlipH = true;
            }
            else if (direction.X > 0)
            {
                playerSprite.FlipH = false;
            }
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    public override void _Input(InputEvent inputEvent)
    {
        //GD.Print("Space Pressed");
        
        // Start charging
        if ((Input.IsActionJustPressed("Space") || (Input.IsActionJustPressed("LMB") && !_isPhone)) && isThrowAllowed)
        {
            isCharging = true;
            throwPower = 0;
            powerBar.Value = 0;
            //powerBar.Visible = true;
        }
        
        if ((Input.IsActionJustReleased("Space") || (Input.IsActionJustReleased("LMB") && !_isPhone)) && isThrowAllowed && isCharging)
        {
            isCharging = false;
            powerBar.Visible = false;

            IEnumerable<Yoba> yobas = yobaHolder.GetChildren().OfType<Yoba>();

            foreach (Yoba yoba in yobas)
            {
                yoba.Reparent(this.GetTree().Root, true);
                yoba.Freeze = false;
                yoba.SetZAsRelative(false);
                yoba.SetZIndex(205);
                yoba.EnableCollisionShape();

                Vector2 impulse = new Vector2(0, throwPower); // Or add horizontal adjustment if desired
                yoba.ApplyImpulse(impulse, Vector2.Zero);
            }

            SignalManager.InvokePlayerSpawnedYoba();
            isThrowAllowed = false;
            throwTimer.Start();
        }
        
        if (inputEvent is InputEventMouseMotion mouseMotionEvent && !_isPhone)
        {
            // Update the sprite position to follow the mouse
            this.GlobalPosition = new Vector2(
                Mathf.Clamp(mouseMotionEvent.Position.X, 0, GetViewport().GetVisibleRect().Size.X), // Clamp only the x-axis
                this.GlobalPosition.Y // Keep the current y-axis position
            );
        }
    }
}