using Godot;
using System;
using YobaGame.Models;
using YobaGame.Models.Managers;

public partial class NextYoba : Node2D
{

    private Node2D yobaHolder;

    public override void _Ready()
    {
        yobaHolder = GetNode<Node2D>("YobaHolder");
        
        SignalManager.GetNextYobaSignal += TakeYoba;
        SignalManager.InvokeNextYobaReady();
    }

    public override void _ExitTree()
    {
        SignalManager.GetNextYobaSignal -= TakeYoba;
    }
    
    private void TakeYoba(Yoba _yoba)
    {
        try
        {
            var children = yobaHolder.GetChildren();
            foreach (var child in children)
                yobaHolder.RemoveChild(child); 
            yobaHolder.AddChild(_yoba);
            _yoba.GlobalPosition = yobaHolder.GlobalPosition;
            _yoba.DisableCollisionShape();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return;
        }
        
    }
    
    private void TakeYobae()
    {
        var children = yobaHolder.GetChildren();
        foreach (var child in children)
            yobaHolder.RemoveChild(child); 
        // yobaHolder.AddChild(_yoba);
        // _yoba.DisableCollisionShape();
    }
}
