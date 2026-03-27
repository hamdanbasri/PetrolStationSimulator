using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using TMPro;
using UnityEngine;
using JetBrains.Annotations;
using UnityEditor.ShaderGraph.Internal;

public class Scanner : MonoBehaviour
{
    [Header("Cashier")]
    public float totalPrice;
    public TextMeshProUGUI priceText;
    public float amountPaidPrice;
    public TextMeshProUGUI amountPaidText;
    public float balanceAmount;
    public TextMeshProUGUI balanceAmountText;
    public GameObject scanLight;
    public int scannedItemNumber;
    public List<GameObject> items = new List<GameObject>();


    [Header("Prefab")]
    public GameObject Button_Item;
    public Transform Panel_Item;
    // Start is called before the first frame update
    void Start()
    {
        scanLight.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Object") || other.CompareTag("Snacks") && scannedItemNumber < 10 && !other.GetComponent<ObjectInfo>().itemIsScanned)
        {
            StartCoroutine(Scanning());
            Debug.Log($"{other.gameObject.name} is scanned");
            items[scannedItemNumber].SetActive(true);

            TMP_Text itemName = items[scannedItemNumber].transform.Find("Text_Name").GetComponent<TMP_Text>();
            TMP_Text itemPrice = items[scannedItemNumber].transform.Find("Text_Price").GetComponent<TMP_Text>();

            if (itemName != null)
            {
                itemName.text = other.GetComponent<ObjectInfo>().objectName;
                
            }

            if (itemPrice != null)
            {
                itemPrice.text = other.GetComponent<ObjectInfo>().sellPrice.ToString("F2");
                AddPrice(other.GetComponent<ObjectInfo>().sellPrice);
                other.GetComponent<ObjectInfo>().itemIsScanned = true;
                StartCoroutine(Scanning());
            }            
        }
    }

    IEnumerator Scanning()
    {
        scanLight.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        scanLight.SetActive(false);
    }

    public void AddPrice(float amount)
    {
        totalPrice += amount;
        scannedItemNumber++;
        UpdatePrice();
    }

    public void DeductPrice(float amount)
    {
        totalPrice -= amount;
        scannedItemNumber--;
        UpdatePrice();
    }

    public void UpdatePrice()
    {
        priceText.text = totalPrice.ToString("F2");
    }

    public void CalculateAmountPaid(float amount)
    {
        amountPaidPrice += amount;
        amountPaidText.text = $"Amount Paid: {amountPaidPrice.ToString("F2")}";
        CalculateBalanceToReturn(amountPaidPrice);
    }

    public void CalculateBalanceToReturn(float amount)
    {
        balanceAmount = amountPaidPrice - totalPrice;
        balanceAmountText.text = $"Balance: {balanceAmount.ToString("F2")}";
    }
}
