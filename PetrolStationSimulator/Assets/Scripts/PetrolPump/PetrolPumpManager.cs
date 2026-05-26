using System;
using Unity.VisualScripting;
using UnityEngine;

public class PetrolPumpManager : MonoBehaviour
{
    public bool isOccupied;
    public float fuelPrice;

    [Header("Customer Info")]
    public GameObject car;

    [Header("Pump Settings")]
    [Tooltip("How often in seconds the cash is added (e.g., every 1.0 seconds)")]
    public float tickInterval = 1f;

    [Tooltip("How many liters of fuel are dispensed per second")]
    public float fuelDispenseRate = 5f;

    private float timer = 0f;

    void Start()
    {
        fuelPrice = LogicManager.Instance.fuelPrice;
    }

    void Update()
    {
        // Only run the timer if a car is currently at the pump
        if (isOccupied)
        {
            timer += Time.deltaTime;

            // Check if our timer has reached the tick interval
            if (timer >= tickInterval)
            {
                // Calculate cash based on how much fuel was dispensed during this interval
                float litersDispensed = fuelDispenseRate * tickInterval;
                float cashToAdd = litersDispensed * fuelPrice;

                CashManager.Instance.cashAmount += cashToAdd;
                CashManager.Instance.UpdateCashAmount();

                LogicManager.Instance.fuelRemaining -= litersDispensed;
                LogicManager.Instance.UpdateFuelRemaining();
                car.GetComponent<CarNavigation>().currentFuel += litersDispensed;

                // Subtract the interval from the timer rather than resetting to 0. 
                // This prevents losing fractions of a second and keeps the math highly accurate over time!
                timer -= tickInterval; 
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Toggle occupied status ON when the car arrives
        if (other.CompareTag("Car"))
        {
            isOccupied = true;
            car = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Toggle occupied status OFF and reset the timer when the car leaves
        if (other.CompareTag("Car"))
        {
            isOccupied = false;
            timer = 0f; 
        }
    }
}