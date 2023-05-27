namespace DuchyOfThorns;

/// <summary>
/// Class for getting shortest path between two points using A* algorithm
/// </summary>
public partial class Pathfinding : Node2D
{
    [Export] private Color enabledColor;
    [Export] private Color disabledColor;
    [Export] private bool displayDebuggingGrid = true;
    private Node2D grid;
    private ColorRect[] gridRects;
    AStar2D astar = new AStar2D();
    TileMap tilemap;
    Vector2 halfCellSize;
    Rect2 usedRect;
    public override void _Ready() => grid = GetNode<Node2D>("Grid");
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        UpdateNavigatioMap();
    }
    public void CreateNavigationMap(TileMap tilemap)
    {
        this.tilemap = tilemap;
        halfCellSize = tilemap.TileSet.TileSize / 2;
        usedRect = tilemap.GetUsedRect();
        Vector2I[] tiles = tilemap.GetUsedCells(0).OfType<Vector2I>().ToArray();
        gridRects = new ColorRect[tiles.Length];
        AddTraversableTiles(tiles);
        ConnectTraversableTiles(tiles);
    }
    private void AddTraversableTiles(Vector2I[] tiles)
    {
        foreach (Vector2I tile in tiles)
        {
            int id = GetIdForPoint(tile);
            astar.AddPoint(id, tile);

            //Debugging Grid
            if (displayDebuggingGrid)
            {
                ColorRect rect = new ColorRect();
                grid.AddChild(rect);
                gridRects[id] = rect;
                rect.Modulate = new Color(1, 1, 1, 0.3f);
                rect.Color = enabledColor;
                rect.MouseFilter = Control.MouseFilterEnum.Ignore;
                rect.Size = tilemap.TileSet.TileSize;
                rect.Position = tilemap.MapToLocal(tile) - halfCellSize; // GODOT4
            }
            //Debugging Grid
        }
    }
    private void ConnectTraversableTiles(Vector2I[] tiles)
    {
        TileMap walls = GetParent().GetNode<TileMap>("Walls");
        Vector2I[] wallTiles = walls.GetUsedCells(0).OfType<Vector2I>().ToArray();
        foreach (Vector2I wallTile in wallTiles)
        {
            int id = GetIdForPoint(wallTile);
            TileData tileData = walls.GetCellTileData(0, wallTile);
            foreach (Vector2I c in (Vector2[])tileData.GetCustomData("CanWalk"))
            {
                Vector2I target = wallTile + c;
                int targetID = GetIdForPoint(target);
                astar.ConnectPoints(id, targetID);
            }
        }
        foreach (Vector2I tile in tiles)
        {
            int id = GetIdForPoint(tile);
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    Vector2I target = tile + new Vector2I(x - 1, y - 1);
                    int targetID = GetIdForPoint(target);

                    // TODO performance / loading time
                    if (tile == target || !astar.HasPoint(targetID) || wallTiles.Contains(target) || wallTiles.Contains(tile))
                    {
                        continue;
                    }
                    astar.ConnectPoints(id, targetID, false);
                }
            }
        }
    }
    private void UpdateNavigatioMap()
    {
        foreach (int point in astar.GetPointIds())
        {
            astar.SetPointDisabled(point, false);

            // Debugging grid
            if (displayDebuggingGrid)
            {
                gridRects[point].Color = enabledColor;
            }
            // Debugging grid
        }

        var obstacles = GetTree().GetNodesInGroup("obstacles");
        foreach (var obstacle in obstacles)
        {
            // TODO static tiles update every frame
            if (obstacle is TileMap temp)
            {
                var tiles = temp.GetUsedCells(0);
                foreach (Vector2I tile in tiles)
                {
                    int id = GetIdForPoint(tile);
                    if (astar.HasPoint(id))
                    {
                        astar.SetPointDisabled(id, true);

                        // Debugging grid
                        if (displayDebuggingGrid)
                        {
                            gridRects[id].Color = disabledColor;
                        }
                        // Debugging grid
                    }
                }
            }
            else if (obstacle is Actor actor)
            {
                Vector2I tileCord = tilemap.LocalToMap(actor.GlobalPosition);
                int id = GetIdForPoint(tileCord);
                if (astar.HasPoint(id))
                {
                    astar.SetPointDisabled(id, true);
                }

                // Debugging grid
                if (displayDebuggingGrid)
                {
                    gridRects[id].Color = disabledColor;
                }
                // Debugging grid
            }

        }
    }
    private int GetIdForPoint(Vector2I point)
    {
        var x = point.X - usedRect.Position.X;
        var y = point.Y - usedRect.Position.Y;
        return (int)(x + y * usedRect.Size.X);
    }
    public Vector2[] GetNewPath(Vector2 start, Vector2 end)
    {
        Vector2I startTile = tilemap.LocalToMap(start);
        Vector2I endTile = tilemap.LocalToMap(end);
        int startID = GetIdForPoint(startTile);
        int endID = GetIdForPoint(endTile);
        if (!astar.HasPoint(startID) || !astar.HasPoint(endID))
        {
            return new Vector2[] { };
        }
        Vector2[] pathMap = astar.GetPointPath(startID, endID);
        for (int i = 0; i < pathMap.Length; i++)
        {
            pathMap[i] = tilemap.MapToLocal((Vector2I)pathMap[i]);
        }
        return pathMap;
    }
    public Vector2[] GetEngagePath(Vector2 start, Vector2 end)
    {
        Vector2I startTile = tilemap.LocalToMap(start);
        Vector2I endTile = tilemap.LocalToMap(end);
        int startID = GetIdForPoint(startTile);
        int endID = GetIdForPoint(endTile);
        if (!astar.HasPoint(startID) || !astar.HasPoint(endID))
        {
            return new Vector2[] { };
        }
        astar.SetPointDisabled(endID, false);
        Vector2[] pathMap = astar.GetPointPath(startID, endID);
        for (int i = 0; i < pathMap.Length; i++)
        {
            pathMap[i] = tilemap.MapToLocal((Vector2I)pathMap[i]);
        }
        return pathMap;
    }
}