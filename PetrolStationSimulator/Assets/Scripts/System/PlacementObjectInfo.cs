using UnityEngine;
using UnityEngine.UI;
using TMPro; 

[RequireComponent(typeof(Button))] // Good practice: ensures a Button component exists on this GameObject
public class PlacementObjectInfo : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public Image iconImage;
    
    [HideInInspector] // We don't need to see this in the inspector anymore, it's handled by code
    public GameObject objectPrefab;
    
    private Button myButton;

    private void Awake()
    {
        // Grab the button component from this UI prefab
        myButton = GetComponent<Button>();
    }

    // Called by the PurchaseManager when the prefab is instantiated
    public void SetupElement(UpgradeItem itemData)
    {
        nameText.text = itemData.itemName;
        priceText.text = "$" + itemData.itemPrice.ToString("F2"); 
        iconImage.sprite = itemData.itemIcon;
        objectPrefab = itemData.objectPrefab;
        //GridSystem.Instance.itemPrice = itemData.itemPrice;

        // Clear any old listeners (good practice if you ever reuse/pool UI elements)
        myButton.onClick.RemoveAllListeners();

        // When this dynamic button is clicked, tell the Singleton UIManager to select this specific prefab
        myButton.onClick.AddListener(() => UIManager.Instance.UpdateObjectToPlace(objectPrefab));
    }
}