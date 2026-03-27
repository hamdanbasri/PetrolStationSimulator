using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCollector : MonoBehaviour
{
    public List<GameObject> inventoryObjects = new List<GameObject>();


    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Object") || other.CompareTag("Snacks"))
        {
            // Add the GameObject to the list
            if (!inventoryObjects.Contains(other.gameObject))
            {
                inventoryObjects.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
{
    if (other.CompareTag("Object") || other.CompareTag("Snacks"))
    {
        inventoryObjects.Remove(other.gameObject);
    }
}
}
