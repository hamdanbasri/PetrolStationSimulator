using UnityEngine;

public class VirtualCursor : MonoBehaviour
{
    public float sensitivity = 10f;
    private RectTransform rectTransform;
    private Vector2 virtualPos;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        // Start the virtual position at the center of the screen
        virtualPos = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    void Update()
    {
        // 1. Get the raw movement of the mouse (even when locked)
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // 2. Add that movement to our virtual position
        virtualPos.x += mouseX;
        virtualPos.y += mouseY;

        // 3. Clamp the position so the circle doesn't leave the screen
        virtualPos.x = Mathf.Clamp(virtualPos.x, 0, Screen.width);
        virtualPos.y = Mathf.Clamp(virtualPos.y, 0, Screen.height);

        // 4. Update the UI element's position
        // We subtract half the screen size because UI anchors use 0,0 as center
        rectTransform.position = virtualPos;
    }
}