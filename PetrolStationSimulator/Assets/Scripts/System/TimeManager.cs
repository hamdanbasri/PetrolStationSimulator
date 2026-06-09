using UnityEngine;
using TMPro;
using System;

public class TimeManager : MonoBehaviour
{
    // 1. Singleton Instance
    public static TimeManager Instance { get; private set; }

    // 2. C# Event for Day Changes
    public static event Action<int> OnDayChanged;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dayText;

    [Header("Lighting Reference")]
    [SerializeField] private Light sunLight; // Assign your Directional Light here
    public Material morningSkybox;
    public Material nightSkybox;
    public Light mainSpotLight;
    public Light officeSpotLight;
    public Light storeSpotLight;
    public Light[] pumpSpotLight;

    [Header("Time Settings")]
    [Tooltip("How much faster time passes. 1 = real time. 10 = 10x faster.")]
    [SerializeField] private float timeMultiplier = 10f;

    // Internal tracking variables
    private float currentTimeInSeconds = 0f; // Starts at 00:00 (midnight)
    private int currentDay = 1;              // Starts at Day 1
    
    private const float SECONDS_IN_A_DAY = 86400f; // 24 hours * 60 mins * 60 secs

    private void Awake()
    {
        // Setup Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("More than one TimeManager in the scene! Destroying the newest one.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize UI and Lighting on start
        UpdateUI();
    }

    private void Update()
    {
        // Add time based on real-world seconds passed, scaled by our multiplier
        currentTimeInSeconds += Time.deltaTime * timeMultiplier;

        // Check if a full day has passed
        if (currentTimeInSeconds >= SECONDS_IN_A_DAY)
        {
            currentTimeInSeconds -= SECONDS_IN_A_DAY; // Reset time back to 00:00, keeping leftover decimal seconds
            currentDay++;
            
            // Trigger the event so other scripts know the day changed
            OnDayChanged?.Invoke(currentDay);
        }

        // Update Text and Lights every frame
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Calculate Hours and Minutes
        int hours = Mathf.FloorToInt(currentTimeInSeconds / 3600f);
        int minutes = Mathf.FloorToInt((currentTimeInSeconds % 3600f) / 60f);

        // Format Time to always show double digits (e.g., 08:05 instead of 8:5)
        if (timeText != null)
        {
            timeText.text = string.Format("{0:00}:{1:00}", hours, minutes);
        }

        // Format Day
        if (dayText != null)
        {
            dayText.text = "Day " + currentDay;
        }

        // Update Sun/Night logic
        UpdateLighting(hours);
    }

    private void UpdateLighting(int currentHour)
    {
        if (sunLight != null)
        {
            // If time is 19:00 (7 PM) or later, OR before 07:00 (7 AM) -> It is Night
            if (currentHour >= 19 || currentHour < 7)
            {
                sunLight.intensity = 0f;
                RenderSettings.skybox = nightSkybox;
                mainSpotLight.intensity = 20;
                officeSpotLight.intensity = 5;
                storeSpotLight.intensity = 5;

                foreach(Light pumpSpotlight in pumpSpotLight)
                {
                    pumpSpotlight.intensity = 20;
                }
            }
            else // It is Day
            {
                sunLight.intensity = 1f;                
                RenderSettings.skybox = morningSkybox;
                mainSpotLight.intensity = 0;
                officeSpotLight.intensity = 0;
                storeSpotLight.intensity = 0;

                foreach(Light pumpSpotlight in pumpSpotLight)
                {
                    pumpSpotlight.intensity = 0;
                }
            }
        }
    }
}