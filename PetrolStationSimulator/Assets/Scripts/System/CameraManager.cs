using UnityEngine;
using UnityEngine.EventSystems; // Required for UI checking

public class CameraManager : MonoBehaviour
{
    [Header("References")]
    public GridSystem gridSystem;
    
    [Header("Orbit Settings (Middle Mouse)")]
    public float orbitSpeed = 5f;
    public float pitchMin = 5f; // Slightly above 0 to prevent ground clipping
    public float pitchMax = 60f;
    
    [Header("Zoom Settings (Scroll Wheel)")]
    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 20f;
    
    [Header("Smoothing")]
    public float moveSmoothness = 10f;
    public float rotationSmoothness = 10f;
    public float zoomSmoothness = 10f;

    [Header("Input Thresholds")]
    public float dragThresholdPixels = 10f;

    private Transform cameraTransform;
    private Transform pivotTransform;
    private Plane groundPlane;

    // State Variables
    private bool isDragging;
    private bool canOrbit; // Tracks if the orbit started on a valid game-world area
    private Vector2 dragStartMousePos;

    // Target values for smooth lerping
    private Vector3 targetPivotPosition;
    private float targetYaw;
    private float targetPitch;
    private float targetDistance;

    // Current values
    private Vector3 currentPivotPosition;
    private float currentYaw;
    private float currentPitch;
    private float currentDistance;

    void Start()
    {
        cameraTransform = GetComponent<Camera>().transform;
        groundPlane = new Plane(Vector3.up, Vector3.zero);

        // 1. Create the rig dynamically
        GameObject pivotObj = new GameObject("CameraRig_Pivot");
        pivotTransform = pivotObj.transform;

        // 2. Calculate initial target based on starting camera position
        Ray initialRay = new Ray(cameraTransform.position, cameraTransform.forward);
        if (groundPlane.Raycast(initialRay, out float dist))
        {
            targetPivotPosition = initialRay.GetPoint(dist);
        }
        else
        {
            // Fallback in case camera isn't looking at the ground
            targetPivotPosition = new Vector3(cameraTransform.position.x, 0f, cameraTransform.position.z + 10f);
        }

        // 3. Extract the initial editor angles and distance perfectly
        targetYaw = cameraTransform.eulerAngles.y;
        targetPitch = cameraTransform.eulerAngles.x;
        targetDistance = Vector3.Distance(cameraTransform.position, targetPivotPosition);

        // 4. Sync current to target instantly on start
        currentPivotPosition = targetPivotPosition;
        currentYaw = targetYaw;
        currentPitch = targetPitch;
        currentDistance = targetDistance;

        // 5. Build the hierarchy and ZERO the camera's local rotation
        pivotTransform.position = currentPivotPosition;
        pivotTransform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        
        cameraTransform.SetParent(pivotTransform);
        cameraTransform.localPosition = new Vector3(0f, 0f, -currentDistance);
        cameraTransform.localRotation = Quaternion.identity; // This fixes the weird direction bug!
    }

    void Update()
    {
        // Check if the pointer is over a UI element this frame
        bool isOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        HandleZoom(isOverUI);
        HandleOrbit(isOverUI);
        HandleLeftClickMove(isOverUI);
    }

    void LateUpdate()
    {
        // Smoothly interpolate all values
        currentPivotPosition = Vector3.Lerp(currentPivotPosition, targetPivotPosition, Time.deltaTime * moveSmoothness);
        currentYaw = Mathf.LerpAngle(currentYaw, targetYaw, Time.deltaTime * rotationSmoothness);
        currentPitch = Mathf.LerpAngle(currentPitch, targetPitch, Time.deltaTime * rotationSmoothness);
        currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * zoomSmoothness);

        ApplyTransformations();
    }

    private void HandleLeftClickMove(bool isOverUI)
    {
        // Instantly target a new lookAt position on Left-Click, provided we aren't placing an object
        if (Input.GetMouseButtonDown(0))
        {
            // Ignore click if over UI
            if (isOverUI) return; 

            if (gridSystem != null && !gridSystem.isPlacing)
            {
                SetNewLookAtPoint();
            }
        }
    }

    private void HandleOrbit(bool isOverUI)
    {
        // Orbiting uses Middle Mouse Button (2)
        if (Input.GetMouseButtonDown(2))
        {
            // If we start the click on UI, prevent orbiting for the duration of this click
            if (isOverUI)
            {
                canOrbit = false;
            }
            else
            {
                canOrbit = true;
                isDragging = false;
                dragStartMousePos = Input.mousePosition;
            }
        }

        if (Input.GetMouseButton(2) && canOrbit)
        {
            // Check threshold before initiating drag
            if (!isDragging && Vector2.Distance(dragStartMousePos, Input.mousePosition) > dragThresholdPixels)
            {
                isDragging = true;
                
                // Hide and lock the cursor for clean orbiting
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (isDragging)
            {
                targetYaw += Input.GetAxis("Mouse X") * orbitSpeed;
                targetPitch -= Input.GetAxis("Mouse Y") * orbitSpeed;
                targetPitch = Mathf.Clamp(targetPitch, pitchMin, pitchMax);
            }
        }

        if (Input.GetMouseButtonUp(2))
        {
            // Reset orbit validation
            canOrbit = false;

            // Release the cursor back to the user
            if (isDragging)
            {
                isDragging = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    private void HandleZoom(bool isOverUI)
    {
        // Prevent zooming if hovering over UI (allows safe scrolling in UI menus)
        if (isOverUI) return;

        float scrollDelta = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scrollDelta) > 0.01f)
        {
            targetDistance -= scrollDelta * zoomSpeed;
            targetDistance = Mathf.Clamp(targetDistance, minZoom, maxZoom);
        }
    }

    private void SetNewLookAtPoint()
    {
        // Temporarily unlock cursor logic strictly for calculating the raycast properly
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        
        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);

            // Snap the lookAtPoint to the exact grid size used in GridSystem
            float gridSnap = gridSystem != null ? gridSystem.gridSize : 1f;
            float snappedX = Mathf.Round(hitPoint.x / gridSnap) * gridSnap;
            float snappedZ = Mathf.Round(hitPoint.z / gridSnap) * gridSnap;

            targetPivotPosition = new Vector3(snappedX, 0f, snappedZ);
        }
    }

    private void ApplyTransformations()
    {
        pivotTransform.position = currentPivotPosition;
        pivotTransform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        cameraTransform.localPosition = new Vector3(0f, 0f, -currentDistance);
    }
}