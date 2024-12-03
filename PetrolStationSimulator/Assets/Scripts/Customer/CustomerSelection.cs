using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSelection : MonoBehaviour
{
    public InventoryCollector inventoryCollector;
    public List<GameObject> customerInventory = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("InventoryCollector"))
        {
            inventoryCollector = other.GetComponent<InventoryCollector>();
            GetItems();
        }
    }

    public void GetItems()
    {
        int randomDestination = Random.Range(0, inventoryCollector.inventoryObjects.Count);
        customerInventory.Add(inventoryCollector.inventoryObjects[0]);
    }


}
