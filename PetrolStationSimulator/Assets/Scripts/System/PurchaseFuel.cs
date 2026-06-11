using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using Cinemachine;

public class PurchaseFuel : MonoBehaviour
{
    [Header("Petrol Price")]
    public float ninetySevenPurchasePrice;
    public float ninetyFivePurchasePrice;
    public float euroPurchasePrice;
    public int priceMultiplier = 1000;

    [Header("Display Price Text")]
    public TextMeshProUGUI ninetySevenDisplayPriceText;
    public TextMeshProUGUI ninetyFiveDisplayPriceText;
    public TextMeshProUGUI euroDisplayPriceText;

    [Header("Purchase Price Text")]
    public TextMeshProUGUI ninetySevenPurchasePriceText;
    public TextMeshProUGUI ninetyFivePurchasePriceText;
    public TextMeshProUGUI euroPurchasePriceText;

    [Header("Total Fuel Purchase Price Text")]
    public TextMeshProUGUI totalFuelPurchasePrice;
    void Start()
    {
        ninetySevenDisplayPriceText.text = ninetySevenPurchasePrice.ToString("F2");
        ninetyFiveDisplayPriceText.text = ninetyFivePurchasePrice.ToString("F2");
        euroDisplayPriceText.text = euroPurchasePrice.ToString("F2");

    }

    void Update()
    {
        
    }

    public void IncreaseFuelPurchase(Image fuelBarImage)
    {
        if(fuelBarImage != null && fuelBarImage.fillAmount != 1)
        {
            fuelBarImage.fillAmount += 0.1f;
            UpdateTotalFuelPurchasePrice();
        }
    }

    public void DecreaseFuelPurchase(Image fuelBarImage)
    {
        if(fuelBarImage != null && fuelBarImage.fillAmount != 0)
        {
            fuelBarImage.fillAmount -= 0.1f;
            UpdateTotalFuelPurchasePrice();
        }
    }

    public void Update97PurchasePrice(Image fuelBarImage)
    {
        if (fuelBarImage != null && fuelBarImage.fillAmount != 0)
        {
            ninetySevenPurchasePriceText.text = "$" + (ninetySevenPurchasePrice * (fuelBarImage.fillAmount * priceMultiplier)).ToString("F2");
        }
        else
        {
            ninetySevenPurchasePriceText.text = "$0.00";
        }
    }

    public void Update95PurchasePrice(Image fuelBarImage)
    {
        if (fuelBarImage != null && fuelBarImage.fillAmount != 0)
        {
            ninetyFivePurchasePriceText.text = "$" + (ninetyFivePurchasePrice * (fuelBarImage.fillAmount * priceMultiplier)).ToString("F2");
        }
        else
        {
            ninetyFivePurchasePriceText.text = "$0.00";
        }
    }

    public void UpdateEuroPurchasePrice(Image fuelBarImage)
    {
        if (fuelBarImage != null && fuelBarImage.fillAmount != 0)
        {
            euroPurchasePriceText.text = "$" + (euroPurchasePrice * (fuelBarImage.fillAmount * priceMultiplier)).ToString("F2");
        }
        else
        {
            euroPurchasePriceText.text = "$0.00";
        }
    }

    public void UpdateTotalFuelPurchasePrice()
    {
        // Clean strings by removing "$" and any invisible spaces
        string clean97 = ninetySevenPurchasePriceText.text.Replace("$", "").Trim((char)8203, ' ');
        string clean95 = ninetyFivePurchasePriceText.text.Replace("$", "").Trim((char)8203, ' ');
        string cleanEuro = euroPurchasePriceText.text.Replace("$", "").Trim((char)8203, ' ');

        // Use TryParse to safely get the values (defaults to 0.0f if empty or invalid)
        float.TryParse(clean97, out float nineSevenPrice);
        float.TryParse(clean95, out float nineFivePrice);
        float.TryParse(cleanEuro, out float euroPrice);

        // Calculate total
        float totalPrice = nineSevenPrice + nineFivePrice + euroPrice;

        // Display total with the dollar sign back in front of it
        totalFuelPurchasePrice.text = "$" + totalPrice.ToString("F2");
    }

}
