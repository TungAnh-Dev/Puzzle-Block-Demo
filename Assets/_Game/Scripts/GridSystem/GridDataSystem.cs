using UnityEngine;

public class GridDataSystem : MonoBehaviour
{
    public const int Size = 8;

    [SerializeField] private Vector2 origin;
    [SerializeField] private float   cellSize = 1f;

    private bool[,]     occupied   = new bool[Size, Size];
    private BrickObj[,] gridBricks = new BrickObj[Size, Size];

    public bool IsInside(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < Size &&
               cell.y >= 0 && cell.y < Size;
    }

    public bool IsFree(Vector2Int cell)
    {
        return IsInside(cell) && !occupied[cell.x, cell.y];
    }

    public void Occupy(Vector2Int cell, BrickObj brick)
    {
        occupied[cell.x, cell.y]   = true;
        gridBricks[cell.x, cell.y] = brick;
    }

    public void ClearCell(Vector2Int cell)
    {
        if (!occupied[cell.x, cell.y]) return;

        var brick = gridBricks[cell.x, cell.y];
        if (brick != null)
            brick.gameObject.SetActive(false);

        gridBricks[cell.x, cell.y] = null;
        occupied[cell.x, cell.y]   = false;
    }

    public bool IsOccupied(Vector2Int cell)
    {
        return IsInside(cell) && occupied[cell.x, cell.y];
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - origin.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.y - origin.y) / cellSize);
        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(Vector2Int cell)
    {
        return new Vector3(
            origin.x + (cell.x + 0.5f) * cellSize,
            origin.y + (cell.y + 0.5f) * cellSize,
            0
        );
    }
    
    public void ClearAllData()
    {
        for (int x = 0; x < Size; x++)
        {
            for (int y = 0; y < Size; y++)
            {
                if (gridBricks[x, y] != null)
                {
                    gridBricks[x, y].gameObject.SetActive(false);
                    gridBricks[x, y].transform.SetParent(null);
                    gridBricks[x, y] = null;
                }

                occupied[x, y] = false;
            }
        }
    }
    
    #region Gizmos

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;

        for (int x = 0; x <= Size; x++)
        {
            Vector3 from = new Vector3(origin.x + x * cellSize, origin.y, 0);
            Vector3 to   = new Vector3(origin.x + x * cellSize, origin.y + Size * cellSize, 0);
            Gizmos.DrawLine(from, to);
        }

        for (int y = 0; y <= Size; y++)
        {
            Vector3 from = new Vector3(origin.x, origin.y + y * cellSize, 0);
            Vector3 to   = new Vector3(origin.x + Size * cellSize, origin.y + y * cellSize, 0);
            Gizmos.DrawLine(from, to);
        }
    }

    #endregion
}