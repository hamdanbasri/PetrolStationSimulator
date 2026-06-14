using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CustomerNavigation : MonoBehaviour
{
    [Header("Destinations")]
    public Transform[] destination;
    public Transform currentDestination;
    public Transform exitDestination;
    public float distanceToDestination;
    public float distanceThreshold;

    [Header("Variables")]
    public float currentFuel;
    public float fuelTankSize;
    public bool agentHasDestination;

    [Header("Private")]
    private NavMeshAgent agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // 1. Find all GameObjects with the target tag
        GameObject[] shelfObjects = GameObject.FindGameObjectsWithTag("DisplayShelf");

        // 2. Initialize the array with the exact matching size
        destination = new Transform[shelfObjects.Length];

        // 3. Populate the array with each object's transform
        for (int i = 0; i < shelfObjects.Length; i++)
        {
            destination[i] = shelfObjects[i].transform;
        }
        SetDestination();
    }

    // Update is called once per frame
    void Update()
    {
        if (agentHasDestination)
        {
            distanceToDestination = Vector3.Distance(agent.transform.position, currentDestination.position);

            if (distanceToDestination > distanceThreshold)
            {
                agent.destination = currentDestination.position;
            }
            else
            {
                agentHasDestination = false;
            }
        }

        if (currentFuel >= fuelTankSize)
        {
            agent.destination = exitDestination.position;
        }
    }

    public void SetDestination()
    {
        int randomDestination = Random.Range(0, destination.Length);
        currentDestination = destination[randomDestination];
        agentHasDestination = true;
    }
}
