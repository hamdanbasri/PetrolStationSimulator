using UnityEngine;

public class MouseDetector : MonoBehaviour
{
    [Header("Settings")]
    public RectTransform mouseCursor;
    public float rayDistance = 100f;        // Mouse rays usually need more distance
    public string targetTag = "CashRegister";
    public LayerMask detectionLayer;       // Important: Set this to "Default" or your specific layer    

    [Header("Cash Register Settings")]
    public Animator cashRegisterAnimator;
    public bool isCashRegisterOpen;
      
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
            if (hit.collider.CompareTag("OpenCashRegister"))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (!isCashRegisterOpen)
                    {
                        cashRegisterAnimator.SetTrigger("Open");
                        objectToActivate.SetActive(true);
                        isCashRegisterOpen = true;
                    }
                }
            }
            // 3. Check for Box Collider and Tag
            if (hit.collider.CompareTag(targetTag) && objectToActivate.activeSelf)
            {
                // if (objectToActivate != null && !objectToActivate.activeSelf)
                // {
                //     objectToActivate.SetActive(true);
                //     Debug.Log("Looking at Cash Registe");
                // }
                if (Input.GetMouseButtonDown(0))
                {
                    if (isCashRegisterOpen)
                    {
                        cashRegisterAnimator.SetTrigger("Close");
                        isCashRegisterOpen = false;
                    }
                }

            }
            else
            {
                objectToActivate.SetActive(false);
            }
        }
    }
}
