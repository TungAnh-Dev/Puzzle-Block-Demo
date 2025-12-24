using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public const int MAX_BLOCKS = 3;
    
    [Header("Ref")]
    [SerializeField] private GridDataSystem gridDataSystem;
    [SerializeField] private BlockSystem         blockSystem;
    [SerializeField] private BlockDragHandler    blockDragHandler;
    [SerializeField] private BlockPlacementLogic placementLogic;
    
    [SerializeField] private int resetBlock = 0;
    
    public GridDataSystem GridDataSystem { get => this.gridDataSystem;}
    public BlockSystem    BlockSystem    { get => blockSystem;}

    public event Action OnReplayLevel;

    private void Reset()
    {
        this.gridDataSystem       = GetComponentInChildren<GridDataSystem>();
        blockSystem      = GetComponentInChildren<BlockSystem>();
        blockDragHandler = GetComponentInChildren<BlockDragHandler>();
        placementLogic = GetComponentInChildren<BlockPlacementLogic>();
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        resetBlock                     =  0;
        blockDragHandler.OnBlockPlaced += OnBlockPlacedHandle;
        placementLogic.Init();
        blockSystem.Generate3Blocks();
    }

    private void OnDestroy()
    {
        blockDragHandler.OnBlockPlaced -= OnBlockPlacedHandle;
    }

    private void OnBlockPlacedHandle()
    {
        resetBlock++;
        if (IsLose(blockSystem.BlockList))
        {
            resetBlock = 0;
            Debug.Log("Lose");
            //TODO:Show UI -> Replay
            UIManager.Instance.OpenUI<UIReplay>().OnReplayButtonClicked += Replay;
            return;
        }
        if (resetBlock >= MAX_BLOCKS)
        {
            resetBlock = 0;
            blockSystem.Generate3Blocks();
        }
    }
    
    public bool IsLose(List<BlockObj> blocks)
    {
        if (blocks == null || blocks.Count == 0)
            return false;

        for (int i = 0; i < blocks.Count; i++)
        {
            if (placementLogic.CanPlaceBlockAnywhere(blocks[i]))
                return false;
        }

        return true; 
    }

    [Button]
    public void Test()
    {
        UIManager.Instance.OpenUI<UIReplay>().OnReplayButtonClicked += Replay;
    }
    
    [Button]
    public void Replay()
    {
        resetBlock = 0;
        gridDataSystem.ClearAllData();
        blockSystem.Generate3Blocks();
        OnReplayLevel?.Invoke();
    }
}
