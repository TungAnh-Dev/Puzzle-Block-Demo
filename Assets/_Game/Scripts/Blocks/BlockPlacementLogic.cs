using UnityEngine;

public abstract class BlockPlacementLogic : MonoBehaviour
{
    public abstract void PreviewBlock(BlockObj block);
    public abstract bool PlaceBlock(BlockObj block);
    public abstract void ClearPreview();
    public abstract bool CanPlaceBlockAnywhere(BlockObj block);
}