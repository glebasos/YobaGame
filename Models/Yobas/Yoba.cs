using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YobaGame.Models.Managers;
using Tween = Godot.Tween;

namespace YobaGame.Models
{
    public partial class Yoba : RigidBody2D
    {
        public string YobaName { get; set; }
        public string ImagePath { get; set; } // Path to the image file
        public float Diameter { get; set; } // Diameter of the Yoba

        private Timer timer;
        // Private field to hold the PhysicsMaterial
        private PhysicsMaterial _material;
        private Sprite2D sprite; // For visual representation
        public Sprite2D Sprite => sprite;

        private CollisionShape2D collisionShape; // For handling collisions
        private CircleShape2D circleShape; // Assuming a CircleShape for simplicity
        // Called when the node enters the scene tree for the first time.
        
        private AnimationPlayer animationPlayer;
        
        private VisibleOnScreenNotifier2D visibleOnScreenNotifier;
        
        private bool hasCollided = false;
        public bool HasCollided => hasCollided;

        private bool canDepleteHealth = false;
        public bool CanDepleteHealth => canDepleteHealth;
        
        public bool DidDepleteHealth { get; set; }  = false;
        
        Shader _currentShader = new Shader();
        private List<Shader> _shaders = new();
        List<string> _shaderPaths = new()
        {
            "res://Scenes/Yobas/Explosion.gdshader",
            "res://Scenes/Yobas/Burn.gdshader",
        };
        
        private Shader _testShader;
        
        CompressedTexture2D _dissolveTexture;
        
        public override void _Ready()
        {
            //this.ZIndex = 120;
            AddToGroup("Yobas");
            
            sprite = GetNode<Sprite2D>("Sprite2D");
            collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
            Connect("body_entered", new Callable(this, "_OnBodyEntered"));
            
            visibleOnScreenNotifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
            visibleOnScreenNotifier.ScreenExited += OnScreenLeaving;
            
            animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            // animationPlayer.AnimationFinished += OnAnimationFinished;
            // animationPlayer.Play("RESET");

            // Create a new Timer instance.
            timer = new Timer();
            // Set the wait time in seconds.
            timer.WaitTime = 0.1f;
            // Connect the timeout signal to the _OnTimerTimeout method.
            timer.Connect("timeout", new Callable(this, "_OnTimerTimeout"));
            // Add the timer to the scene tree.
            AddChild(timer);

            _dissolveTexture = (CompressedTexture2D)GD.Load("res://Resources/Misc/milky 2 - 256x256.png");
            foreach (var path in _shaderPaths)
            {
                _shaders.Add((Shader)GD.Load(path));
            }
            if (_shaders.Count > 0)
            {
                _currentShader = _shaders[0];
                
                ShaderMaterial shaderMaterial = new ShaderMaterial();
                shaderMaterial.Shader = _currentShader;
                shaderMaterial.SetShaderParameter("dissolve_state", 1.0f);
                
                sprite.Material = shaderMaterial;
            }
            
            //_testShader = (Shader)GD.Load("res://Scenes/Yobas/Explosion.gdshader");
            ;
            //
            // ShaderMaterial shaderMaterial = new ShaderMaterial();
            // shaderMaterial.Shader = _testShader;
            // shaderMaterial.SetShaderParameter("dissolve_state", 1.0f);
            //
            // // Assign the ShaderMaterial to the Sprite2D
            // sprite.Material = shaderMaterial;
            
            //SignalManager.YobaFellSignal += Shake;
        }
        
        public override void _ExitTree()
        {
            //SignalManager.YobaFellSignal -= Shake;
            // if (visibleOnScreenNotifier != null)
            // {
            //     visibleOnScreenNotifier.ScreenExited -= OnScreenLeaving;
            // }
        }
        
        private void Shake()
        {
            float force = 500f; 

            Vector2 randomForce = new Vector2(
                (float)GD.RandRange(-1, 1), 
                (float)GD.RandRange(-1, 0.5)
            ).Normalized() * force;
            GD.Print(randomForce);
            this.ApplyImpulse(randomForce, Vector2.Zero);
        }
        
        private void TweenShaderParameter()
        {
            Tween tween = GetTree().CreateTween();

            // Assuming the shader has a uniform parameter named "time"
            //tween.TweenProperty(sprite.Material, "shader_parameter/time", 1.0f, 0.5f);
            //tween.TweenProperty(sprite.Material, "shader_parameter/dissolve_state", 1.0f, 0.5f);
            tween.TweenProperty(sprite.Material, "shader_parameter/dissolve_state", 0, 0.5f)
                .SetTrans(Tween.TransitionType.Quad)
                .SetEase(Tween.EaseType.Out);
        }
        
        // public override void _ExitTree()
        // {
        //     //SignalManager.GameOverSignal -= GameOverExplode;
        // }


        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        }

        // Called when another body enters the collision area
        private void _OnBodyEntered(Node body)
        {
            if (body is Yoba otherYoba)
            {
                // Only collide with other Yobas of the same type
                if (YobaName == otherYoba.YobaName)
                {
                    Vector2 middlePoint = CalculateMiddlePoint(otherYoba);
                    GD.Print($"{YobaName} Collided with {otherYoba.YobaName}");

                    // Only spawn new Yoba if conditions are met (e.g., IDs, sleeping states)
                    if (ShouldSpawnNewYoba(otherYoba))
                    {
                        hasCollided = true;
                        CallDeferred("SpawnNewNode", middlePoint, otherYoba);
                        
                        SignalManager.InvokePop();
                        
                        Explode();
                        otherYoba.CallDeferred("Explode");
                        
                        // TweenShaderParameter();
                        // otherYoba.CallDeferred("TweenShaderParameter");
                    }
                }
                hasCollided = true;
            }
        }
        
        

