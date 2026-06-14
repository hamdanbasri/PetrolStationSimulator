using UnityEngine;

public class PlacedObjectData : MonoBehaviour
{
    [Header("Save/Load Configuration")]
    [Tooltip("A unique string matching the prefab key in the SaveManager's collection (e.g., 'ATM', 'PetrolPump', 'Shelf').")]
    public string itemTypeID;

    [HideInInspector]
    public GameObject originalPrefab;
    
    [HideInInspector]
    public float objectPrice;

    void Awake()
    {
        // Gracefully handle if GridSystem instance is initializing simultaneously
        if (GridSystem.Instance != null)
        {
            objectPrice = GridSystem.Instance.itemPrice;
        }
    }

    void Start()
    {
        // Automatically register with the save manager when this object is fully built in the world
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.RegisterObject(this);
        }
    }

    void OnDestroy()
    {
        // Automatically clean up references if deleted through EditManager or developer wiping
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.UnregisterObject(this);
        }
    }
}