using UnityEngine;

public class BrickDebug : MonoBehaviour
{
    public GameObject X;
    public GameObject O;
    
    public enum BrickDebugType
    {
        Occupied,
        Unoccupied,
    }

    public void SetBrickDebugType(BrickDebugType type)
    {
        switch (type)
        {
            case BrickDebugType.Occupied:
                X.SetActive(true);
                O.SetActive(false);
                break;
            case BrickDebugType.Unoccupied:
                X.SetActive(false);
                O.SetActive(true);
                break;
        }
    }
}
