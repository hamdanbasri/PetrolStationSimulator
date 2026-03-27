using UnityEngine;

public class ReturnCashPosition : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("RM10"))
        {
            other.gameObject.SetActive(false);
            other.transform.position = other.GetComponent<CashLogic>().startPosition;
            other.transform.rotation = other.GetComponent<CashLogic>().startRotation;
        }
    }
}
