using UnityEngine;
using UnityEngine.UIElements;

public class CashLogic : MonoBehaviour
{
    public Vector3 startPosition;
    public Quaternion startRotation;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }
}
