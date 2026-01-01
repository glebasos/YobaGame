using Godot;
using NotImplementedException = System.NotImplementedException;

namespace YobaGame.Models.System;

public  partial class VibrationHandler : Node
{
    public static VibrationHandler Instance { get; private set; }
    
    public static BaseVibrator Vibrator { get; private set; }

    public VibrationHandler()
    {
        EnableVibration(true);
    }
    
    public override void _Ready()
    {
        Instance = this;
    }

    public void EnableVibration(bool enable)
    {
        Vibrator = enable ? Vibrator = CheckVibrationDevice() : new BaseVibrator();
    }

    private BaseVibrator CheckVibrationDevice()
    {
        if (OS.GetName() == "Android")
            return new AndroidVibrator();
        
        return new BaseVibrator();
    }
}