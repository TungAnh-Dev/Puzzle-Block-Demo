using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(
    fileName = "BlockShapeSO",
    menuName = "ScriptableObjects/BlockShapeSO",
    order = 1)]
public class BlockShapeSO : ScriptableObject
{
    [SerializeField, Tooltip("Auto loaded from Resources")]
    private TextAsset shapeJson;

    [SerializeField]
    public List<BlockShape> Shapes = new();

#if UNITY_EDITOR
    private void Reset()
    {
        LoadFromResources();
        LoadFromJson();
    }

    [ContextMenu("Reload From Resources")]
    private void LoadFromResources()
    {
        shapeJson = Resources.Load<TextAsset>("block_shapes");

        if (shapeJson == null)
        {
            Debug.LogError(
                "BlockShapeSO: Cannot load 'block_shapes.json' from Resources"
            );
        }
    }

    [ContextMenu("Reload From JSON")]
    private void LoadFromJson()
    {
        Shapes.Clear();

        if (shapeJson == null)
        {
            Debug.LogError("BlockShapeSO: Missing JSON file");
            return;
        }

        var json = JsonUtility.FromJson<BlockShapeJsonFile>(shapeJson.text);
        if (json == null || json.shapes == null)
        {
            Debug.LogError("BlockShapeSO: Invalid JSON");
            return;
        }

        foreach (var s in json.shapes)
        {
            Shapes.Add(new BlockShape
            {
                ID    = s.id,
                Cells = s.cells.ToArray()
            });
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();

        Debug.Log($"BlockShapeSO loaded {Shapes.Count} shapes from Resources");
    }
#endif

    public BlockShape GetByID(int id)
    {
        for (int i = 0; i < Shapes.Count; i++)
            if (Shapes[i].ID == id)
                return Shapes[i];

        return default;
    }
}