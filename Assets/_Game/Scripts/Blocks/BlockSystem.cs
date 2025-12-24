using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

public class BlockSystem : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private BlockObj blockPrefab;
    [SerializeField] private BrickObj brickPrefab;

    [Header("Data")]
    [SerializeField] private BlockShapeSO blockShapeSO;
    [SerializeField] private BlockThemeSO     blockThemeSO;
    [SerializeField] private BlockRandomLogic blockRandomLogic;

    [Header("Spawn Points (size = 3)")]
    [SerializeField] private BlockSpawnPoint[] spawnPoints;

    [Header("Brick")]
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private List<BlockObj> blockList = new List<BlockObj>();
    
    public List<BlockObj> BlockList => blockList;

    private void Awake()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            spawnPoint.OnDetachBlock += OnDetachBlockHandle;
        }
    }

    private void OnDestroy()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            spawnPoint.OnDetachBlock -= OnDetachBlockHandle;
        }
    }

    private void OnDetachBlockHandle(BlockObj obj)
    {
        if(blockList.Contains(obj))
            blockList.Remove(obj);
    }

    [Button("Generate 3 Blocks")]
    public void Generate3Blocks()
    {
        ClearAllPoints();
        SpawnForAllPoints();
    }

    // ================= CORE =================

    private void SpawnForAllPoints()
    {
        int count = spawnPoints.Length;

        if (this.blockRandomLogic == null)
        {
            Debug.LogError("BlockSystem: Missing BlockRandomStrategy");
            return;
        }

        this.blockRandomLogic.Generate(
            count,
            blockShapeSO,
            blockThemeSO,
            out var shapes,
            out var themeIds
        );

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            var point = spawnPoints[i];
            if (point == null || point.HasBlock)
                continue;

            var block = SpawnBlock(
                shapes[i],
                themeIds[i],
                point.transform
            );

            point.Attach(block);
            blockList.Add(block);
        }
    }


    private BlockObj SpawnBlock(BlockShape shape, int themeId, Transform parent)
    {
        // ===== BLOCK =====
        var block = LazyPooling.Instant.getObj(blockPrefab);
        block.gameObject.SetActive(true);
        block.transform.SetParent(parent);
        block.transform.localPosition = Vector3.zero;
        block.UnSelect();
        block.SetTheme(themeId);

        Sprite sprite = blockThemeSO.GetById(themeId);

        // ===== BRICKS =====
        Vector2 centerOffset = GetShapeCenter(shape, cellSize);

        foreach (var cell in shape.Cells)
        {
            var brick = LazyPooling.Instant.getObj(brickPrefab);
            brick.gameObject.SetActive(true);
            brick.transform.SetParent(block.transform);
            
            brick.GridOffset = cell;

            brick.transform.localPosition =
                new Vector3(
                    cell.x * cellSize - centerOffset.x,
                    cell.y * cellSize - centerOffset.y,
                    0
                );

            brick.transform.localScale = Vector3.one;
            brick.SetSprite(sprite);
            brick.SetBrick(BrickObj.BrickType.Free);

            block.AddBrick(brick);
        }

        return block;
    }

    private void ClearAllPoints()
    {
        foreach (var point in spawnPoints)
        {
            if (point == null || !point.HasBlock)
                continue;

            var block = point.CurrentBlock;

            foreach (var brick in block.Bricks)
            {
                brick.gameObject.SetActive(false);
                brick.transform.SetParent(null);
            }

            block.ClearBricks();
            block.gameObject.SetActive(false);
            point.Detach();
        }
    }


    private List<int> PickDistinctThemeIds(int count)
    {
        var temp   = new List<BlockThemeData>(blockThemeSO.Sprites);
        var result = new List<int>();

        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, temp.Count);
            result.Add(temp[idx].Id);
            temp.RemoveAt(idx);
        }

        return result;
    }

    private Vector2 GetShapeCenter(BlockShape shape, float cellSize)
    {
        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (var c in shape.Cells)
        {
            if (c.x < minX) minX = c.x;
            if (c.y < minY) minY = c.y;
            if (c.x > maxX) maxX = c.x;
            if (c.y > maxY) maxY = c.y;
        }

        float centerX = (minX + maxX + 1) * 0.5f * cellSize;
        float centerY = (minY + maxY + 1) * 0.5f * cellSize;

        return new Vector2(centerX, centerY);
    }
}
