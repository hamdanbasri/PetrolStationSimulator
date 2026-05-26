using UnityEngine;
using TMPro;

public class CashManager : MonoBehaviour
{
    public float cashAmount;
    public TextMeshProUGUI cashAmountText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateCashAmount();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCashAmount()
    {
        cashAmountText.text = cashAmount.ToString("F2");
    }
}
