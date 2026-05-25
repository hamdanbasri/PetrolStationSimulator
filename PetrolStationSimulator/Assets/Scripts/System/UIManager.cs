using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject mainUI;
    public GridSystem gridSystem;
    public Button placeObjectButton;
    public GameObject placementEnabledText;

    [Header("Object Selection")]
    public GameObject displayShelf;
    public GameObject atm;
    public GameObject iceCreamFridge;
    public Button placeDisplayShelfButton;
    public Button placeATMButton;
    public Button placeIceCreamFridgeButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
            // Add the listener to the onClick event
            placeObjectButton.onClick.AddListener(EnablePlacement);
        }
        else
        {
            Debug.LogWarning("Button reference is missing!");
        }

        if (placeDisplayShelfButton != null)
        {
            // Use lambda () => to pass the parameter correctly
            placeDisplayShelfButton.onClick.AddListener(() => UpdateObjectToPlace(displayShelf));
        }
        else
        {
            Debug.LogWarning("Button reference is missing!");
        }

        if (placeATMButton != null)
        {
            // Use lambda () => to pass the parameter correctly
            placeATMButton.onClick.AddListener(() => UpdateObjectToPlace(atm));
        }
        else
        {
            Debug.LogWarning("Button reference is missing!");
        }

        if (placeIceCreamFridgeButton != null)
        {
            // Use lambda () => to pass the parameter correctly
            placeIceCreamFridgeButton.onClick.AddListener(() => UpdateObjectToPlace(iceCreamFridge));
        }
        else
        {
            Debug.LogWarning("Button reference is missing!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnablePlacement()
    {
        gridSystem.isPlacing = true;
        placementEnabledText.SetActive(true);
        placeObjectButton.interactable = false;
    }

    private void DisablePlacement()
    {        
        placementEnabledText.SetActive(false);
        placeObjectButton.interactable = true; 
    }

    public void UpdateObjectToPlace(GameObject updatePlaceObjected)
    {
        gridSystem.itemPrefab = updatePlaceObjected;
        gridSystem.DestroyGhost();
        gridSystem.CreateGhost();
    }

    void OnDestroy()
    {
        if (gridSystem != null)
        {
            gridSystem.OnPlacementCanceled -= DisablePlacement;
        }
    }
}
