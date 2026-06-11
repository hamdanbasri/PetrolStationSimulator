using UnityEngine;
using TMPro;

public class CashManager : MonoBehaviour
{
    public static CashManager Instance;
    public float cashAmount;
    public TextMeshProUGUI cashAmountText;
    public TextMeshProUGUI dashboardCashAmountText;

    void Awake()
    {
        // Standard Singleton Setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
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
        dashboardCashAmountText.text = cashAmount.ToString("F2");
    }
}
