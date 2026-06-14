using UnityEngine;
using TMPro;

public class LogicManager : MonoBehaviour
{
    public static LogicManager Instance;
    public UIManager uiManager;
    public float fuelPrice;
    public int pumpIslandAmount;
    public float fuelRemaining;
    public TextMeshProUGUI fuelRemainingText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {        
        CalculateFuelCapacity();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            uiManager.mainUI.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            uiManager.dashboardUI.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {            
            HideDashboards();
        }
    }

    public void CalculateFuelCapacity()
    {
        pumpIslandAmount = GameObject.FindObjectsByType<PetrolPumpManager>(FindObjectsSortMode.None).Length;
        fuelRemaining = pumpIslandAmount * 7500 / 2;
        fuelRemainingText.text = fuelRemaining.ToString() + "l";
    }

    public void UpdateFuelRemaining()
    {
        fuelRemainingText.text = fuelRemaining.ToString() + "l";
    }

    public void HideDashboards()
    {
        uiManager.mainUI.SetActive(false);
        uiManager.dashboardUI.SetActive(false);
    }
}
