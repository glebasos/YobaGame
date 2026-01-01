using Godot;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace YobaGame.Models.UI;
public partial class SceneTransition: CanvasLayer {

    public static SceneTransition Instance { get; private set; }
    
    [Export] private float _switchDuration = 1f;
    public string currentScene = "";

    private ColorRect _colorRect;

    public SceneTransition() {
        Instance = this;
    }

    public override void _Ready() {
        
        _colorRect = GetNode<ColorRect>("ColorRect");
        _colorRect.MouseFilter = Control.MouseFilterEnum.Ignore;
        _colorRect.Modulate = new Color(1, 1, 1, 0);
    }

    public async void GoToScene(string scene) {
        _colorRect.MouseFilter = Control.MouseFilterEnum.Stop;

        Tween tween = GetTree().CreateTween();
        tween.SetPauseMode(Tween.TweenPauseMode.Process);
        tween.TweenProperty(_colorRect, "modulate", 
            new Color(1, 1, 1, 1), _switchDuration / 2f);
        await ToSignal(tween, Tween.SignalName.Finished);

        GetTree().ChangeSceneToFile(
            $"{scene}");
        GetTree().Paused = false;
        currentScene = scene;

        tween = GetTree().CreateTween();
        tween.SetPauseMode(Tween.TweenPauseMode.Process);
        tween.TweenProperty(_colorRect, "modulate",
            new Color(1, 1, 1, 0), _switchDuration / 2f);
        _colorRect.MouseFilter = Control.MouseFilterEnum.Ignore;
    }
}