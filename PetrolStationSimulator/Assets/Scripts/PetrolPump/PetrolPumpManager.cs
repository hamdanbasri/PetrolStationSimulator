using System.Collections.Generic;
using UnityEngine;

public class PetrolPumpManager : MonoBehaviour
{
    public bool isOccupied;
    public bool isReserved;
    public float fuelRemaining;
    public float fuelPrice;

    [Header("Pump Settings")]
    public float tickInterval = 1f;
    public float fuelDispenseRate = 5f;
    private float timer = 0f;
    private CarNavigation currentCarAtPump;

    [Header("Queue System")]
    [Tooltip("The physical spot where the car sits to get gas")]
    public Transform pumpingSpot;

    [Tooltip("The spots behind the pump where cars wait in line. Index 0 is the front of the line.")]
    public Transform[] waitSpots;

    // Tracks the cars currently waiting in this specific line
    public List<CarNavigation> carsInQueue = new List<CarNavigation>();

    void Awake()
    {
        // Defaulting to 2.0f just to prevent errors if LogicManager isn't set up yet
        fuelPrice = LogicManager.Instance != null ? LogicManager.Instance.fuelPrice : 2.0f;

        if (StationManager.Instance != null)
        {
            StationManager.Instance.AddPump(this);
        }
    }

    void Update()
    {
        if (isOccupied)
        {
            timer += Time.deltaTime;
            if (timer >= tickInterval)
            {
                float litersDispensed = fuelDispenseRate * tickInterval;
                float cashToAdd = litersDispensed * fuelPrice;

                if (CashManager.Instance != null)
                {
                    CashManager.Instance.cashAmount += cashToAdd;
                    CashManager.Instance.UpdateCashAmount();
                }

                // ADD THIS: Pour the fuel into the car's tank
                if (currentCarAtPump != null)
                {
                    currentCarAtPump.ReceiveFuel(litersDispensed);
                }

                timer -= tickInterval;
            }
        }
    }

    // Called by the Car when it asks to join this pump's line
    public Transform AssignQueueSpot(CarNavigation car)
    {
        // If completely empty, not reserved, and no one is in line
        if (!isOccupied && !isReserved && carsInQueue.Count == 0)
        {
            isReserved = true; // Call dibs!
            return pumpingSpot;
        }

        // Otherwise, check if there is room in the physical line
        if (carsInQueue.Count < waitSpots.Length)
        {
            carsInQueue.Add(car);
            return waitSpots[carsInQueue.Count - 1];
        }

        // The line is totally full
        return null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            isOccupied = true;
            isReserved = false;

            // Grab the script of the car that just pulled in
            currentCarAtPump = other.GetComponent<CarNavigation>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            isOccupied = false;
            timer = 0f;

            // Clear the reference since the car is driving away
            currentCarAtPump = null;

            AdvanceQueue();
        }
    }

    private void AdvanceQueue()
    {
        if (carsInQueue.Count > 0)
        {
            isReserved = true; // The next car is on its way, reserve the spot!

            CarNavigation nextCar = carsInQueue[0];
            carsInQueue.RemoveAt(0);
            nextCar.UpdateDestination(pumpingSpot);

            for (int i = 0; i < carsInQueue.Count; i++)
            {
                carsInQueue[i].UpdateDestination(waitSpots[i]);
            }
        }
    }

    void OnDestroy()
    {
        // If the player sells or deletes a pump, it cleans up after itself
        if (StationManager.Instance != null)
        {
            StationManager.Instance.RemovePump(this);
        }
    }
}