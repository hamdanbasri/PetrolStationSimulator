using UnityEngine;
using UnityEngine.AI;

public class CarNavigation : MonoBehaviour
{
    public enum MovementState { OnHighway, EnteringStation, NavigatingToPump, Refueling, Leaving }

    [Header("Current State")]
    public MovementState currentState;
    public bool wantsFuel;
    [Tooltip("Chance out of 1.0 that a spawned car wants to enter the station")]
    public float enterStationChance = 0.5f;

    [Header("Highway Movement")]
    public float highwaySpeed = 15f;
    public float rotationSpeed = 10f;
    public float collisionAvoidanceDistance = 6f;
    public LayerMask carLayer; // Assign a layer to your cars so they can see each other
    private TrafficRoute currentRoute;
    private int currentPathIndex = 0;
    private float currentSpeed;

    [Header("Destinations")]
    public Transform currentDestination;
    public float distanceToDestination;
    public float distanceThreshold = 1.5f;

    [Header("Variables")]
    public float currentFuel;
    public float fuelTankSize = 50f;
    public float refuelRate = 10f;
    public bool agentHasDestination;
    
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
        // Decide if this car is a customer or just passing by
        wantsFuel = Random.value <= enterStationChance;
        currentFuel = Random.Range(5f, 20f);
        
        // Disable NavMesh initially, we will drive mathematically on the highway
        agent.enabled = false;
        currentState = MovementState.OnHighway;
        currentSpeed = highwaySpeed;
        currentPathIndex = 0;

        // Find a random highway route to spawn on
        // Corrected: Using FindObjectsOfType (plural) to get an array
        TrafficRoute[] allRoutes = FindObjectsOfType<TrafficRoute>();

        if (allRoutes.Length > 0)
        {
            currentRoute = allRoutes[Random.Range(0, allRoutes.Length)];
            transform.position = currentRoute.pathPoints[0]; // Snap to start of route
        }
    }

    void Update()
    {
        if (currentState == MovementState.OnHighway)
        {
            UpdateHighwayMovement();
        }
        else if (currentState == MovementState.Refueling)
        {
            UpdateRefueling();
        }
        else if (agentHasDestination && currentDestination != null)
        {
            UpdateNavMeshMovement();
        }
    }

    void UpdateHighwayMovement()
    {
        if (currentRoute == null || currentPathIndex >= currentRoute.pathPoints.Count)
        {
            // Reached the end of the highway without entering the station
            if (spawner != null) spawner.DespawnCustomer(this.gameObject);
            else Destroy(gameObject);
            return;
        }

        Vector3 targetPos = currentRoute.pathPoints[currentPathIndex];
        float distanceToNextPoint = Vector3.Distance(transform.position, targetPos);

        if (distanceToNextPoint < 1f)
        {
            currentPathIndex++;
            if (currentPathIndex >= currentRoute.pathPoints.Count) return;
            targetPos = currentRoute.pathPoints[currentPathIndex];
        }

        // --- Collision Avoidance (Braking) ---
        // SphereCast forward to detect cars ahead
        RaycastHit hit;
        Vector3 forwardOrigin = transform.position + Vector3.up * 0.5f; // Cast slightly above ground
        
        if (Physics.SphereCast(forwardOrigin, 1f, transform.forward, out hit, collisionAvoidanceDistance, carLayer))
        {
            // Slow down or stop if a car is in front
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime * 5f);
        }
        else
        {
            // Accelerate back to normal speed
            currentSpeed = Mathf.Lerp(currentSpeed, highwaySpeed, Time.deltaTime * 2f);
        }

        // --- Move & Rotate ---
        Vector3 moveDirection = (targetPos - transform.position).normalized;
        transform.position += moveDirection * currentSpeed * Time.deltaTime;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void UpdateNavMeshMovement()
    {
        if (!agent.enabled) return;

        distanceToDestination = Vector3.Distance(agent.transform.position, currentDestination.position);

        if (distanceToDestination <= distanceThreshold)
        {
            agentHasDestination = false;
            
            if (currentState == MovementState.NavigatingToPump)
            {
                // We arrived at the pump! Switch to the refueling state.
                currentState = MovementState.Refueling;
            }
            else if (currentState == MovementState.Leaving)
            {
                // We arrived at the exit!
                if (spawner != null) spawner.DespawnCustomer(this.gameObject);
                else Destroy(gameObject);
            }
        }
    }

    void UpdateRefueling()
    {
        // Check if the tank is not full yet
        if (currentFuel < fuelTankSize)
        {
            ReceiveFuel(refuelRate * Time.deltaTime);
            transform.rotation = currentDestination.rotation;
            
            // NOTE: If you eventually want the player to manually pump the gas, 
            // you would remove this ReceiveFuel line and wait for the player's 
            // interaction script to call ReceiveFuel() instead!
        }
        else
        {
            // Tank is full, time to pay and leave!
            // (You can hook into your CashManager here later)
            LeaveStation();
        }
    }

    // --- TRIGGERS ---
    // Place a BoxCollider (IsTrigger = true) on the highway where the off-ramp starts. Tag it "StationEntrance"
    void OnTriggerEnter(Collider other)
    {
        if (currentState == MovementState.OnHighway && other.CompareTag("StationEntrance"))
        {
            if (wantsFuel)
            {
                // We decided to get fuel, and we found the entrance!
                EnterStation();
            }
            // If wantsFuel is false, do nothing. Just keep driving along the highway.
        }
    }

    void EnterStation()
    {
        currentState = MovementState.EnteringStation;
        
        // Enable NavMesh navigation
        agent.enabled = true;
        // Warp ensures the NavMeshAgent snaps exactly to our current mathematical position
        agent.Warp(transform.position); 

        // Start standard petrol pump logic
        RequestPumpAssignment();
    }

    public void RequestPumpAssignment()
    {
        currentState = MovementState.NavigatingToPump;
        assignedPump = StationManager.Instance.GetShortestQueuePump();

        if (assignedPump != null)
        {
            Transform assignedSpot = assignedPump.AssignQueueSpot(this);
            if (assignedSpot != null)
            {
                UpdateDestination(assignedSpot);
            }
            else LeaveStation();
        }
    }

    public void UpdateDestination(Transform newDestination)
    {
        currentDestination = newDestination;
        if(agent.isActiveAndEnabled)
        {
            agent.destination = currentDestination.position;
            agentHasDestination = true;
        }
    }

    public void LeaveStation()
    {
        currentState = MovementState.Leaving;
        assignedPump = null; 
        UpdateDestination(StationManager.Instance.stationExit);
    }

    public void ReceiveFuel(float amount)
    {
        currentFuel += amount;
    }
}