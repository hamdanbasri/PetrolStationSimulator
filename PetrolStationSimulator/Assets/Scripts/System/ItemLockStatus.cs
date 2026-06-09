using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemLockStatus : MonoBehaviour
{
    public bool isLocked;
    public int pointsToUnlock;
    public Button button;
    public GameObject lockImage;
    public TextMeshProUGUI lockText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button.GetComponent<Button>();

        if(isLocked)
        {
            button.interactable = false;
            lockText.text = "UNLOCKS AT " + pointsToUnlock.ToString() + "XP";
        }
        else
        {
            lockImage.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckXP()
    {
        // Need to check the player XP to unlock
    }
}
