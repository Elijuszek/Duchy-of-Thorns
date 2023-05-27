namespace DuchyOfThorns;

/// <summary>
/// Interface for objects which will be pooled and reused (Usually loot)
/// </summary>
public interface IPoolable
{
    void AddToScene();
    void RemoveFromScene();
}