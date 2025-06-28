using Microsoft.Xna.Framework;

namespace StardewCamera;

public class CameraViewport
{
    public float X { get; set; }

    public float Y { get; set; }

    public float Width { get; set; }

    public float Height { get; set; }

    public Vector2 Position { get => new(X, Y); set { X = value.X; Y = value.Y; } }

    public float Left => X;

    public float Top => Y;

    public float Right => X + Width;

    public float Bottom => Y + Height;

    public Vector2 Center => new(X + (Width / 2), Y + (Height / 2));


    public CameraViewport(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public Vector2 ToCameraSpace(Vector2 position) => new(position.X - X, position.Y - Y);
}
