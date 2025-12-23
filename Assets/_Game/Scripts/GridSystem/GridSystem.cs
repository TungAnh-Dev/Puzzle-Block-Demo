using System;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private BlockThemeSO blockThemeSO;
    [SerializeField] private BrickObj     ghostBrickPrefab;
    [SerializeField] private BrickObj     gridBrickPrefab;
    [SerializeField] private Transform    brickHolder;
    [SerializeField] private Transform    brickPreviewHolder;
    public const             int          Size = 8;

    [Header("Grid")]
    [SerializeField] private Vector2 origin = Vector2.zero; 
    [SerializeField] private float cellSize = 1f;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;

    private          bool[,]        occupied    = new bool[Size, Size];
    private          BrickObj[,]    gridBricks  = new BrickObj[Size, Size];
    private readonly List<BrickObj> ghostBricks = new();
    
    private void Awake()
    {
        ClearGrid();
    }

    public void ClearGrid()
    {
        for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
                occupied[x, y] = false;
    }
    
    public bool IsInside(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < Size &&
               cell.y >= 0 && cell.y < Size;
    }

    public bool IsFree(Vector2Int cell)
    {
        return IsInside(cell) && !occupied[cell.x, cell.y];
    }

    public void Occupy(Vector2Int cell)
    {
        if (IsInside(cell))
            occupied[cell.x, cell.y] = true;
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
    
    public void PreviewBlock(BlockObj block)
    {
        ClearPreview();

        HashSet<Vector2Int> previewCells = new();
        List<Vector2Int>    cells        = new();

        foreach (var brick in block.Bricks)
        {
            Vector2Int cell = WorldToGrid(brick.transform.position);

            if (!IsFree(cell))
            {
                ClearPreview();
                return;
            }

            previewCells.Add(cell);
            cells.Add(cell);
        }

        int    themeId = block.GetThemeId();
        Sprite sprite  = blockThemeSO.GetById(themeId);

        // ===== Preview block cells =====
        foreach (var cell in cells)
        {
            SpawnGhost(cell, sprite, BrickObj.BrickType.Preview);
        }

        // ===== Check FULL ROW =====
        for (int y = 0; y < Size; y++)
        {
            bool full = true;
            for (int x = 0; x < Size; x++)
            {
                if (!WillOccupy(new Vector2Int(x, y), previewCells))
                {
                    full = false;
                    break;
                }
            }

            if (full)
            {
                for (int x = 0; x < Size; x++)
                {
                    Vector2Int c = new(x, y);
                    if (!previewCells.Contains(c))
                        SpawnGhost(c, sprite, BrickObj.BrickType.PreviewClear);
                }
            }
        }

        // ===== Check FULL COLUMN =====
        for (int x = 0; x < Size; x++)
        {
            bool full = true;
            for (int y = 0; y < Size; y++)
            {
                if (!WillOccupy(new Vector2Int(x, y), previewCells))
                {
                    full = false;
                    break;
                }
            }

            if (full)
            {
                for (int y = 0; y < Size; y++)
                {
                    Vector2Int c = new(x, y);
                    if (!previewCells.Contains(c))
                        SpawnGhost(c, sprite, BrickObj.BrickType.PreviewClear);
                }
            }
        }
    }
    
    private bool WillOccupy(Vector2Int cell, HashSet<Vector2Int> previewCells)
    {
        if (previewCells.Contains(cell))
            return true;

        return occupied[cell.x, cell.y];
    }
    
    private void SpawnGhost(
        Vector2Int cell,
        Sprite sprite,
        BrickObj.BrickType type
    )
    {
        Vector3 worldPos = GridToWorld(cell);

        var ghost = LazyPooling.Instant.getObj(ghostBrickPrefab);
        ghost.gameObject.SetActive(true);
        ghost.transform.SetParent(brickPreviewHolder);
        ghost.transform.position = worldPos;

        ghost.SetSprite(sprite);
        ghost.SetBrick(type);

        ghostBricks.Add(ghost);
    }

    
    public void ClearPreview()
    {
        for (int i = 0; i < ghostBricks.Count; i++)
        {
            ghostBricks[i].gameObject.SetActive(false);
        }
        ghostBricks.Clear();
    }
    
    public bool PlaceBlock(BlockObj block)
    {
        List<Vector2Int> cells = new();

        foreach (var brick in block.Bricks)
        {
            Vector2Int cell = WorldToGrid(brick.transform.position);
            if (!IsFree(cell))
                return false;

            cells.Add(cell);
        }

        int    themeId = block.GetThemeId();
        Sprite sprite  = blockThemeSO.GetById(themeId);

        foreach (var cell in cells)
        {
            var gridBrick = LazyPooling.Instant.getObj(gridBrickPrefab);
            gridBrick.gameObject.SetActive(true);
            gridBrick.transform.SetParent(brickHolder);
            gridBrick.transform.position = GridToWorld(cell);
            gridBrick.transform.localScale = Vector3.one;

            gridBrick.SetSprite(sprite);
            gridBrick.SetBrick(BrickObj.BrickType.Occupied);

            Occupy(cell);
            gridBricks[cell.x, cell.y] = gridBrick;
        }
        

        ClearPreview();
        CheckAndClearLines();
        
        return true;
    }
    
    public bool IsOccupied(Vector2Int cell)
    {
        if (!IsInside(cell))
            return false;

        return occupied[cell.x, cell.y];
    }
    
    private List<int> GetFullRows()
    {
        List<int> rows = new();

        for (int y = 0; y < Size; y++)
        {
            bool full = true;
            for (int x = 0; x < Size; x++)
            {
                if (!occupied[x, y])
                {
                    full = false;
                    break;
                }
            }

            if (full)
                rows.Add(y);
        }

        return rows;
    }
    
    private List<int> GetFullColumns()
    {
        List<int> cols = new();

        for (int x = 0; x < Size; x++)
        {
            bool full = true;
            for (int y = 0; y < Size; y++)
            {
                if (!occupied[x, y])
                {
                    full = false;
                    break;
                }
            }

            if (full)
                cols.Add(x);
        }

        return cols;
    }
    
    private void ClearRow(int row)
    {
        for (int x = 0; x < Size; x++)
        {
            if (!occupied[x, row])
                continue;

            BrickObj brick = gridBricks[x, row];
            if (brick != null)
            {
                brick.gameObject.SetActive(false);
                brick.transform.SetParent(null);
                gridBricks[x, row] = null;
            }

            occupied[x, row] = false;
        }
        
        //TODO: effect
    }

    private void ClearColumn(int col)
    {
        for (int y = 0; y < Size; y++)
        {
            if (!occupied[col, y])
                continue;

            BrickObj brick = gridBricks[col, y];
            if (brick != null)
            {
                brick.gameObject.SetActive(false);
                brick.transform.SetParent(null);
                gridBricks[col, y] = null;
            }

            occupied[col, y] = false;
        }
        //TODO: effect
    }
    
    public void CheckAndClearLines()
    {
        var rows = GetFullRows();
        var cols = GetFullColumns();

        foreach (var r in rows)
            ClearRow(r);

        foreach (var c in cols)
            ClearColumn(c);
    }






    #region Gizmos

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

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
