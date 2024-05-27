using DuchyOfThorns;
using Godot.Collections;

public partial class HealingPotion : TextureButton
{
	[Export] public Array<AnimatedSprite2D> sprites;
    protected Globals globals;
    public int pressCount { get; set; } = 0;


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

    public void UpdatePotion()
    {
        foreach (AnimatedSprite2D sprite in sprites)
        {
            sprite.Visible = false;
        }

        sprites[0].Visible = true;
    }
    private void OnPressed()
    {
        if (pressCount < 3)
        {
            foreach (AnimatedSprite2D sprite in sprites)
                sprite.Visible = false;
            pressCount++;
            sprites[pressCount].Visible = true;

            globals.EmitSignal("HealingPotion", 50);     
        }
    }
}
