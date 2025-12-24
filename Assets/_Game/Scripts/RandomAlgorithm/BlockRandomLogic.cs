using System.Collections.Generic;
using UnityEngine;

public abstract class BlockRandomLogic : MonoBehaviour
{
    public abstract void Generate(
        int count,
        BlockShapeSO shapeSO,
        BlockThemeSO themeSO,
        out List<BlockShape> shapes,
        out List<int> themeIds
    );
}