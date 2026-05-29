using System.Collections.Generic;
using UnityEngine;

public class StationManager : MonoBehaviour
{
    public static StationManager Instance { get; private set; }

    [Header("Station Setup")]
    // Drag and drop all your Petrol Pumps into this list in the Inspector
    public List<PetrolPumpManager> allPumps = new List<PetrolPumpManager>();
    
    // Where cars should go if the entire station is 100% full
    public Transform stationExit; 

    void Awake()
    {
        // Simple Singleton setup
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Cars will call this method when they spawn
    public PetrolPumpManager GetShortestQueuePump()
    {
        if (allPumps.Count == 0) return null;

        PetrolPumpManager bestPump = allPumps[0];
        int shortestLine = bestPump.carsInQueue.Count;

        // Loop through all pumps to find the one with the fewest waiting cars
        foreach (PetrolPumpManager pump in allPumps)
        {
            if (pump.carsInQueue.Count < shortestLine)
            {
                bestPump = pump;
                shortestLine = pump.carsInQueue.Count;
            }
        }

        return bestPump;
    }

    public void AddPump(PetrolPumpManager newPump)
{
    if (!allPumps.Contains(newPump))
    {
        allPumps.Add(newPump);
    }
}

public void RemovePump(PetrolPumpManager oldPump)
{
    if (allPumps.Contains(oldPump))
    {
        allPumps.Remove(oldPump);
    }
}
}