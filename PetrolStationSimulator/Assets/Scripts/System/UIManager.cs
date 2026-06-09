using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // Singleton instance so dynamic prefabs can call UpdateObjectToPlace() easily
    public static UIManager Instance;

    public GameObject mainUI;
    public GameObject dashboardUI;
    public GridSystem gridSystem;

    public TextMeshProUGUI dashboardTitle;
    
    [Header("Master Placement Controls")]
    public Button placeObjectButton;
    public GameObject placementEnabledText;

    void Awake()
    {
        // Standard Singleton Setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        mainUI.SetActive(false);
        gridSystem = FindFirstObjectByType<GridSystem>();
        placementEnabledText.SetActive(false);

        if (gridSystem != null)
        {
            gridSystem.OnPlacementCanceled += DisablePlacement;
        }

        if (placeObjectButton != null)
        {
            placeObjectButton.onClick.AddListener(EnablePlacement);
            
            // QoL Addition: Disable the place button until the player actually selects an item from the menu
            placeObjectButton.interactable = false;
        }
        else
        {
            Debug.LogWarning("Place Object Button reference is missing!");
        }
    }

    public void EnablePlacement()
    {
        // Failsafe: Don't allow placement if no prefab is selected
        if (gridSystem.itemPrefab == null) return;

        gridSystem.isPlacing = true;
        placementEnabledText.SetActive(true);
        placeObjectButton.interactable = false;
    }

    private void DisablePlacement()
    {        
        placementEnabledText.SetActive(false);
        
        // Re-enable the place button so they can try placing the selected item again if they canceled
        if (placeObjectButton != null)
        {
            placeObjectButton.interactable = true; 
        }
    }

    // This is now called dynamically by the PlacementObjectInfo buttons!
    public void UpdateObjectToPlace(GameObject updatePlaceObjected)
    {
        gridSystem.itemPrefab = updatePlaceObjected;
        gridSystem.DestroyGhost();
        gridSystem.CreateGhost();

        // Now that an object is selected, enable the master Place button
        if (placeObjectButton != null)
        {
            placeObjectButton.interactable = true;
        }
    }

    public void UpdateDashboardTitle(string title)
    {
        dashboardTitle.text = title;
    }

    void OnDestroy()
    {
        if (gridSystem != null)
        {
            gridSystem.OnPlacementCanceled -= DisablePlacement;
        }
    }
}