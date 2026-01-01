using Godot;
using System;
using YobaGame.Models;
using YobaGame.Models.Managers;

public partial class ExplodedSprite : Node2D
{
    private Sprite2D sprite;
    
    private AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        // Create a Sprite2D node dynamically
        sprite = new Sprite2D();
        AddChild(sprite); // Add it as a child of this node
        
        animationPlayer = new AnimationPlayer();
        AddChild(animationPlayer);

        // Create and configure the Explosion animation
        var animation = new Animation();
        animation.Length = 1.0f;

        var animationLibrary = new AnimationLibrary();
        animationPlayer.AddAnimationLibrary("Explosion",CreateExplosionAnimation());

        // Optional: Add fading effect
        //animationPlayer.Add
    }
    
    private AnimationLibrary CreateExplosionAnimation()
    {
        var animationLibrary = new AnimationLibrary();
        var animation = new Animation
        {
            Length = 1.0f, // Set animation length to 1 second
            LoopMode = Animation.LoopModeEnum.None
        };

        // Add a value track for the Sprite2D's `scale` property
        int scaleTrackId = animation.AddTrack(Animation.TrackType.Value);
        animation.TrackSetPath(scaleTrackId, "Sprite2D:scale");

        // Define keyframes for scaling the sprite
        animation.TrackInsertKey(scaleTrackId, 0.0f, Vector2.One);          // Start scale
        animation.TrackInsertKey(scaleTrackId, 1.0f, new Vector2(2, 2));   // End scale (double size)

        // Add a value track for the Sprite2D's `modulate` property (optional for fade-out)
        int modulateTrackId = animation.AddTrack(Animation.TrackType.Value);
        animation.TrackSetPath(modulateTrackId, "Sprite2D:modulate");

        // Define keyframes for fading out
        animation.TrackInsertKey(modulateTrackId, 0.0f, Colors.White);     // Fully visible
        animation.TrackInsertKey(modulateTrackId, 1.0f, Colors.Transparent); // Fully transparent

        animationLibrary.AddAnimation("Explosion", animation);
        
        return animationLibrary;
    }

    public void SetSprite(Sprite2D sourceSprite)
    {
        // Duplicate the sprite texture and assign it to the new sprite
        if (sourceSprite != null)
        {
            sprite.Texture = sourceSprite.Texture;
            sprite.Centered = sourceSprite.Centered;
            sprite.Scale = sourceSprite.Scale;
        }
    }
    
    public void PlayExplosionAnimation()
    {
        // Play the "Explosion" animation if it exists
        if (animationPlayer.HasAnimation("Explosion"))
        {
            animationPlayer.Play("Explosion");
        }
    }
    
}
