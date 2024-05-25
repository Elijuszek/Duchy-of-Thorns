namespace DuchyOfThorns;

/// <summary>
/// Class for handling various calculations
/// </summary>
public static partial class Utilities
{
    public static float GetRandomFloat(float min, float max)
    {
        Random rand = new Random();
        float range = max - min;
        double sample = rand.NextDouble();
        double scaled = (sample * range) + min;
        return (float)scaled;
    }

    public static Vector2 GetRandomPositionInArea(CollisionShape2D collisionShape)
    {
        Vector2 extents = collisionShape.Shape.GetRect().Size;
        Vector2 topLeft = collisionShape.GlobalPosition - (extents / 2);
        float x = GetRandomFloat(topLeft.X, topLeft.X + extents.X);
        float y = GetRandomFloat(topLeft.Y, topLeft.Y + extents.Y);
        return new Vector2(x, y);
    }

    public static Vector2 GetRandomPositionInArea(Vector2 topLeft, Vector2 extents)
    {
        float x = GetRandomFloat(topLeft.X, topLeft.X + extents.X);
        float y = GetRandomFloat(topLeft.Y, topLeft.Y + extents.Y);
        return new Vector2(x, y);
    }

    public static bool Chance(int chance = 50)
    {
        return new Random().Next(0, 100) <= chance;
    }
}
