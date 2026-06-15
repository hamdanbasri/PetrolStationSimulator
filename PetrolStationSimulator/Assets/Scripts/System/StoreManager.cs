using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    [Header("Shelf Management")]
    [Tooltip("All shelves available in the store.")]
    public List<Transform> availableShelves = new List<Transform>();

    [Header("Cashier Queue Management")]
    [Tooltip("Place empty GameObjects in your scene to act as queue spots. Element 0 is the cashier, Element 1 is the first person waiting, etc.")]
    public Transform queueParent;

    // Changed to a List so it's easier to add things dynamically
    public List<Transform> queuePoints = new List<Transform>();

    // This array tracks WHO is standing in WHICH spot
    private CustomerNavigation[] customersInQueue;

    private void Start()
    {
        // 1. Find all shelves ONLY ONCE at the start of the game
        GameObject[] shelfObjects = GameObject.FindGameObjectsWithTag("DisplayShelf");
        foreach (GameObject shelf in shelfObjects)
        {
            availableShelves.Add(shelf.transform);
        }

        queueParent = GameObject.FindGameObjectWithTag("CashierPayingArea").transform;

        // 2. Setup the Queue Points dynamically
        if (queueParent != null)
        {
            // A foreach loop on a Transform automatically loops through its children in order!
            foreach (Transform childSpot in queueParent)
            {
                queuePoints.Add(childSpot);
            }
        }
        else
        {
            Debug.LogError("You forgot to assign the Queue Parent in the StoreManager!");
        }

        // 3. Setup our queue tracking array based on how many spots we found
        customersInQueue = new CustomerNavigation[queuePoints.Count];
    }

    // --- SHELF LOGIC ---

    public Transform RequestEmptyShelf()
    {
        if (availableShelves.Count == 0)
        {
            return null; // No shelves available!
        }

        // Pick a random shelf from the AVAILABLE list
        int randomIndex = Random.Range(0, availableShelves.Count);
        Transform chosenShelf = availableShelves[randomIndex];

        // Remove it from the list so no one else can pick it
        availableShelves.RemoveAt(randomIndex);

        return chosenShelf;
    }

    public void ReleaseShelf(Transform shelfToRelease)
    {
        // The customer is done, put the shelf back in the available pool
        if (shelfToRelease != null && !availableShelves.Contains(shelfToRelease))
        {
            availableShelves.Add(shelfToRelease);
        }
    }

    // --- QUEUE LOGIC ---

    public int JoinQueue(CustomerNavigation customer)
    {
        // Find the first empty spot in the queue (looping from 0 upwards)
        for (int i = 0; i < customersInQueue.Length; i++)
        {
            if (customersInQueue[i] == null) // This spot is empty!
            {
                customersInQueue[i] = customer;
                return i; // Tell the customer their index in line
            }
        }

        return -1; // -1 means the queue is completely full
    }

    public void LeaveQueueAndShift(CustomerNavigation leavingCustomer)
    {
        // Check if this customer is actually at the front of the line (Index 0)
        if (customersInQueue[0] == leavingCustomer)
        {
            customersInQueue[0] = null; // Clear the front spot

            // Shift everyone else forward by 1
            for (int i = 1; i < customersInQueue.Length; i++)
            {
                if (customersInQueue[i] != null)
                {
                    // Move them forward in the array
                    customersInQueue[i - 1] = customersInQueue[i];
                    customersInQueue[i] = null;

                    // Tell the customer to physically walk to their new spot in line
                    customersInQueue[i - 1].UpdateQueueDestination(queuePoints[i - 1], i - 1);
                }
            }
        }
    }
}