using UnityEngine;

public class MouseDetector : MonoBehaviour
{
    [Header("Settings")]
    public RectTransform mouseCursor;
    public float rayDistance = 100f;        // Mouse rays usually need more distance
    public string targetTag = "CashRegister";
    public LayerMask detectionLayer;       // Important: Set this to "Default" or your specific layer
    
    [Header("References")]
    public GameObject objectToActivate;

    void Update()
    {
        // 1. Create a ray from the mouse position into the 3D scene
        Ray ray = Camera.main.ScreenPointToRay(mouseCursor.position);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
        RaycastHit hit;

        // 2. Perform the Raycast
        if (Physics.Raycast(ray, out hit, rayDistance, detectionLayer))
        {
            // 3. Check for Box Collider and Tag
            if (hit.collider is BoxCollider && hit.collider.CompareTag(targetTag))
            {
                if (objectToActivate != null && !objectToActivate.activeSelf)
                {
                    objectToActivate.SetActive(true);
                    Debug.Log("Mouse is hovering over: " + hit.collider.gameObject.name);
                }
            }
            else
            {
                // Optional: Turn it off if hovering over something else
                objectToActivate.SetActive(false);
            }
        }
    }
}
