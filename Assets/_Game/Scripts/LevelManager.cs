using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public const int MAX_BLOCKS = 3;
    
    [Header("Ref")]
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private BlockSystem blockSystem;
    [SerializeField] private BlockDragHandler blockDragHandler;
    
    [SerializeField] private int resetBlock = 0;
    
    public GridSystem GridSystem { get => gridSystem;}
    public BlockSystem BlockSystem { get => blockSystem;}

    private void Reset()
    {
        gridSystem = GetComponentInChildren<GridSystem>();
        blockSystem = GetComponentInChildren<BlockSystem>();
        blockDragHandler = GetComponentInChildren<BlockDragHandler>();
    }

    private void Awake()
    {
        resetBlock                     =  0;
        blockDragHandler.OnBlockPlaced += OnBlockPlacedHandle;
        blockSystem.Generate3Blocks();
    }

    private void OnDestroy()
    {
        blockDragHandler.OnBlockPlaced -= OnBlockPlacedHandle;
    }

    private void OnBlockPlacedHandle()
    {
        resetBlock++;
        if (resetBlock >= MAX_BLOCKS)
        {
            resetBlock = 0;
            blockSystem.Generate3Blocks();
        }
    }
}
