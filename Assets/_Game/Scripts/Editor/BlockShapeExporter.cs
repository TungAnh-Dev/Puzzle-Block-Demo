#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class BlockShapeGridEditor : EditorWindow
{
    public int gridSize = 5;     
    public int cellSize = 56;     
    public int shapeID  = 0;

    private bool[,] grid;

    private const string FolderPath = "Assets/_Game/Resources";
    private const string FilePath   = "Assets/_Game/Resources/block_shapes.json";

    [MenuItem("Tools/Block Shape Grid Editor")]
    public static void Open()
    {
        GetWindow<BlockShapeGridEditor>("Block Shape Grid");
    }

    private void OnEnable()
    {
        InitGrid();
    }

    private void InitGrid()
    {
        grid = new bool[gridSize, gridSize];
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Block Shape Grid Editor", EditorStyles.boldLabel);

        shapeID  = EditorGUILayout.IntField("Shape ID", shapeID);
        gridSize = EditorGUILayout.IntField("Grid Size", gridSize);
        cellSize = EditorGUILayout.IntSlider("Cell Size", cellSize, 32, 96);

        if (GUILayout.Button("Resize Grid"))
            InitGrid();

        GUILayout.Space(10);

        DrawGrid();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear"))
            InitGrid();

        if (GUILayout.Button("Export"))
            Export();
        GUILayout.EndHorizontal();
    }

    private void DrawGrid()
    {
        for (int y = gridSize - 1; y >= 0; y--)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < gridSize; x++)
            {
                GUI.backgroundColor = grid[x, y] ? Color.green : Color.gray;

                if (GUILayout.Button(
                        "",
                        GUILayout.Width(cellSize),
                        GUILayout.Height(cellSize)))
                {
                    grid[x, y] = !grid[x, y];
                }
            }
            GUILayout.EndHorizontal();
        }

        GUI.backgroundColor = Color.white;
    }

    private void Export()
    {
        var cells = new List<Vector2Int>();

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (grid[x, y])
                    cells.Add(new Vector2Int(x, y));
            }
        }

        if (cells.Count == 0)
        {
            Debug.LogError("BlockShapeGridEditor: No cell selected");
            return;
        }

        Normalize(cells);

        var file = LoadOrCreateFile();

        file.shapes.RemoveAll(s => s.id == shapeID);

        file.shapes.Add(new BlockShapeJson
        {
            id    = shapeID,
            cells = cells
        });
        
        shapeID++;

        File.WriteAllText(FilePath, JsonUtility.ToJson(file, true));
        AssetDatabase.Refresh();

        Debug.Log($"Exported Shape ID {shapeID - 1} → {FilePath}");
    }

    private BlockShapeJsonFile LoadOrCreateFile()
    {
        if (!AssetDatabase.IsValidFolder(FolderPath))
        {
            AssetDatabase.CreateFolder("Assets/_Game", "Resources");
            AssetDatabase.Refresh();
        }

        if (!File.Exists(FilePath))
        {
            var empty = new BlockShapeJsonFile();
            File.WriteAllText(FilePath, JsonUtility.ToJson(empty, true));
            AssetDatabase.Refresh();

            Debug.Log("Created new block_shapes.json");
            return empty;
        }

        return JsonUtility.FromJson<BlockShapeJsonFile>(
            File.ReadAllText(FilePath)
        );
    }

    private void Normalize(List<Vector2Int> cells)
    {
        int minX = int.MaxValue;
        int minY = int.MaxValue;

        foreach (var c in cells)
        {
            if (c.x < minX) minX = c.x;
            if (c.y < minY) minY = c.y;
        }

        for (int i = 0; i < cells.Count; i++)
        {
            cells[i] = new Vector2Int(
                cells[i].x - minX,
                cells[i].y - minY
            );
        }
    }
}
#endif
