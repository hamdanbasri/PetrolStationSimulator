using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInfo : MonoBehaviour
{
    public string objectName;
    public float basePrice;
    public float sellPrice;
    private Scanner scanner;

    void Start()
    {
        scanner = GameObject.FindObjectOfType<Scanner>();
    }

    public void RemoveObject()
    {
        scanner.totalPrice -= sellPrice;
        Destroy(gameObject);
    }
}
