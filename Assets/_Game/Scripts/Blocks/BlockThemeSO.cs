using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(
    fileName = "BlockThemeSO",
    menuName = "ScriptableObjects/BlockThemeSO",
    order = 1)]
public class BlockThemeSO : ScriptableObject
{
    [SerializeField]
    public List<BlockThemeData> Sprites = new();

#if UNITY_EDITOR
    private void Reset()
    {
        Sprites.Clear();

        string   folderPath = "Assets/_Game/Sprites/Block";
        string[] guids      = AssetDatabase.FindAssets("t:Sprite", new[] { folderPath });

        int id = 0;

        foreach (var guid in guids)
        {
            string path   = AssetDatabase.GUIDToAssetPath(guid);
            var    sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            if (sprite == null)
                continue;

            Sprites.Add(new BlockThemeData
            {
                Id     = id,
                Sprite = sprite
            });

            id++;
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif


    public Sprite GetById(int id)
    {
        for (int i = 0; i < Sprites.Count; i++)
        {
            if (Sprites[i].Id == id)
                return Sprites[i].Sprite;
        }

        Debug.LogWarning($"[BlockThemeSO] Sprite not found for Id: {id}");
        return null;
    }

    public bool TryGetSprite(int id, out Sprite sprite)
    {
        for (int i = 0; i < Sprites.Count; i++)
        {
            if (Sprites[i].Id == id)
            {
                sprite = Sprites[i].Sprite;
                return true;
            }
        }

        sprite = null;
        return false;
    }
    
    public Sprite GetRandom()
    {
        if (Sprites == null || Sprites.Count == 0)
            return null;

        int index = Random.Range(0, Sprites.Count);
        return Sprites[index].Sprite;
    }

}

[System.Serializable]
public struct BlockThemeData
{
    public int    Id;
    public Sprite Sprite;
}