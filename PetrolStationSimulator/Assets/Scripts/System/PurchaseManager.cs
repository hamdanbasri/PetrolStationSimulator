using System.Collections.Generic;
using UnityEngine;

// 1. We still need a struct to define what an item is.
[System.Serializable]
public struct UpgradeItem
{
    public string itemName;
    public Sprite itemIcon; // Remember: Sprite is the data, Image is the UI component!
    public float itemPrice;
    public GameObject objectPrefab;
}

public class PurchaseManager : MonoBehaviour
{
    // A Singleton instance so other scripts can easily find this manager
    public static PurchaseManager Instance;

    [Header("Shop Database")]
    // Your list of items that you will populate in the Inspector
    public List<UpgradeItem> shopItems = new List<UpgradeItem>();

    [Header("UI References")]
    public GameObject itemPrefab;
    public Transform contentParent;

    private void Awake()
    {
        // Simple Singleton setup: ensures only one ShopManager exists
        if (Instance == null) 
        { 
            Instance = this; 
        }
        else 
        { 
            Destroy(gameObject); 
        }
    }

    private void Start()
    {
        // Populate the shop as soon as the game starts
        PopulateShop();
    }

    public void PopulateShop()
    {
        // 1. Clear out any dummy/placeholder items in the layout group first
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 2. Loop through the list and spawn a prefab for each item
        foreach (UpgradeItem item in shopItems)
        {
            GameObject spawnedItem = Instantiate(itemPrefab, contentParent);

            PlacementObjectInfo placementObjectInfo = spawnedItem.GetComponent<PlacementObjectInfo>();
            if (placementObjectInfo != null)
            {
                placementObjectInfo.SetupElement(item);
            }
        }
    }
}