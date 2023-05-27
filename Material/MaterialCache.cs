namespace DuchyOfThorns;

/// <summary>
/// TODO: might not be needed
/// class for caching materials and shaders
/// </summary>
public partial class MaterialCache : CanvasLayer
{
    ParticleProcessMaterial[] particales = {
            ResourceLoader.Load<ParticleProcessMaterial>("res://Material/Particles/ParticalesMaterial/Blood.tres"),
            ResourceLoader.Load<ParticleProcessMaterial>("res://Material/Particles/ParticalesMaterial/Embers.tres"),
            ResourceLoader.Load<ParticleProcessMaterial>("res://Material/Particles/ParticalesMaterial/Fire.tres"),
            ResourceLoader.Load<ParticleProcessMaterial>("res://Material/Particles/ParticalesMaterial/FireplaceEmbers.tres"),
            ResourceLoader.Load<ParticleProcessMaterial>("res://Material/Particles/ParticalesMaterial/FireplaceFire.tres")
    };
    ShaderMaterial[] shaders = {

        ResourceLoader.Load<ShaderMaterial>("res://Material/Shaders/Transitions/PixelationTransition.tres")
    };
    public override void _Ready()
    {
        foreach (ParticleProcessMaterial material in particales)
        {
            GpuParticles2D particles = new GpuParticles2D();
            particles.ProcessMaterial = material;
            particles.OneShot = true;
            particles.Modulate = (new Color(1, 1, 1, 0));
            particles.Emitting = true;
            this.AddChild(particles);
        }
        foreach (ShaderMaterial material in shaders)
        {
            Sprite2D sprite = new Sprite2D();
            sprite.Material = material;
            this.AddChild(sprite);
            sprite.Material.Set("shader_param/pixelFactor", 0.01f);
            sprite.QueueFree();
        }
    }
}
