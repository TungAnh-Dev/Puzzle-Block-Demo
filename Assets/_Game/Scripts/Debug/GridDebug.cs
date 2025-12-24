using UnityEngine;

public class GridDebug : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private BlockDragHandler dragHandler;

    [Header("Debug Prefab")]
    [SerializeField] private BrickDebug brickDebugPrefab;

    private BrickDebug[,] debugBricks;

    private void Awake()
    {
        if (this.levelManager == null)
        {
            Debug.LogError("[GridDebug] Missing GridSystem");
            enabled = false;
            return;
        }

        debugBricks = new BrickDebug[GridDataSystem.Size, GridDataSystem.Size];
        CreateDebugGrid();
        levelManager.OnReplayLevel += Refresh;
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
        for (int x = 0; x < GridDataSystem.Size; x++)
        {
            for (int y = 0; y < GridDataSystem.Size; y++)
            {
                Vector3 pos = this.levelManager.GridDataSystem.GridToWorld(new Vector2Int(x, y));

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
        for (int x = 0; x < GridDataSystem.Size; x++)
        {
            for (int y = 0; y < GridDataSystem.Size; y++)
            {
                bool occupied = this.levelManager.GridDataSystem.IsOccupied(new Vector2Int(x, y));

                debugBricks[x, y].SetBrickDebugType(
                    occupied
                        ? BrickDebug.BrickDebugType.Occupied
                        : BrickDebug.BrickDebugType.Unoccupied
                );
            }
        }
    }
}
