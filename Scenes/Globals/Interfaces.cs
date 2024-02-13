namespace DuchyOfThorns;

// delegates
public delegate void RemovedFromSceneEventHandler(IPoolable source);

/// <summary>
/// Interface for objects which will be pooled and reused (Usually loot)
/// </summary>
public interface IPoolable
{
    event RemovedFromSceneEventHandler RemovedFromScene;
    void AddToScene();
    void RemoveFromScene();
}