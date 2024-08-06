using Godot;
using System;

public partial class ScaledTextureRect : TextureRect
{

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
		Texture = newTexture;
		ReverseScale = Vector2.One / proportionalScale;
		Visible = true;
	}

}
