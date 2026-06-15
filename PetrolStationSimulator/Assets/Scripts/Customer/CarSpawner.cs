using System.Collections;
using UnityEngine;
using UnityEngine.Pool; // Required for Unity's built-in Object Pool

public class CarSpawner : MonoBehaviour
{
    [Header("Pool Setup")]
    [Tooltip("The Customer Prefab to spawn.")]
    public GameObject carPrefab;
    
    [Tooltip("Drag your 2 (or more) spawn point Transforms here.")]
    public Transform[] spawnPoints;
    public Transform selectedSpawnPoint;

    [Header("Game Progression Limits")]
    [Tooltip("Maximum customers allowed in the station at once.")]
    public int maxStationCapacity = 5; 
    
    [Tooltip("How often (in seconds) the game tries to spawn a new customer.")]
    public float spawnInterval = 3f;

    // The built-in Unity Object Pool
    private IObjectPool<GameObject> customerPool;
    
    // Tracks how many customers are currently walking around
    private int currentActiveCustomers = 0;

    private void Awake()
    {
        // Initialize the Object Pool
        customerPool = new ObjectPool<GameObject>(
            createFunc: CreateNewCustomer,
            actionOnGet: OnPullFromPool,
            actionOnRelease: OnReturnToPool,
            actionOnDestroy: Destroy,
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 50 // Absolute maximum the pool can hold in memory
        );
    }

    private void Start()
    {
        // Start the continuous spawning loop
        StartCoroutine(SpawnRoutine());
    }

    // --- OBJECT POOL LOGIC ---

    // 1. Called when the pool is empty and needs to create a brand new object
    private GameObject CreateNewCustomer()
    {
        GameObject newCustomer = Instantiate(carPrefab);
        // Optional: If your customer needs to know who spawned them, you can set a reference here
        return newCustomer;
    }

    // 2. Called when we pull an existing customer from the pool
    private void OnPullFromPool(GameObject customer)
    {
        //customer.SetActive(true);
        currentActiveCustomers++;
    }

    // 3. Called when a customer leaves the station and goes back into the pool
    private void OnReturnToPool(GameObject customer)
    {
        customer.SetActive(false);
        currentActiveCustomers--;
    }

    // --- SPAWNING LOGIC ---

    private IEnumerator SpawnRoutine()
    {
        while (true) // Infinite loop while the game is running
        {
            yield return new WaitForSeconds(spawnInterval);

            // Check if we have room in the station based on player's current progression
            if (currentActiveCustomers < maxStationCapacity)
            {
                SpawnCustomer();
            }
        }
    }

    private void SpawnCustomer()
    {
        // Pick a random spawn point from the array
        int randomIndex = Random.Range(0, spawnPoints.Length);
        selectedSpawnPoint = spawnPoints[randomIndex];

        // Pull a customer from the pool
        GameObject customer = customerPool.Get();

        // Move them to the spawn point
        customer.transform.position = selectedSpawnPoint.position;
        customer.transform.rotation = selectedSpawnPoint.rotation;

        customer.SetActive(true);
    }

    // --- GAME PROGRESSION LOGIC ---

    // Call this method from your StationManager when the player buys an upgrade!
    public void UpgradeStationCapacity(int extraCapacity)
    {
        maxStationCapacity += extraCapacity;
        Debug.Log("Station Upgraded! Max capacity is now: " + maxStationCapacity);
    }

    // --- CLEANUP LOGIC ---

    // YOUR CUSTOMER SCRIPT MUST CALL THIS WHEN THEY LEAVE THE STATION
    public void DespawnCustomer(GameObject customer)
    {
        // Instead of Destroy(gameObject), we release them back to the pool to sleep
        customerPool.Release(customer);
    }
}