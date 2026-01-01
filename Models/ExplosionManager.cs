using Godot;
using System;
using YobaGame.Models;
using YobaGame.Models.Managers;

public partial class ExplosionManager : Node2D
{
    private PackedScene explosionScene = new PackedScene();
    public override void _Ready()
    {
        explosionScene = GD.Load<PackedScene>($"res://Scenes/ExplodedSprite.tscn");
        SignalManager.ExplodeSignal += OnExplodeSignal;
    }

    private void OnExplodeSignal(Yoba _yoba, Yoba _otheryoba)
    {
        var exp1 = (ExplodedSprite)explosionScene.Instantiate();
        var exp2 = (ExplodedSprite)explosionScene.Instantiate();
        
        this.GetTree().Root.AddChild(exp1);
        this.GetTree().Root.AddChild(exp2);

        exp1.GlobalPosition = _yoba.GlobalPosition;
        exp2.GlobalPosition = _otheryoba.GlobalPosition;
        
        exp1.SetSprite(_yoba.Sprite);
        exp2.SetSprite(_otheryoba.Sprite);
        
        exp1.PlayExplosionAnimation();
        exp2.PlayExplosionAnimation();
        
    }
}
