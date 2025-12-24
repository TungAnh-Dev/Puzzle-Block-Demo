using System.Collections.Generic;
using UnityEngine;

public class GridPlacementLogic : BlockPlacementLogic
{
    [SerializeField] private GridDataSystem grid;
    [SerializeField] private BlockThemeSO blockThemeSO;
    [SerializeField] private BrickObj ghostBrickPrefab;
    [SerializeField] private BrickObj gridBrickPrefab;
    [SerializeField] private Transform brickHolder;
    [SerializeField] private Transform previewHolder;

    private readonly List<BrickObj> ghostBricks = new();

    public override void PreviewBlock(BlockObj block)
    {
        ClearPreview();

        HashSet<Vector2Int> previewCells = new();

        foreach (var brick in block.Bricks)
        {
            Vector2Int cell = grid.WorldToGrid(brick.transform.position);
            if (!grid.IsFree(cell))
                return;

            previewCells.Add(cell);
        }

        Sprite sprite = blockThemeSO.GetById(block.GetThemeId());

        foreach (var cell in previewCells)
            SpawnGhost(cell, sprite, BrickObj.BrickType.Preview);

        this.PreviewRow(previewCells, sprite);
        this.PreviewCol(previewCells, sprite);
    }

    private void PreviewRow(HashSet<Vector2Int> previewCells, Sprite sprite)
    {
        for (int y = 0; y < GridDataSystem.Size; y++)
        {
            bool full = true;
            for (int x = 0; x < GridDataSystem.Size; x++)
            {
                Vector2Int c = new(x, y);
                if (!previewCells.Contains(c) && !this.grid.IsOccupied(c))
                {
                    full = false;
                    break;
                }
            }

            if (full)
                for (int x = 0; x < GridDataSystem.Size; x++)
                    this.SpawnGhost(new Vector2Int(x, y), sprite, BrickObj.BrickType.PreviewClear);
        }
    }

    private void PreviewCol(HashSet<Vector2Int> previewCells, Sprite sprite)
    {
        for (int x = 0; x < GridDataSystem.Size; x++)
        {
            bool full = true;
            for (int y = 0; y < GridDataSystem.Size; y++)
            {
                Vector2Int c = new(x, y);
                if (!previewCells.Contains(c) && !this.grid.IsOccupied(c))
                {
                    full = false;
                    break;
                }
            }

            if (full)
            {
                for (int y = 0; y < GridDataSystem.Size; y++)
                    this.SpawnGhost(new Vector2Int(x, y), sprite, BrickObj.BrickType.PreviewClear);
            }
        }
    }

    public override bool PlaceBlock(BlockObj block)
    {
        List<Vector2Int> cells = new();

        foreach (var brick in block.Bricks)
        {
            Vector2Int cell = grid.WorldToGrid(brick.transform.position);
            if (!grid.IsFree(cell))
                return false;

            cells.Add(cell);
        }

        Sprite sprite = blockThemeSO.GetById(block.GetThemeId());

        foreach (var cell in cells)
        {
            var brick = LazyPooling.Instant.getObj(gridBrickPrefab);
            brick.gameObject.SetActive(true);
            brick.transform.SetParent(brickHolder);
            brick.transform.position = grid.GridToWorld(cell);
            brick.transform.localScale = Vector3.one;

            brick.SetSprite(sprite);
            brick.SetBrick(BrickObj.BrickType.Occupied);

            grid.Occupy(cell, brick);
        }

        ClearPreview();
        ClearLines();
        return true;
    }

    private void ClearLines()
    {
        HashSet<Vector2Int> cellsToClear = new();

        // ===== SCAN ROW =====
        for (int y = 0; y < GridDataSystem.Size; y++)
        {
            bool full = true;
            for (int x = 0; x < GridDataSystem.Size; x++)
            {
                if (!grid.IsOccupied(new Vector2Int(x, y)))
                {
                    full = false;
                    break;
                }
            }

            if (full)
            {
                for (int x = 0; x < GridDataSystem.Size; x++)
                    cellsToClear.Add(new Vector2Int(x, y));
            }
        }

        // ===== SCAN COLUMN =====
        for (int x = 0; x < GridDataSystem.Size; x++)
        {
            bool full = true;
            for (int y = 0; y < GridDataSystem.Size; y++)
            {
                if (!grid.IsOccupied(new Vector2Int(x, y)))
                {
                    full = false;
                    break;
                }
            }

            if (full)
            {
                for (int y = 0; y < GridDataSystem.Size; y++)
                    cellsToClear.Add(new Vector2Int(x, y));
            }
        }

        // ===== CLEAR ALL AT ONCE =====
        foreach (var cell in cellsToClear)
            grid.ClearCell(cell);
    }


