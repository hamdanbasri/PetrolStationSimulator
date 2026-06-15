using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CustomerNavigation : MonoBehaviour
{
    [Header("Destinations")]
    public Transform[] destination;
    public Transform currentDestination;
    public Transform payingDestination;
    public Transform exitDestination;
    public float distanceToDestination;
    public float distanceThreshold;

    [Header("Variables")]
    public bool agentHasDestination;
    public bool isPaying; 
    public bool isExitShop;

    [Header("Animator")]
    public Animator animator;

    [Header("Private")]
    private NavMeshAgent agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        SetIdle();

        payingDestination = GameObject.FindGameObjectWithTag("CashierPayingArea").transform;
        // 1. Find all GameObjects with the target tag
        GameObject[] shelfObjects = GameObject.FindGameObjectsWithTag("DisplayShelf");

        // 2. Initialize the array with the exact matching size
        destination = new Transform[shelfObjects.Length];

        // 3. Populate the array with each object's transform
        for (int i = 0; i < shelfObjects.Length; i++)
        {
            destination[i] = shelfObjects[i].transform;
        }

        if(shelfObjects.Length > 0)
        {
            SetDestination();
        }
        else
        {
            Debug.LogWarning("CUSTOMER: Unable to find Shelf to set destination");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (agentHasDestination)
        {
            distanceToDestination = Vector3.Distance(agent.transform.position, currentDestination.position);

            if (distanceToDestination > distanceThreshold)
            {
                SetWalking();
                agent.destination = currentDestination.position;
            }
            else
            {
                SetIdle();
                transform.position = currentDestination.position;
                transform.rotation = currentDestination.rotation;
                StartCoroutine(LookThroughShelf());
                agentHasDestination = false;                
            }
        }

        if (isPaying)
        {
            distanceToDestination = Vector3.Distance(agent.transform.position, payingDestination.position);

            if (distanceToDestination > distanceThreshold)
            {
                SetWalking();
                agent.destination = payingDestination.position;
            }
            else
            {
                SetIdle();
                transform.position = payingDestination.position;
                transform.rotation = payingDestination.rotation;
                StartCoroutine(PayAtCashier());
                isPaying = false;
            }
        }
            

        if (isExitShop)
        {
            distanceToDestination = Vector3.Distance(agent.transform.position, exitDestination.position);

            if (distanceToDestination > distanceThreshold)
            {
                SetWalking();
                agent.destination = exitDestination.position;
            }
            else
            {
                SetIdle();
                transform.position = exitDestination.position;
                transform.rotation = exitDestination.rotation;
                isExitShop = false;
            }
        }
    }

    public void SetDestination()
    {
        int randomDestination = Random.Range(0, destination.Length);
        currentDestination = destination[randomDestination];
        agentHasDestination = true;
    }

    IEnumerator LookThroughShelf()
    {
        yield return new WaitForSeconds(5);
        isPaying = true;
    }

    IEnumerator PayAtCashier()
    {
        yield return new WaitForSeconds(5);
        isExitShop = true;
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
