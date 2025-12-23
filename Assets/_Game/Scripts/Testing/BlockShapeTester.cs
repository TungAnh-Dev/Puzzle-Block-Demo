using UnityEngine;

public class BlockShapeTester : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private BlockShapeSO blockShapeSO;
    [SerializeField] private BlockThemeSO blockThemeSO;

    [Header("View")]
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private int shapesPerRow = 6;
    [SerializeField] private float spacing = 2f;

    private void Start()
    {
        if (blockShapeSO == null)
        {
            Debug.LogError("BlockShapeTester: Missing BlockShapeSO");
            return;
        }

        SpawnAll();
    }

    [ContextMenu("Spawn All Shapes")]
    private void SpawnAll()
    {
        Clear();

        var shapes = blockShapeSO.Shapes;
        if (shapes == null || shapes.Count == 0)
        {
            Debug.LogError("BlockShapeTester: No shapes loaded");
            return;
        }

        Sprite sprite = blockThemeSO != null
            ? blockThemeSO.GetRandom()
            : null;

        for (int i = 0; i < shapes.Count; i++)
        {
            int row = i / shapesPerRow;
            int col = i % shapesPerRow;

            Vector3 rootPos = new Vector3(
                col * spacing,
                -row * spacing,
                0
            );

            SpawnShape(shapes[i], rootPos, sprite);
        }
    }

    private void SpawnShape(BlockShape shape, Vector3 position, Sprite sprite)
    {
        var root = new GameObject($"BlockShape_ID_{shape.ID}");
        root.transform.SetParent(transform);
        root.transform.position = position;

        foreach (var cell in shape.Cells)
        {
            var go = new GameObject($"Cell_{cell.x}_{cell.y}");
            go.transform.SetParent(root.transform);
            go.transform.localPosition =
                new Vector3(cell.x * cellSize, cell.y * cellSize, 0);

            if (sprite != null)
            {
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
            }
        }
    }

    private void Clear()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);
    }
}
