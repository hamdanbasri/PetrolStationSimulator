using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [Header("Placement State")]
    public bool isPlacing;
    public float gridSize = 1f;

    [Header("References")]
    [Tooltip("The single prefab used for both the ghost and the final placed object.")]
    public GameObject itemPrefab;

    [Header("Visual Feedback")]
    public Material validMaterial;
    public Material invalidMaterial;

    [Header("Collision Settings")]
    [Tooltip("Layer assigned to objects after they are placed.")]
    public LayerMask placedObjectsLayer;

    private Camera mainCamera;
    private Plane groundPlane;
    private bool canPlace = true;

    // Bounds based on a 1,1,1 Plane at 0,0,0
    private readonly float minBound = -5f;
    private readonly float maxBound = 5f;

    // Runtime Ghost Data
    private GameObject ghostInstance;
    private BoxCollider ghostCollider;
    private Renderer[] ghostRenderers;

    void Start()
    {
        mainCamera = Camera.main;
        groundPlane = new Plane(Vector3.up, Vector3.zero);
    }

    void Update()
    {
        // 1. Handle Escaping
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPlacing = false;
        }

        // 2. Manage Ghost Lifecycle
        if (isPlacing && ghostInstance == null && itemPrefab != null)
        {
            CreateGhost();
        }
        else if (!isPlacing && ghostInstance != null)
        {
            DestroyGhost();
            return;
        }

        // If not placing, do nothing else
        if (!isPlacing) return;

        // 3. Core Logic
        HandleRotation();
        HandleHoverAndSnap();
        CheckPlacementValidity();
        UpdateGhostVisuals();

        // 4. Place Object (Continuous)
        if (Input.GetMouseButtonDown(0) && canPlace)
        {
            PlaceObject();
        }
    }

    private void CreateGhost()
    {
        // Instantiate the prefab to act as our ghost
        ghostInstance = Instantiate(itemPrefab);
        ghostInstance.name = "Ghost_" + itemPrefab.name;

        // Force it to the "Ignore Raycast" layer (Layer 2) so the mouse doesn't hit it
        Transform[] allChildren = ghostInstance.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            child.gameObject.layer = 2; 
        }

        ghostCollider = ghostInstance.GetComponent<BoxCollider>();
        if (ghostCollider != null)
        {
            ghostCollider.isTrigger = true; // Prevent physical collisions while hovering
        }

        ghostRenderers = ghostInstance.GetComponentsInChildren<Renderer>();
    }

    private void DestroyGhost()
    {
        if (ghostInstance != null)
        {
            Destroy(ghostInstance);
        }
    }

    private void HandleHoverAndSnap()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);

            float snappedX = Mathf.Round(hitPoint.x / gridSize) * gridSize;
            float snappedZ = Mathf.Round(hitPoint.z / gridSize) * gridSize;

            ghostInstance.transform.position = new Vector3(snappedX, 0f, snappedZ);
        }
    }

    private void HandleRotation()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ghostInstance.transform.Rotate(Vector3.up, -90f);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            ghostInstance.transform.Rotate(Vector3.up, 90f);
        }
    }

    private void CheckPlacementValidity()
    {
        canPlace = true;
        Vector3 currentPos = ghostInstance.transform.position;

        // Condition A: Bounds Checking
        if (currentPos.x < minBound || currentPos.x > maxBound ||
            currentPos.z < minBound || currentPos.z > maxBound)
        {
            canPlace = false;
            return; 
        }

        // Condition B: Collision Detection
        if (ghostCollider != null)
        {
            Vector3 boxCenterWorld = ghostInstance.transform.TransformPoint(ghostCollider.center);
            Vector3 halfExtents = Vector3.Scale(ghostCollider.size, ghostInstance.transform.lossyScale) * 0.45f;

            Collider[] hitColliders = Physics.OverlapBox(
                boxCenterWorld,
                halfExtents,
                ghostInstance.transform.rotation,
                placedObjectsLayer
            );

            if (hitColliders.Length > 0)
            {
                canPlace = false;
            }
        }
    }

    private void UpdateGhostVisuals()
    {
        if (ghostRenderers == null || ghostRenderers.Length == 0) return;

        Material targetMat = canPlace ? validMaterial : invalidMaterial;

        foreach (Renderer rend in ghostRenderers)
        {
            // Only swap if it's not already the target material to save performance
            if (rend.sharedMaterial != targetMat)
            {
                // Create an array of the target material to handle meshes with multiple material slots
                Material[] mats = new Material[rend.materials.Length];
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = targetMat;
                }
                rend.materials = mats;
            }
        }
    }

    private void PlaceObject()
    {
        // Instantiate the REAL permanent object
        GameObject newObj = Instantiate(itemPrefab, ghostInstance.transform.position, ghostInstance.transform.rotation);

        // Assign the newly placed object to the PlacedObjects layer
        int layerIndex = Mathf.RoundToInt(Mathf.Log(placedObjectsLayer.value, 2));
        Transform[] allChildren = newObj.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            child.gameObject.layer = layerIndex;
        }

        // Note: Because isPlacing stays true, the ghost remains visible.
        // It will immediately turn red on the next frame because it is now colliding
        // with the object we just placed, which is the correct feedback!
    }
}