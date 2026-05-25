using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Required for UI checking

public class EditManager : MonoBehaviour
{
    [Header("State")]
    public bool isEditMode = false;
    
    [Header("References")]
    public LayerMask placedObjectsLayer;
    public GameObject editUIPanel; // The static panel with Move/Delete buttons

    private GameObject selectedObject;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        editUIPanel.SetActive(false); // Ensure UI is hidden at start
    }

    void Update()
    {
        // Don't do anything if we aren't in Edit Mode
        if (!isEditMode) return;

        // 1. Handle Selection via Raycast
        if (Input.GetMouseButtonDown(0))
        {
            // Prevent clicking through the UI menu to select something else
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, placedObjectsLayer))
            {
                SelectObject(hit.collider.gameObject);
            }
            else
            {
                // Clicked on empty ground, deselect
                DeselectObject();
            }
        }

        // 2. Handle Q/E Rotation for the Selected Object
        if (selectedObject != null)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                selectedObject.transform.Rotate(Vector3.up, -90f);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                selectedObject.transform.Rotate(Vector3.up, 90f);
            }
        }
    }

    public void ToggleEditMode(bool state)
    {
        isEditMode = state;
        
        if (!isEditMode)
        {
            DeselectObject();
        }
    }

    private void SelectObject(GameObject obj)
    {
        // If your placed object has child colliders, we want to grab the root object
        selectedObject = obj.transform.root.gameObject;
        
        // Show the Edit UI Panel
        editUIPanel.SetActive(true);
        
        // Optional visual feedback could go here (like outlining the selected object)
    }

    public void DeselectObject()
    {
        selectedObject = null;
        editUIPanel.SetActive(false);
    }

    // --- BUTTON EVENT: Linked to the "Move" button on your Edit UI Panel ---
    public void MoveSelectedObject()
    {
        if (selectedObject == null) return;

        PlacedObjectData data = selectedObject.GetComponent<PlacedObjectData>();
        if (data != null && data.originalPrefab != null)
        {
            // 1. Tell the UIManager to queue up this prefab for placement
            UIManager.Instance.UpdateObjectToPlace(data.originalPrefab);
            UIManager.Instance.EnablePlacement();

            // 2. Destroy the physical object in the world (GridSystem is now holding the ghost)
            Destroy(selectedObject);
        }
        else
        {
            Debug.LogError("No PlacedObjectData found on this object!");
        }

        // 3. Clean up the Edit UI and turn off Edit mode since we are now Placing
        DeselectObject();
        ToggleEditMode(false);
    }

    // --- BUTTON EVENT: Linked to the "Delete" button on your Edit UI Panel ---
    public void DeleteSelectedObject()
    {
        if (selectedObject == null) return;

        Destroy(selectedObject);
        DeselectObject();
    }

    public void EnableEditMode()
    {
        isEditMode = true;
    }
}