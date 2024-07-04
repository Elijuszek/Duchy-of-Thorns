using Godot.Collections;
namespace DuchyOfThorns;

public partial class EnterableBuilding : StaticBody2D
{
    [Export] private TileMap tilemap;

    private Color visibleColor = new Color(1, 1, 1, 1f);
    private Color invisibleColor = new Color(1, 1, 1, 0f);

    private void EnteredBuilding(Node2D body)
    {
        if (body is Player)
        {
            Tween tween = CreateTween().SetParallel();
            tween.TweenProperty(tilemap, "layer_1/modulate", invisibleColor, 0.5d);
            tween.TweenProperty(tilemap, "layer_2/modulate", invisibleColor, 0.5d);
            tween.TweenProperty(tilemap, "layer_3/modulate", invisibleColor, 0.5d);
        }
    }

    private void ExitedBuilding(Node2D body)
    {
        if (body is Player)
        {
            Tween tween = CreateTween().SetParallel();
            tween.TweenProperty(tilemap, "layer_1/modulate", visibleColor, 0.5d);
            tween.TweenProperty(tilemap, "layer_2/modulate", visibleColor, 0.5d);
            tween.TweenProperty(tilemap, "layer_3/modulate", visibleColor, 0.5d);
        }
    }
}
