using UnityEngine;

public class BrickObj : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    public enum BrickType
    {
        Free,
        Occupied,
        Preview,
        PreviewClear
    }
    
    public const int OccupiedOrderInLayer = 0;
    public const int FreeOrderInLayer     = 10;
    public const int PreviewClearOrderInLayer  = 2;

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void SetBrick(BrickType brickType)
    {
        switch (brickType)
        {
            case BrickType.Occupied:
                spriteRenderer.sortingOrder = OccupiedOrderInLayer;
                spriteRenderer.color        = new Color(1, 1, 1, 1);
                break;

            case BrickType.Free:
                spriteRenderer.sortingOrder = FreeOrderInLayer;
                spriteRenderer.color        = new Color(1, 1, 1, 1);
                break;

            case BrickType.Preview:
                spriteRenderer.sortingOrder = OccupiedOrderInLayer;
                spriteRenderer.color        = new Color(1, 1, 1, 0.5f); 
                break;

            case BrickType.PreviewClear:
                spriteRenderer.sortingOrder = PreviewClearOrderInLayer;
                spriteRenderer.color        = new Color(1f, 1f, 1f, 1f); 
                break;
        }
    }

}
