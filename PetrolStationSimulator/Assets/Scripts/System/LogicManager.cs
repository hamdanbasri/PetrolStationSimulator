using UnityEngine;

public class LogicManager : MonoBehaviour
{
    public UIManager uiManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //uiManager = FindFirstObjectByType<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            uiManager.mainUI.SetActive(true);
        }
    }
}
