using System.Collections.Concurrent;

namespace DuchyOfThorns;

/// <summary>
/// Dynamic object pool increases performance by storing and reusing objects
/// in this way cost to add new objects to the scene is dramatically reduced.
/// Only use Node objects which can be instantiated independently and are 
/// not dependent on other objects. Objects must have "RemovedFromScene" signal
/// </summary>
/// <typeparam name="T">Any Node with IPoolable interface</typeparam>
public partial class ObjectPool<T> where T : Node, IPoolable, new()
{
    private readonly ConcurrentBag<T> pool;
    private Node root;
    private PackedScene nodeScene;
    private int size;
    public ObjectPool(Node parent, PackedScene packedScene, int startingsize)
    {
        pool = new ConcurrentBag<T>();
        root = parent;
        nodeScene = packedScene;
        size = startingsize;
        Expand(size);
    }

    public void ReleasedFromScene(IPoolable source)
    {
        pool.Add((T)source);
    }
    public T Take()
    {
        if (pool.IsEmpty)
        {
            Expand(size * 2);
        }
        T item;
        if (pool.TryTake(out item))
        {
            item.AddToScene();
            return item;
        }
        GD.PushError("Failed to take item from {0} pool", GetType());
        return null;

    }
    public void Expand(int count)
    {
        for (int i = 0; i < count; i++) 
        {
            T item = nodeScene.Instantiate<T>();
            item.RemovedFromScene += ReleasedFromScene;
            root.AddChild(item);
            item.RemoveFromScene();
        }
        size += count;
    }
}
