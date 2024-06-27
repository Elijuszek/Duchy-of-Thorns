using Godot.Collections;

namespace DuchyOfThorns;

public partial class HealingPotion : TextureButton
{
	[Export] public Array<AnimatedSprite2D> sprites;
    protected Globals globals;
    private int pressCount = 0;

    public override void _Ready()
    {
        globals = GetNode<Globals>("/root/Globals");

        foreach (AnimatedSprite2D sprite in sprites)
        {
            sprite.Visible = false;
            sprite.Play("default");
        }

        sprites[0].Visible = true;
    }

    private void OnPressed()
    {
        if (pressCount < 3)
        {
            sprites[pressCount].Visible = false;
            pressCount++;
            sprites[pressCount].Visible = true;

            globals.EmitSignal("HealingPotion", 50);     
        }
    }
}
