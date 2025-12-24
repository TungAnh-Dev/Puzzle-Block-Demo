using System;
using UnityEngine;

public class BlockDragHandler : MonoBehaviour
{
    [Header("Drag UX")]
    [SerializeField]
    private Vector3 liftOffset = new Vector3(0f, 0.6f, 0f);

    private Camera cam;

    private BlockObj        draggingBlock;
    private BlockSpawnPoint fromPoint;
    private Vector3         dragOffset;

    [SerializeField] BlockPlacementLogic grid;
    
    public event Action OnBlockPlaced;

    private void Awake()
    {
        cam  = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TryPick();

        if (Input.GetMouseButton(0))
            Drag();

        if (Input.GetMouseButtonUp(0))
            Release();
    }


    private void TryPick()
    {
        Vector3 mouseWorld = GetMouseWorld();

        RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero);
        if (!hit)
            return;

        var point = hit.collider.GetComponent<BlockSpawnPoint>();
        if (point == null || !point.HasBlock)
            return;

        fromPoint     = point;
        draggingBlock = point.CurrentBlock;

        draggingBlock.Select();

        dragOffset =
            draggingBlock.transform.position
            - mouseWorld
            + liftOffset;
    }


    private void Drag()
    {
        if (draggingBlock == null)
            return;

        draggingBlock.transform.position =
            GetMouseWorld() + dragOffset;

        grid.PreviewBlock(draggingBlock);
    }


    private void Release()
    {
        if (draggingBlock == null)
            return;

        bool placed = grid.PlaceBlock(draggingBlock);

        if (placed)
        {
            fromPoint.Detach();
            DespawnBlock(draggingBlock);
            OnBlockPlaced?.Invoke();
        }
        else
        {
            draggingBlock.transform.position = fromPoint.transform.position;
            grid.ClearPreview();
            draggingBlock.UnSelect();
        }
        
        draggingBlock = null;
        fromPoint     = null;
    }

    // ================= UTIL =================

    private void DespawnBlock(BlockObj block)
    {
        foreach (var brick in block.Bricks)
        {
            brick.gameObject.SetActive(false);
            brick.transform.SetParent(null);
        }

        block.ClearBricks();
        block.gameObject.SetActive(false);
        block.transform.SetParent(null);
    }

    private Vector3 GetMouseWorld()
    {
        Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        return pos;
    }
}
