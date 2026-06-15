using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CustomerNavigation : MonoBehaviour
{
    [Header("Destinations")]
    public Transform currentShelfDestination;
    public Transform exitDestination;
    public float distanceThreshold = 0.5f;
    public float exitDistanceThreshold = 0.7f;
    public float distance;

    [Header("Variables")]
    public bool isShopping;
    public bool isWalkingToQueue;
    public bool isExitShop;
    
    // To track our spot in line
    private int myQueueIndex = -1; 
    private Transform myCurrentQueuePoint;

    [Header("Managers")]
    public CustomerSpawner spawner;
    public StoreManager storeManager; // NEW REFERENCE

    [Header("Animator")]
    public Animator animator;

    private NavMeshAgent agent;

    private void Awake()
    {
        // Cache these once to save performance
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        spawner = FindAnyObjectByType<CustomerSpawner>();
        storeManager = FindAnyObjectByType<StoreManager>();
    }

    private void OnEnable()
    {
        exitDestination = GameObject.FindGameObjectWithTag("ExitDestination").transform;
        
        // Reset states
        isShopping = false;
        isWalkingToQueue = false;
        isExitShop = false;
        myQueueIndex = -1;

        // Ask the manager for a shelf
        currentShelfDestination = storeManager.RequestEmptyShelf();

        if (currentShelfDestination != null)
        {
            isShopping = true;
            agent.destination = currentShelfDestination.position;
            SetWalking();
        }
        else
        {
            // What happens if the store is full? For now, let's just make them leave.
            SetWalking();
            Debug.LogWarning("No shelves available! Customer leaving.");
            isExitShop = true;
            agent.destination = exitDestination.position;
        }
    }

    void Update()
    {
        // 1. If shopping at the shelf
        if (isShopping)
        {
            distance = Vector3.Distance(transform.position, currentShelfDestination.position);
            if (distance <= distanceThreshold)
            {
                isShopping = false;
                SetIdle();
                transform.rotation = currentShelfDestination.rotation;
                StartCoroutine(LookThroughShelf());
            }
        }

        // 2. If walking to their spot in the queue
        if (isWalkingToQueue)
        {
            distance = Vector3.Distance(transform.position, myCurrentQueuePoint.position);
            if (distance <= distanceThreshold)
            {
                isWalkingToQueue = false;
                SetIdle();
                transform.rotation = myCurrentQueuePoint.rotation;

                // If I am at Index 0, it means I am at the cashier! Time to pay.
                if (myQueueIndex == 0)
                {
                    StartCoroutine(PayAtCashier());
                }
            }
        }

        // 3. If leaving the shop
        if (isExitShop)
        {
            distance = Vector3.Distance(transform.position, exitDestination.position);
            if (distance < exitDistanceThreshold)
            {
                // We reached the exit!
                if (spawner != null) spawner.DespawnCustomer(this.gameObject);
                else Destroy(gameObject);
            }
        }
    }

    IEnumerator LookThroughShelf()
    {
        yield return new WaitForSeconds(5); // Browsing the shelf...
        
        // Done shopping here! Release this specific shelf back to the manager 
        // so someone else can use it while we try to check out.
        storeManager.ReleaseShelf(currentShelfDestination);
        currentShelfDestination = null;

        // Try to go to the cashier
        GoToCashier();
    }

    void GoToCashier()
    {
        // Ask the manager for a spot in line
        myQueueIndex = storeManager.JoinQueue(this);

        if (myQueueIndex != -1)
        {
            // SUCCESS: We got a spot in the queue!
            myCurrentQueuePoint = storeManager.queuePoints[myQueueIndex];
            agent.destination = myCurrentQueuePoint.position;
            isWalkingToQueue = true;
            SetWalking();
        }
        else
        {
            // FAILED: The Queue is completely full! 
            // The customer decides to go look at another shelf instead of leaving.
            Debug.Log(gameObject.name + " saw the queue was full. Finding another shelf...");
            FindAnotherShelf();
        }
    }

    // --- NEW LOGIC FOR LOOPING ---

    void FindAnotherShelf()
    {
        // Ask the manager for a new shelf to look at
        currentShelfDestination = storeManager.RequestEmptyShelf();

        if (currentShelfDestination != null)
        {
            // Found a new empty shelf! Go back into the shopping state.
            isShopping = true;
            agent.destination = currentShelfDestination.position;
            SetWalking();
        }
        else
        {
            // EDGE CASE: The queue is full AND there are no empty shelves!
            // The customer will just stand still and wait for a moment, then try the queue again.
            Debug.Log("Store is completely packed! Customer is waiting...");
            StartCoroutine(WaitAndTryCashierAgain());
        }
    }

    IEnumerator WaitAndTryCashierAgain()
    {
        SetIdle();
        
        // Wait for 3 seconds (you can add an animation here later, like looking at a watch)
        yield return new WaitForSeconds(3f); 
        
        // Try the cashier line one more time!
        GoToCashier(); 
    }

    // -----------------------------

    // The StoreManager calls this when the person in front of us leaves
    public void UpdateQueueDestination(Transform newQueuePoint, int newIndex)
    {
        myQueueIndex = newIndex;
        myCurrentQueuePoint = newQueuePoint;
        
        // Start walking to the new spot in line
        agent.destination = myCurrentQueuePoint.position;
        isWalkingToQueue = true;
        SetWalking();
    }

    IEnumerator PayAtCashier()
    {
        yield return new WaitForSeconds(5); // Taking time to pay...
        
        // Done paying! Tell the manager we are leaving the line
        storeManager.LeaveQueueAndShift(this);
        
        // Head to the exit
        isExitShop = true;
        agent.destination = exitDestination.position;
        SetWalking();
    }

    void SetIdle()
    {        
        animator.SetBool("isIdle", true);
        animator.SetBool("isWalking", false);
    }

    void SetWalking()
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isWalking", true);
    }
}