using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashierTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Customer"))
        {
            GameManager.Instance.cashRegisterOccupied = true;
            Debug.Log("Occupied");
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("Customer"))
        {
            GameManager.Instance.cashRegisterOccupied = false;
        }
    }
}
