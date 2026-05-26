using UnityEngine;

public class PlacedObjectData : MonoBehaviour
{
    // Remembers the original prefab so we can spawn a new ghost when moving
    [HideInInspector]
    public GameObject originalPrefab;
    public float objectPrice;

    void Awake()
    {
        objectPrice = GridSystem.Instance.itemPrice;
    }
}