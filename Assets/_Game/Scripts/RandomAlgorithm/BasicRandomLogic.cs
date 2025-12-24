using System.Collections.Generic;
using UnityEngine;

public class BasicRandomLogic : BlockRandomLogic
{
    public override void Generate(
        int count,
        BlockShapeSO shapeSO,
        BlockThemeSO themeSO,
        out List<BlockShape> shapes,
        out List<int> themeIds
    )
    {
        shapes   = new List<BlockShape>(count);
        themeIds = new List<int>(count);

        for (int i = 0; i < count; i++)
        {
            shapes.Add(
                shapeSO.Shapes[Random.Range(0, shapeSO.Shapes.Count)]
            );
        }

        var temp = new List<BlockThemeData>(themeSO.Sprites);
        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, temp.Count);
            themeIds.Add(temp[idx].Id);
            temp.RemoveAt(idx);
        }
    }
}