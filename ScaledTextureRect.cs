using Godot;
using System;

public partial class ScaledTextureRect : TextureRect
{
	[Signal]
	public delegate void RescaledEventHandler();

	public Vector2 CurrentScale {get; set;}
	public Vector2 ReverseScale {get; set;}

	public void ApplyAndScaleTexture(Texture2D newTexture)
	{
		Control parent = (Control)GetParent();
		Vector2 parentSize = parent.Size;
		Vector2 textureSize = newTexture.GetSize();
		Vector2 scaleFactors = parentSize / textureSize;
		float scaleFactor = Math.Min(scaleFactors.X, scaleFactors.Y);
		Vector2 proportionalScale = new Vector2(scaleFactor, scaleFactor);

		ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
		StretchMode = TextureRect.StretchModeEnum.Scale;

		CustomMinimumSize = textureSize * proportionalScale;
		if (Texture != null)
		{
			var oldTexture = Texture;
			Texture = null;
			oldTexture.Free();
		}
		Texture = newTexture;

		CurrentScale = proportionalScale;
		ReverseScale = Vector2.One / proportionalScale;
		Visible = true;
	}

	

}
