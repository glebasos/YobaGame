using Godot;
using System;

public partial class HeartContainer : TextureRect
{
	[Export] public Texture2D FullTexture { get; set; }
	[Export] public Texture2D DepletedTexture { get; set; }

	[Export] public Vector2 MinimumSize { get; set; } = new Vector2(32, 32);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.SetCustomMinimumSize(MinimumSize);
		this.Size = this.MinimumSize;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void SetState(bool isFull)
	{
		Texture = isFull ? FullTexture : DepletedTexture;
	}
}
