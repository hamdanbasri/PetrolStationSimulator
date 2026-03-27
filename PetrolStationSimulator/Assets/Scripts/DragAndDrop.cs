using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DragAndDrop : MonoBehaviour
{
    // Drag your Circle UI object here in the Inspector
    public RectTransform virtualCursor;
    public float grabDistance = 100f;

    private GameObject grabbedObject;
    private float zDistance;
    private Vector3 offset;

    void Awake()
    {
        virtualCursor = FindAnyObjectByType<VirtualCursor>().gameObject.GetComponent<RectTransform>();
    }

    void Update()
    {
        // 1. CHECK FOR MOUSE DOWN (Pick up)
        if (Input.GetMouseButtonDown(0))
        {
            TryGrabObject();
            // CheckForUI();
        }

        // 2. CHECK FOR MOUSE HOLD (Dragging)
        if (Input.GetMouseButton(0) && grabbedObject != null)
        {
            DragObject();
        }

        // 3. CHECK FOR MOUSE UP (Release)
        if (Input.GetMouseButtonUp(0))
        {
            grabbedObject = null;
        }
    }

    void TryGrabObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(virtualCursor.position);
        RaycastHit hit;

        // Check if we hit an object with a collider
        if (Physics.Raycast(ray, out hit, grabDistance))
        {

            if (hit.collider.CompareTag("RemoveItemButton"))
            {
                // Try to find the script on the button and trigger the function
                CashierItemLogic logic = hit.collider.GetComponent<CashierItemLogic>();
                if (logic != null)
                {
                    logic.RemoveFromList();
                    return; // Stop here so we don't try to "drag" the button
                }
            }

            if (hit.collider.CompareTag("RM10"))
            {
                grabbedObject = hit.collider.gameObject;

                // Record how far away the object is from the camera
                zDistance = hit.distance;

                // Calculate offset so the object doesn't "snap" its center to the cursor
                offset = grabbedObject.transform.position - GetWorldPos();
            }

            if (hit.collider.CompareTag("Object"))
            {
                grabbedObject = hit.collider.gameObject;

                // Record how far away the object is from the camera
                zDistance = hit.distance;

                // Calculate offset so the object doesn't "snap" its center to the cursor
                offset = grabbedObject.transform.position - GetWorldPos();
            }
        }
    }

    void DragObject()
    {
        grabbedObject.transform.position = GetWorldPos() + offset;
    }

    Vector3 GetWorldPos()
    {
        Vector3 screenPoint = virtualCursor.position;
        screenPoint.z = zDistance; // Use the distance recorded when we first clicked
        return Camera.main.ScreenToWorldPoint(screenPoint);
    }

    // void CheckForUI()
    // {
    //     PointerEventData eventData = new PointerEventData(EventSystem.current);
    //     eventData.position = virtualCursor.position; // Use your circle's position!

    //     List<RaycastResult> results = new List<RaycastResult>();
    //     EventSystem.current.RaycastAll(eventData, results);

    //     if (results.Count > 0)
    //     {
    //         // We hit a UI element! 
    //         GameObject uiElement = results[0].gameObject;

    //         // If it's a button, click it manually
    //         var button = uiElement.GetComponent<UnityEngine.UI.Button>();
    //         if (button != null) button.onClick.Invoke();
    //     }
    // }
}