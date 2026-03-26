using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CashierItemLogic : MonoBehaviour
{
    public Scanner scanner;
    void Start()
    {
        scanner = FindAnyObjectByType<Scanner>();
    }

    public void RemoveFromList()
    {
        scanner.DeductPrice(float.Parse(transform.parent.Find("Text_Price").GetComponent<TMP_Text>().text));
        transform.parent.gameObject.SetActive(false);
        Debug.Log("Item removed");
    }
}
