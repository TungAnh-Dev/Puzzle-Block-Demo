using UnityEngine;

public class GridDebug : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private BlockDragHandler dragHandler;

    [Header("Debug Prefab")]
    [SerializeField] private BrickDebug brickDebugPrefab;

    private BrickDebug[,] debugBricks;

    private void Awake()
    {
        if (gridSystem == null)
        {
            Debug.LogError("[GridDebug] Missing GridSystem");
            enabled = false;
            return;
        }

        debugBricks = new BrickDebug[GridSystem.Size, GridSystem.Size];
        CreateDebugGrid();
    }

    private void OnEnable()
    {
        if (dragHandler != null)
            dragHandler.OnBlockPlaced += Refresh;
    }

    private void OnDisable()
    {
        if (dragHandler != null)
            dragHandler.OnBlockPlaced -= Refresh;
    }

    // ================= CREATE =================

    private void CreateDebugGrid()
    {
        for (int x = 0; x < GridSystem.Size; x++)
        {
            for (int y = 0; y < GridSystem.Size; y++)
            {
                Vector3 pos = gridSystem.GridToWorld(new Vector2Int(x, y));

                BrickDebug brick = Instantiate(
                    brickDebugPrefab,
                    pos,
                    Quaternion.identity,
                    transform
                );

                brick.name = $"DebugCell_{x}_{y}";
                brick.SetBrickDebugType(BrickDebug.BrickDebugType.Unoccupied);

                debugBricks[x, y] = brick;
            }
        }
    }

    // ================= REFRESH =================

    private void Refresh()
    {
        for (int x = 0; x < GridSystem.Size; x++)
        {
            for (int y = 0; y < GridSystem.Size; y++)
            {
                bool occupied = gridSystem.IsOccupied(new Vector2Int(x, y));

                debugBricks[x, y].SetBrickDebugType(
                    occupied
                        ? BrickDebug.BrickDebugType.Occupied
                        : BrickDebug.BrickDebugType.Unoccupied
                );
            }
        }
    }
}
