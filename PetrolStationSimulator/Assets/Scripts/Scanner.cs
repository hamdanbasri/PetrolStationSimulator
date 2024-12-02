using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using TMPro;
using UnityEngine;
using JetBrains.Annotations;

public class Scanner : MonoBehaviour
{
    [Header("Cashier")]
    public float totalPrice;
    public TextMeshProUGUI priceText;
    public GameObject scanLight;

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
        if (other.CompareTag("Object"))
        {
            GameObject newObject = Instantiate(Button_Item, Panel_Item);
            RectTransform rectTransform = newObject.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;
            rectTransform.localPosition = Vector3.zero;

            TMP_Text itemName = newObject.transform.Find("Text_Name").GetComponent<TMP_Text>();
            TMP_Text itemPrice = newObject.transform.Find("Text_Price").GetComponent<TMP_Text>();
            if (itemName != null)
            {
                itemName.text = other.GetComponent<ObjectInfo>().objectName; ;
            }

            if (itemPrice != null)
            {
                itemPrice.text = other.GetComponent<ObjectInfo>().sellPrice.ToString("F2");
                totalPrice += other.GetComponent<ObjectInfo>().sellPrice;
                priceText.text = totalPrice.ToString("F2");

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
}
