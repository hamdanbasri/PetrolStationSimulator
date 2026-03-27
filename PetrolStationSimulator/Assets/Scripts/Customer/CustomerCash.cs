using UnityEngine;

public class CustomerCash : MonoBehaviour
{
    [Header("Quantity of Each Bill/Coin")]
    public int cash_TenCents;
    public int cash_FiftyCents;
    public int cash_One;
    public int cash_Five;
    public int cash_Ten;
    public int cash_Twenty;

    void Start()
    {
        GenerateRandomCash();
        Debug.Log($"Customer spawned with: ${GetTotalCashValue()}");
    }

    public void GenerateRandomCash()
    {
        // Random.Range(min, max) -> max is exclusive for integers, 
        // so (0, 6) means they can have between 0 and 5 of that bill.
        cash_TenCents = Random.Range(0, 10);
        cash_FiftyCents = Random.Range(0, 5);
        cash_One = Random.Range(1, 10);    // At least one $1 bill
        cash_Five = Random.Range(0, 4);
        cash_Ten = Random.Range(0, 3);
        cash_Twenty = Random.Range(0, 2);
    }

    public double GetTotalCashValue()
    {
        double total = 0;
        total += cash_TenCents * 0.10;
        total += cash_FiftyCents * 0.50;
        total += cash_One * 1.0;
        total += cash_Five * 5.0;
        total += cash_Ten * 10.0;
        total += cash_Twenty * 20.0;
        
        // Rounding to 2 decimal places to avoid floating point math errors
        return System.Math.Round(total, 2);
    }
}