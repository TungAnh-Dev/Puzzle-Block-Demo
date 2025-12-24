using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AdvancedRandomLogic : BlockRandomLogic
{
    [Header("Refs")]
    [SerializeField] private GridPlacementLogic placementLogic;

    [Header("Config")]
    [SerializeField] private int maxTry = 100;

    private void Reset()
    {
        placementLogic = FindFirstObjectByType<GridPlacementLogic>();
    }

    public override void Generate(
        int count,
        BlockShapeSO shapeSO,
        BlockThemeSO themeSO,
        out List<BlockShape> shapes,
        out List<int> themeIds
    )
    {
        for (int attempt = 0; attempt < maxTry; attempt++)
        {
            shapes   = PickRandomShapes(count, shapeSO);
            themeIds = PickDistinctThemes(count, themeSO);

            bool allPlaceable = true;
            bool hasClear     = false;

            for (int i = 0; i < count; i++)
            {
                BlockShape shape = shapes[i];

                if (!placementLogic.CanPlaceShapeAnywhere(shape))
                {
                    allPlaceable = false;
                    break;
                }

                if (!hasClear && placementLogic.CanClearLineAnywhere(shape))
                {
                    hasClear = true;
                }
            }

            if (allPlaceable && hasClear)
            {
                return;
            }
        }

        shapes   = PickRandomShapes(count, shapeSO);
        themeIds = PickDistinctThemes(count, themeSO);
    }


    private List<BlockShape> PickRandomShapes(int count, BlockShapeSO shapeSO)
    {
        var list = new List<BlockShape>(count);

        for (int i = 0; i < count; i++)
        {
            list.Add(
                shapeSO.Shapes[
                    Random.Range(0, shapeSO.Shapes.Count)
                ]
            );
        }

        return list;
    }

    private List<int> PickDistinctThemes(int count, BlockThemeSO themeSO)
    {
        var result = new List<int>(count);
        var temp   = new List<BlockThemeData>(themeSO.Sprites);

        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, temp.Count);
            result.Add(temp[idx].Id);
            temp.RemoveAt(idx);
        }

        return result;
    }
}