        public void Explode()
        {
            //this.Freeze = true;
            canDepleteHealth = true;
            SetDeferred("freeze", true);
            collisionShape.SetDeferred("disabled", true);
            
            if (sprite.Material == null || !(sprite.Material is ShaderMaterial))
            {
                GD.PrintErr("ShaderMaterial is not assigned to Sprite2D!");
                return;
            }
            
            Tween tween = GetTree().CreateTween();
            //tween.TweenProperty(GetNode("Sprite2D"), "modulate", Colors.Red, 1.0f);
            tween.Parallel().TweenProperty(GetNode("Sprite2D"), "scale",
                GetNode<Sprite2D>("Sprite2D").Scale * 1.3f, 0.5f);
            
            tween.Parallel().TweenProperty(sprite.Material, "shader_parameter/dissolve_state", 0, 0.5f)
                .SetTrans(Tween.TransitionType.Quad)
                .SetEase(Tween.EaseType.Out);
            
            tween.TweenCallback(Callable.From(QueueFree));
        }
        
        public void Burn()
        {
            SignalManager.InvokeBurn();
            
            ShaderMaterial shaderMaterial = new ShaderMaterial();
            shaderMaterial.Shader = _shaders[1];
            shaderMaterial.SetShaderParameter("dissolve_value", 1.0f);
            shaderMaterial.SetShaderParameter("burn_size", 0.08f);
            shaderMaterial.SetShaderParameter("burn_color", GetRandomFireColor());
            shaderMaterial.SetShaderParameter("random_seed", GD.Randi());
            shaderMaterial.SetShaderParameter("offset_strength", 0.1f);
            shaderMaterial.SetShaderParameter("dissolve_texture", _dissolveTexture);
                
            sprite.Material = shaderMaterial;
            
            //this.Freeze = true;
            canDepleteHealth = true;
            SetDeferred("freeze", true);
            collisionShape.SetDeferred("disabled", true);
            
            if (sprite.Material == null || !(sprite.Material is ShaderMaterial))
            {
                GD.PrintErr("ShaderMaterial is not assigned to Sprite2D!");
                return;
            }
            
            Tween tween = GetTree().CreateTween();

            tween.TweenProperty(sprite.Material, "shader_parameter/dissolve_value", 0, 0.4f)
                .SetTrans(Tween.TransitionType.Circ)
                .SetEase(Tween.EaseType.Out);
            
            tween.TweenCallback(Callable.From(QueueFree));
        }

        private Color GetRandomFireColor()
        {
            Random rng = new Random();
            float variation = 0.1f; // Adjust this to control how much randomness is applied
            
            // Base fire color (orange)
            Color baseBurnColor = new Color("ff8f06");
            
            // Randomize individual channels
            float r = Mathf.Clamp(baseBurnColor.R + (float)(rng.NextDouble() * variation - variation / 2), 0, 1);
            float g = Mathf.Clamp(baseBurnColor.G + (float)(rng.NextDouble() * variation - variation / 2), 0, 1);
            float b = Mathf.Clamp(baseBurnColor.B - (float)(rng.NextDouble() * variation / 2), 0, 1);
            
            return new Color(r, g, b);
//             Random rng = new Random();
//
// // Generate random values for each color channel
//             float r = (float)rng.NextDouble();
//             float g = (float)rng.NextDouble();
//             float b = (float)rng.NextDouble();
//
//             return new Color(r, g, b);
        }

        private void OnAnimationFinished(StringName animname)
        {
            if (animname == "Explosion")
            {
                //animationPlayer.Play("RESET");
                QueueFree();
            }
        }

        private bool ShouldSpawnNewYoba(Yoba otherYoba)
        {
            // if(HasCollided)
            //     return false;
            // if(otherYoba.HasCollided)
            //     return false;

            return (GetInstanceId() > otherYoba.GetInstanceId() ||
                    (otherYoba.Sleeping && (GetInstanceId() < otherYoba.GetInstanceId())));
            // && (!HasCollided || !otherYoba.hasCollided);
        }

        private void OnScreenLeaving()
        {
            GD.Print($"YOBA ULETEL");
            SignalManager.InvokeLoseHealth();
            SignalManager.InvokeYobaFell();
            QueueFree();
        }

        private Vector2 CalculateMiddlePoint(RigidBody2D otherBody)
        {
            if (otherBody == null)
                return Vector2.Zero;

            // Return the midpoint between the current and other Yoba's positions
            return (GlobalPosition + otherBody.GlobalPosition) / 2.0f;
        }

        private void SpawnNewNode(Vector2 position, Yoba otherYoba)
        {
            //GD.Print("Trying spawn new yoba");
            var a = this.GetInstanceId();
            var b = otherYoba.GetInstanceId();
            SignalManager.InvokeYobasCollided(position, this, otherYoba);
        }

        private void _OnTimerTimeout()
        {
            // QueueFree();
            CallDeferred("QueueFree");
        }
        

        // Optionally allow setting the Yoba image and diameter in code
        public void SetYobaAppearance(string imagePath, float diameter)
        {
            ImagePath = imagePath;
            Diameter = diameter;

            // Set the texture and diameter based on the provided values
            if (!string.IsNullOrEmpty(ImagePath))
            {
                sprite.Texture = GD.Load<Texture2D>(ImagePath);
            }
            if (Diameter > 0)
            {
                circleShape.Radius = Diameter / 2; // Update radius
            }
        }

        public void EnableCollisionShape() => collisionShape.SetDisabled(false);
        public void DisableCollisionShape() => collisionShape.SetDisabled(true);
    }
}
