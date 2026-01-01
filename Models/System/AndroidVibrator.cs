using Godot;

namespace YobaGame.Models.System;

public class AndroidVibrator : BaseVibrator
{
    private const int BUTTON_PRESS = 10;
    private const int YOBA_FELL = 100;

    public override void OnButtonPressed()
    {
        Input.VibrateHandheld(BUTTON_PRESS);
    }

    public override void OnYobaFell()
    {
        Input.VibrateHandheld(YOBA_FELL);
    }
}