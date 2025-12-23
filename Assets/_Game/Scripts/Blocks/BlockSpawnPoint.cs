using System;
using UnityEngine;

public class BlockSpawnPoint : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BlockObj currentBlock;

    public BlockObj CurrentBlock => currentBlock;

    public bool HasBlock => currentBlock != null;

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Awake()
    {
        spriteRenderer.enabled = false;
    }

    public void Attach(BlockObj block)
    {
        currentBlock = block;

        block.transform.SetParent(transform);
        block.transform.position = transform.position;
    }

    public BlockObj Detach()
    {
        var block = currentBlock;
        currentBlock = null;
        return block;
    }
}