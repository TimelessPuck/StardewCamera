using Microsoft.Xna.Framework;

namespace StardewCamera;

public struct CameraViewport
{
    public float X { get; set; }

    public float Y { get; set; }

    public float Width { get; set; }

    public float Height { get; set; }

    public Vector2 Position { readonly get => new(X, Y); set { X = value.X; Y = value.Y; } }

    public readonly float Left => X;

    public readonly float Top => Y;

    public readonly float Right => X + Width;

    public readonly float Bottom => Y + Height;

    public readonly Vector2 Center => new(X + (Width / 2), Y + (Height / 2));


    public CameraViewport(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public readonly Vector2 ToCameraSpace(Vector2 position) => new(position.X - X, position.Y - Y);
}
