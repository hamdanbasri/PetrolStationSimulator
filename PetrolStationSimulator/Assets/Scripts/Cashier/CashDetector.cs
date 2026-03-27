using UnityEngine;

public class CashDetector : MonoBehaviour
{
    public Scanner scanner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scanner = FindAnyObjectByType<Scanner>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("RM10"))
        {
            scanner.CalculateAmountPaid(10);
        }
    }
}