    private void SpawnGhost(Vector2Int cell, Sprite sprite, BrickObj.BrickType type)
    {
        var ghost = LazyPooling.Instant.getObj(ghostBrickPrefab);
        ghost.gameObject.SetActive(true);
        ghost.transform.SetParent(previewHolder);
        ghost.transform.position = grid.GridToWorld(cell);

        ghost.SetSprite(sprite);
        ghost.SetBrick(type);
        ghostBricks.Add(ghost);
    }

    public override void ClearPreview()
    {
        foreach (var g in ghostBricks)
            g.gameObject.SetActive(false);
        ghostBricks.Clear();
    }

    public override bool CanPlaceBlockAnywhere(BlockObj block)
    {
        foreach (var cell in GetAllGridCells())
        {
            if (CanPlaceBlockAt(block, cell))
                return true;
        }

        return false;
    }

    
    private bool CanPlaceBlockAt(BlockObj block, Vector2Int originCell)
    {
        foreach (var brick in block.Bricks)
        {
            Vector2Int offset = brick.GridOffset; 
            Vector2Int cell   = originCell + offset;

            if (!grid.IsInside(cell))
                return false;

            if (grid.IsOccupied(cell))
                return false;
        }

        return true;
    }
    
    private IEnumerable<Vector2Int> GetAllGridCells()
    {
        for (int x = 0; x < GridDataSystem.Size; x++)
        for (int y = 0; y < GridDataSystem.Size; y++)
            yield return new Vector2Int(x, y);
    }
    
    public bool CanPlaceShapeAnywhere(BlockShape shape)
    {
        foreach (var origin in GetAllGridCells())
        {
            if (CanPlaceShapeAt(shape, origin))
                return true;
        }

        return false;
    }
    
    private bool CanPlaceShapeAt(BlockShape shape, Vector2Int origin)
    {
        foreach (var offset in shape.Cells)
        {
            Vector2Int cell = origin + offset;

            if (!grid.IsInside(cell))
                return false;

            if (grid.IsOccupied(cell))
                return false;
        }

        return true;
    }
    
    public bool CanClearLineAnywhere(BlockShape shape)
    {
        foreach (var origin in GetAllGridCells())
        {
            if (!CanPlaceShapeAt(shape, origin))
                continue;

            if (WillClearLine(shape, origin))
                return true;
        }

        return false;
    }
    
    private bool WillClearLine(BlockShape shape, Vector2Int origin)
    {
        HashSet<Vector2Int> tempOccupied = new();

        for (int x = 0; x < GridDataSystem.Size; x++)
        for (int y = 0; y < GridDataSystem.Size; y++)
            if (grid.IsOccupied(new Vector2Int(x, y)))
                tempOccupied.Add(new Vector2Int(x, y));

        foreach (var offset in shape.Cells)
            tempOccupied.Add(origin + offset);

        // check row
        for (int y = 0; y < GridDataSystem.Size; y++)
        {
            bool full = true;
            for (int x = 0; x < GridDataSystem.Size; x++)
            {
                if (!tempOccupied.Contains(new Vector2Int(x, y)))
                {
                    full = false;
                    break;
                }
            }
            if (full) return true;
        }

        // check column
        for (int x = 0; x < GridDataSystem.Size; x++)
        {
            bool full = true;
            for (int y = 0; y < GridDataSystem.Size; y++)
            {
                if (!tempOccupied.Contains(new Vector2Int(x, y)))
                {
                    full = false;
                    break;
                }
            }
            if (full) return true;
        }

        return false;
    }





}
