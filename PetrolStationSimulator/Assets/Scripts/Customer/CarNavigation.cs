using UnityEngine;
using UnityEngine.AI;

public class CarNavigation : MonoBehaviour
{
    [Header("Destinations")]
    public Transform currentDestination;
    public float distanceToDestination;
    public float distanceThreshold = 1.5f;

    [Header("Variables")]
    public float currentFuel;
    public float fuelTankSize = 50f;
    public bool agentHasDestination;
    public bool isExit;
    
    // We keep track of the pump we are assigned to, so we know if we need to interact with it
    private PetrolPumpManager assignedPump;

    [Header("Managers")]
    public CarSpawner spawner;

    [Header("Private")]
    private NavMeshAgent agent;

    void Awake()
    {
        spawner = FindAnyObjectByType<CarSpawner>();        
        agent = GetComponent<NavMeshAgent>();
    }

    void OnEnable()
    {
        currentFuel = Random.Range(5f, 20f);
        RequestPumpAssignment();
        transform.position = spawner.selectedSpawnPoint.position;
    }

    void Update()
    {
        if (agentHasDestination && currentDestination != null)
        {
            distanceToDestination = Vector3.Distance(agent.transform.position, currentDestination.position);

            if (distanceToDestination <= distanceThreshold)
            {
                agentHasDestination = false;
                
                // Optional: Snap the car perfectly into position when it arrives
                transform.position = currentDestination.position;
                transform.rotation = currentDestination.rotation;
            }
        }

        // Simulating the fuel fill-up (You'll likely change this later based on your fuel logic)
        // When full, leave the pump and head to the exit.
        if (currentFuel >= fuelTankSize && assignedPump != null)
        {
            LeaveStation();
        }

        if(isExit)
        {
            if (Vector3.Distance(transform.position, StationManager.Instance.stationExit.position) <= distanceThreshold)
            {
                // We reached the exit!
                if (spawner != null) spawner.DespawnCustomer(this.gameObject);
                else Destroy(gameObject);
            }
        }
    }

    public void RequestPumpAssignment()
    {
        // Ask the centralized manager for the best pump
        assignedPump = StationManager.Instance.GetShortestQueuePump();

        if (assignedPump != null)
        {
            // Ask the pump for a specific spot (either the pump itself, or a spot in line)
            Transform assignedSpot = assignedPump.AssignQueueSpot(this);

            if (assignedSpot != null)
            {
                UpdateDestination(assignedSpot);
            }
            else
            {
                // Edge case handled below
                LeaveStation();
            }
        }
    }

    // Called by this script, OR by the PetrolPumpManager when the line moves forward
    public void UpdateDestination(Transform newDestination)
    {
        currentDestination = newDestination;
        agent.destination = currentDestination.position;
        agentHasDestination = true;
    }

    public void LeaveStation()
    {
        assignedPump = null; // Detach from the pump
        //UpdateDestination(StationManager.Instance.stationExit);
    }

    public void ReceiveFuel(float amount)
    {
        currentFuel += amount;
    }
}