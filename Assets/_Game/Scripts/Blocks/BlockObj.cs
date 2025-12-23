using System.Collections.Generic;
using UnityEngine;

public class BlockObj : MonoBehaviour
{
    [SerializeField] private int            themeId;
    [SerializeField] private List<BrickObj> bricks = new List<BrickObj>();

    public IReadOnlyList<BrickObj> Bricks => bricks;

    public void SetTheme(int themeId)
    {
        this.themeId = themeId;
    }
    
    public int GetThemeId() => themeId;

    public void AddBrick(BrickObj brick)
    {
        if (!bricks.Contains(brick))
            bricks.Add(brick);
    }

    public void ClearBricks()
    {
        bricks.Clear();
    }

    public void Select()
    {
        transform.localScale = Vector3.one;
    }

    public void UnSelect()
    {
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }
}