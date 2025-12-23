using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct BlockShape
{
    public int          ID;
    public Vector2Int[] Cells;
}


[System.Serializable]
public class BlockShapeJson
{
    public int              id;
    public List<Vector2Int> cells;
}

[System.Serializable]
public class BlockShapeJsonFile
{
    public List<BlockShapeJson> shapes = new();
}